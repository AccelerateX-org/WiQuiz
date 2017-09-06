///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=ReportUnit"
#tool "nuget:?package=OctopusTools"
#tool "nuget:?package=GitVersion.CommandLine"

#addin "Cake.Figlet"

//////////////////////////////////////////////////////////////////////
// EXTERNAL SCRIPTS
//////////////////////////////////////////////////////////////////////

#load "./build/cheese.cake"
#load "./build/kitchenaid.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

// Local build flags
var runMode = RunMode.Debug;
var localBuildConfiguration = "Debug";

var projectName = "WiQuiz";
var solutionPath = File("./WIQuest.sln");
var solution = ParseSolution(solutionPath);
var projects = solution.Projects;

var inputPath = "./Sources/WiQuest/WIQuest.Web/obj/octopacked";
var outputPath = "./Output";

// Get some nice cheese cake
CheeseCake parameters = CheeseCake.getRecipe(Context, BuildSystem, runMode);

var buildSettings = new MSBuildSettings 
	{
		Verbosity = (parameters.Mode == RunMode.Debug ? Verbosity.Minimal : Verbosity.Quiet),
		Configuration = parameters.Configuration,
		DetailedSummary = true
	};

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	Information(Figlet(projectName));
	
	EnsureDirectoryExists(outputPath);

	if (parameters.IsLocalBuild) {
		Information("This is a local build! Build configuration was automatically set from {0} to {1} \n", parameters.Configuration, localBuildConfiguration);
		parameters.setConfiguration(localBuildConfiguration);
	}

	parameters.BuildSettings.Configuration = parameters.Configuration;
	parameters.setOutputPath(outputPath);

	parameters.setBuildVersion(WhichCake.getVersion(Context, parameters: parameters));

	Information("\nBuilding version {0} of {1} (Configuration: {2}, Target: {3}) using version {4} of Cake.",
    	parameters.BuildVersion.SemVersion,
        projectName,
		parameters.BuildSettings.Configuration,
        parameters.Target,
        parameters.CakeVersion
	);

	parameters.setPackagePath(inputPath);
	parameters.setPackageFile(projectName, parameters.BuildVersion.SemVersion);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});		


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Initial-Clean-Output-Directories")
    .Does(() =>
	{
		KitchenAid.cleanOutputDirectories(Context, parameters, projects);
	}
);

Task("Restore-NuGet-Packages")
	.Does(() => 
	{
		NuGetRestore(solutionPath, new NuGetRestoreSettings 
		{ 
			Verbosity = (parameters.Mode == RunMode.Debug ? NuGetVerbosity.Normal : NuGetVerbosity.Quiet)  
		});
	}
);

Task("PreBuild-For-Testing")
	.IsDependentOn("Initial-Clean-Output-Directories")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() => 
	{	
		MSBuild(solutionPath, parameters.BuildSettings);		
	}
);

Task("Run-Unit-Tests")
	.IsDependentOn("PreBuild-For-Testing")
	.Does(() => 
	{
		var testAssemblies = GetFiles("./Sources/WiQuest/**/bin/" + parameters.Configuration + "/*.Test.dll");
		XUnit2(testAssemblies, new XUnit2Settings 
		{
			Parallelism = ParallelismOption.All,
            HtmlReport = false,
            NoAppDomain = true,
            XmlReport = true,
            OutputDirectory = "./TestResults"	
		});
	}
).OnError(exception =>
{  
	ReportUnit("./TestResults/");
});

Task("Clean-Output-Directories")
	.IsDependentOn("Run-Unit-Tests")
	.Does(() => 
	{
		KitchenAid.cleanOutputDirectories(Context, parameters, projects);
	}
);

Task("Build-Package")
	.IsDependentOn("Clean-Output-Directories")
	.Does(() => 
	{
		parameters.BuildSettings.WithProperty("RunOctoPack", "true");
		parameters.BuildSettings.WithProperty("OctoPackPackageVersion", parameters.BuildVersion.SemVersion);
		
		MSBuild(solutionPath, parameters.BuildSettings);
		
		MoveFileToDirectory(parameters.PackageInput, parameters.OutputPath);
	}
);

Task("Push-To-Package-Feed")
	.WithCriteria(parameters.ShouldPublishToFeed)
	.WithCriteria(!parameters.IsLocalBuild)
	.IsDependentOn("Build-Package")
	.Does(() => 
	{
		OctoPush(parameters.OctopusDeploy.Url, parameters.OctopusDeploy.ApiKey, new FilePath(parameters.PackageOutput),
      		new OctopusPushSettings {
        		ReplaceExisting = true
      		}
		);
	}
);

Task("Create-Release-From-Package")
	.WithCriteria(parameters.ShouldDeploy)
	.WithCriteria(!parameters.IsLocalBuild)
	.IsDependentOn("Push-To-Package-Feed")
	.Does(() => 
	{
		OctoCreateRelease(projectName, new CreateReleaseSettings 
		{
        	Server = parameters.OctopusDeploy.Url,
        	ApiKey = parameters.OctopusDeploy.ApiKey,
        	ReleaseNumber = parameters.BuildVersion.SemVersion,
			Packages = new Dictionary<string, string>
            {
                { 
					projectName, parameters.BuildVersion.SemVersion 
				}
            },
      	});
	}	
);

Task("Deploy-Package")
	.WithCriteria(parameters.ShouldDeploy)
	.WithCriteria(!parameters.IsLocalBuild)
	.IsDependentOn("Push-To-Package-Feed")
	.IsDependentOn("Create-Release-From-Package")
	.Does(() => 
	{
		OctoDeployRelease
		(
			parameters.OctopusDeploy.Url, 
			parameters.OctopusDeploy.ApiKey, 
			projectName, 
			"Dev",
			parameters.BuildVersion.SemVersion, 
			new OctopusDeployReleaseDeploymentSettings 
			{
        		ShowProgress = true
    		}
		);
	}	
);

Task("Upload-Artifacts")
	.WithCriteria(parameters.IsRunningOnAppVeyor)
	.IsDependentOn("Build-Package")
	.Does(() => 
	{
		AppVeyor.UploadArtifact(parameters.PackageOutput);
	}	
);

Task("Default")
    .IsDependentOn("Build-Package");

Task("AppVeyor")
	.IsDependentOn("Deploy-Package")
    .IsDependentOn("Upload-Artifacts");

RunTarget(target);

/*

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////


var isAppVeyorBuild = AppVeyor.IsRunningOnAppVeyor;
var isLocal = BuildSystem.IsLocalBuild;

var projectName = "WiQuiz";
var version =  "";

var buildConfiguration = "Debug";

if (isLocal) 
{
	version = "0.0.1";
}

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./Sources/WiQuest/WIQuest.Web/WIQuest.Web.sln");
var parentSolutionPath = File("./WIQuest.sln");

var buildSettings = new MSBuildSettings 
		{
			Verbosity = Verbosity.Minimal,
			Configuration = buildConfiguration,
			DetailedSummary = true
		};
buildSettings.WithTarget("Build");
if (isAppVeyorBuild)
{
	buildSettings.WithLogger("C:/Program Files/AppVeyor/BuildAgent/Appveyor.MSBuildLogger.dll");
}
buildSettings.WithProperty("RunOctoPack", "true");

var OCTO_URL = "https://cd.acceleratex.org/octopus/";
var OCTO_API_KEY = EnvironmentVariable("OCTO_API_KEY");

GitVersion versionInfo = null;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	Information(Figlet("WiQuiz"));
});

Teardown(context =>
{
    Information("Finished running tasks.");
});		

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Restore-NuGet-Packages")
	.Does(() => 
	{
		Information("Restore-NuGet-Packages");	
		NuGetRestore(parentSolutionPath, new NuGetRestoreSettings { Verbosity = NuGetVerbosity.Normal });
		//NuGetRestore(solutionPath, new NuGetRestoreSettings { Verbosity = NuGetVerbosity.Normal });
	}
);

Task("Version")
    .Does(() => {
		if (isAppVeyorBuild)
		{
			GitVersion(new GitVersionSettings{
            	UpdateAssemblyInfo = true,
            	OutputType = GitVersionOutput.BuildServer
        	});
		}	
		versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
		Information("Version: " + versionInfo.SemVer);			
    }
);

/*
Task("Copy-NuGet-Packages")
	.Does(() => 
	{
		Information("Copy-NuGet-Packages");	
		MoveFiles("/packages/*", "./Sources/WiQuest/WIQuest.Web/packages");
	}
);*/

/*
Task("Build")
	.IsDependentOn("Version")
	.IsDependentOn("Restore-NuGet-Packages")
	.Does(() => 
	{
		Information("Building Solution");	
		buildSettings.WithProperty("OctoPackPackageVersion", versionInfo.SemVer);
		MSBuild(parentSolutionPath, buildSettings);
	}
);

Task("Test")
	.IsDependentOn("Restore-NuGet-Packages")
	.IsDependentOn("Build")
	.Does(() => 
	{
		Information("Testing Solution");
		var testAssemblies = GetFiles("./Sources/WiQuest/**bin/" + buildConfiguration + "/*.Test.dll");
		XUnit2(testAssemblies);
	}
);

Task("Packaging")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.Does(() => 
	{
		Information("Packaging");	
		/*NuGetPack("./Sources/WiQuest/WiQuest.Web/WiQuest.Web.nuspec", new NuGetPackSettings 
		{ 
			Id = projectName,	
			Version = version,
			Files = new [] { new NuSpecContent { Source = "WIQuest.Web.dll", Target = "bin"} },
			BasePath = "./Sources/WiQuest/WiQuest.Web/bin",
            OutputDirectory = "./nuget"
		});*
		/*OctoPack(projectName, new OctopusPackSettings
        {
            Format = OctopusPackFormat.NuPkg,
			Version = version,
            OutFolder = "./octopacked/",
            BasePath = "./",
            Overwrite = true,
        });
	}
);

Task("Octopus-Push")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.Does(() => 
	{
		Information("Octopus-Push");
		OctoPush(OCTO_URL, OCTO_API_KEY, new FilePath("./Sources/WiQuest/WIQuest.Web/obj/octopacked/WiQuiz." + versionInfo.SemVer + ".nupkg"),
      		new OctopusPushSettings {
        		ReplaceExisting = true
      		}
		);
	}
);

Task("Octopus-Release")
	.IsDependentOn("Octopus-Push")
	.Does(() => 
	{
		Information("Octopus-Release");
		/*if (isLocal) {
			OctoCreateRelease(projectName, new CreateReleaseSettings {
        		Server = "http://192.168.2.240",
        		ApiKey = "API-T5HX0K7HBUOQKMFBR2KTTK4",
        		ReleaseNumber = version,
				Packages = new Dictionary<string, string>
                     {
                         { "WiQuiz", version }
                     },
      		});
		}
	}
);

Task("Octopus-Deploy")
	.IsDependentOn("Octopus-Release")
	.Does(() => 
	{
		Information("Octopus-Deploy");
		/*if (isLocal) {
			 OctoDeployRelease("http://192.168.2.240", "API-T5HX0K7HBUOQKMFBR2KTTK4", projectName, "Testing", version, new OctopusDeployReleaseDeploymentSettings {
         		ShowProgress = true,
    		 });
		}
	}
);

Task("Upload-Artifacts")
	.IsDependentOn("Octopus-Deploy")
	.Does(() => 
	{
		Information("Upload-Artifacts");	
		if (isAppVeyorBuild)
		{
			AppVeyor.UploadArtifact("./Sources/WiQuest/WIQuest.Web/obj/octopacked/WiQuiz." + versionInfo.SemVer + ".nupkg");
		}
		//AppVeyor.UploadArtifact("./nuget/" + projectName + "." + version + ".nupkg");
	}
);

Task("Default")
	.IsDependentOn("Upload-Artifacts");

RunTarget(target);*/

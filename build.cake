///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OctopusTools"
#tool "nuget:?package=GitVersion.CommandLine"

#addin "Cake.Figlet"

//////////////////////////////////////////////////////////////////////
// EXTERNAL SCRIPTS
//////////////////////////////////////////////////////////////////////

#load "./build/cheese.cake"

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
	
	if (parameters.IsLocalBuild) {
		Information("This is a local build! Build configuration was automatically set from {0} to {1} \n", parameters.Configuration, localBuildConfiguration);
		parameters.setConfiguration(localBuildConfiguration);
	}

	parameters.BuildSettings.Configuration = parameters.Configuration;

	parameters.setBuildVersion(WhichCake.getVersion(Context, parameters: parameters));

	Information("\nBuilding version {0} of {1} (Configuration: {2}, Target: {3}) using version {4} of Cake.",
    	parameters.BuildVersion.SemVersion,
        projectName,
		parameters.Configuration,
        parameters.Target,
        parameters.CakeVersion
	);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});		


///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean-Output-Directories")
    .Does(() =>
{
	foreach(var project in projects) {
		
		if (project.Type != "{2150E333-8FDC-42A3-9474-1A3956D46DE8}") {
			
			Information("Cleaning {0} @ {1}", project.Path, parameters.Configuration);
			
			var dir = project.Path.GetDirectory();
			CleanDirectories(dir + "/bin/" + parameters.Configuration);
			CleanDirectories(dir + "/obj/" + parameters.Configuration);
		
		}
	}
});

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
	.IsDependentOn("Clean-Output-Directories")
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
		XUnit2(testAssemblies);
	}
);

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

/*Task("AppVeyor")
    .IsDependentOn("Clean");*/

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

#tool "nuget:?package=xunit.runner.console"
#tool "nuget:?package=OctopusTools"
#addin nuget:?package=Cake.AppVeyor
#tool "nuget:?package=GitVersion.CommandLine"
#addin "Cake.Figlet"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

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
		var testAssemblies = GetFiles("./Sources/WiQuest/**/bin/" + buildConfiguration + "/*.Test.dll");
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
        });*/
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
		}*/
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
		}*/
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

RunTarget(target);

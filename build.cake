#tool "nuget:?package=OctopusTools"
#addin nuget:?package=Cake.AppVeyor
#addin "Cake.Figlet"
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

var isAppVeyorBuild = AppVeyor.IsRunningOnAppVeyor;
var isLocal = BuildSystem.IsLocalBuild;

var projectName = "WiQuiz";
var version =  EnvironmentVariable("APPVEYOR_BUILD_VERSION");

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
			Configuration = "Release",
			DetailedSummary = true
		};
buildSettings.WithTarget("Build");
if (isAppVeyorBuild)
{
	buildSettings.WithLogger("C:/Program Files/AppVeyor/BuildAgent/Appveyor.MSBuildLogger.dll");
}
buildSettings.WithProperty("RunOctoPack", "true");
buildSettings.WithProperty("OctoPackPackageVersion", version);

var OCTO_URL = "http://5.175.5.136";
var OCTO_API_KEY = EnvironmentVariable("OCTO_API_KEY");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	Information(Figlet("WiQuiz"));

	Information(isAppVeyorBuild);
	Information(isLocal);
	Information(version);
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
		NuGetRestore(solutionPath, new NuGetRestoreSettings { Verbosity = NuGetVerbosity.Normal });
	}
);

Task("Version")
    .Does(() => {
		if (!isLocal) {
			GitVersion(new GitVersionSettings {
            	//UpdateAssemblyInfo = true,
            	OutputType = GitVersionOutput.BuildServer
        	});
		}
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
		MSBuild(solutionPath, buildSettings);
	}
);

Task("Packaging")
	.IsDependentOn("Build")
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
	.Does(() => 
	{
		Information("Octopus-Push");
		OctoPush(OCTO_URL, OCTO_API_KEY, new FilePath("./Sources/WiQuest/WIQuest.Web/obj/octopacked/WiQuiz." + version + ".nupkg"),
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
			AppVeyor.UploadArtifact("./Sources/WiQuest/WIQuest.Web/obj/octopacked/WiQuiz." + version + ".nupkg");
		}
		//AppVeyor.UploadArtifact("./nuget/" + projectName + "." + version + ".nupkg");
	}
);

Task("Default")
	.IsDependentOn("Upload-Artifacts");

RunTarget(target);
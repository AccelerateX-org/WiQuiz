#tool "nuget:?package=OctopusTools"
#addin nuget:?package=Cake.AppVeyor
#addin "Cake.Figlet"
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var projectName = "WiQuest";
var version =  EnvironmentVariable("APPVEYOR_BUILD_VERSION");



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
buildSettings.WithLogger("C:/Program Files/AppVeyor/BuildAgent/Appveyor.MSBuildLogger.dll");
buildSettings.WithProperty("RunOctoPack", "true");
buildSettings.WithProperty("OctoPackPackageVersion", version);

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	Information("");
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
		NuGetRestore(solutionPath, new NuGetRestoreSettings { Verbosity = NuGetVerbosity.Normal });
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

Task("Upload-Artifacts")
	.IsDependentOn("Packaging")
	.Does(() => 
	{
		Information("Upload-Artifacts");	
		if (AppVeyor.IsRunningOnAppVeyor)
		{
			AppVeyor.UploadArtifact("./Sources/WiQuest.Web/WiQuest/obj/octopacked/WiQuiz." + version + ".nupkg");
		}
		//AppVeyor.UploadArtifact("./nuget/" + projectName + "." + version + ".nupkg");
	}
);

Task("Default")
	.IsDependentOn("Upload-Artifacts");

RunTarget(target);
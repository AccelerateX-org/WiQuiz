#tool "nuget:?package=OctopusTools"
#addin nuget:?package=Cake.AppVeyor
///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var projectName = "WiQuest";
var version =  EnvironmentVariable("APPVEYOR_BUILD_VERSION");

Information(version);	

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./Sources/WiQuest/WIQuest.Web/WIQuest.Web.sln");
var parentSolutionPath = File("./WIQuest.sln");

var buildSettings = new MSBuildSettings 
		{
			Verbosity = Verbosity.Minimal,
			Configuration = "Release",
		};
buildSettings.WithTarget("Build");
buildSettings.WithLogger("C:/Program Files/AppVeyor/BuildAgent/Appveyor.MSBuildLogger.dll");
buildSettings.WithProperty("RunOctoPack", "true");
buildSettings.WithProperty("OctoPackPackageVersion", version);		

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Restore-NuGet-Packages")
	.Does(() => 
	{
		Information("Restore-NuGet-Packages");	
		//NuGetRestore(parentSolutionPath, new NuGetRestoreSettings { Verbosity = NuGetVerbosity.Quiet });
		//NuGetRestore(solutionPath, new NuGetRestoreSettings { Verbosity = NuGetVerbosity.Quiet });
		NuGetRestore(parentSolutionPath);
		NuGetRestore(solutionPath);
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
		MSBuild(parentSolutionPath, buildSettings);
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
		//AppVeyor.UploadArtifact("./octopacked/WiQuiz." + version + ".nupkg");
		//AppVeyor.UploadArtifact("./nuget/" + projectName + "." + version + ".nupkg");
	}
);

Task("Default")
	.IsDependentOn("Upload-Artifacts");

RunTarget(target);
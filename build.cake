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
#tool "nuget:?package=Codecov"
#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=GitReleaseNotes"
#tool "nuget:?package=OctopusTools"
#tool "nuget:?package=OpenCover"
#tool "nuget:?package=ReportGenerator"

#addin "Cake.Figlet"
#addin "nuget:?package=Cake.Git"
#addin "nuget:?package=Cake.Codecov"


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
var repositoryUrl = "https://github.com/AccelerateX-org/WiQuiz";
var repositoryCommitUrl = "https://github.com/AccelerateX-org/WiQuiz/commit/";

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

if (parameters.IsRunningOnAppVeyor)
{
	buildSettings.WithLogger("C:/Program Files/AppVeyor/BuildAgent/Appveyor.MSBuildLogger.dll");
}	

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
	Information(Figlet(projectName));
	
	EnsureDirectoryExists(outputPath);

	// TODO: Refactor
	EnsureDirectoryExists("./TestResults");

	if (parameters.IsLocalBuild) {
		Information("This is a local build! Build configuration was automatically set from {0} to {1} \n", parameters.Configuration, localBuildConfiguration);
		parameters.setConfiguration(localBuildConfiguration);
	}

	parameters.BuildSettings.Configuration = parameters.Configuration;
	parameters.setOutputPath(outputPath);

	parameters.setBuildVersion(WhichCake.getVersion(Context, parameters: parameters));

	parameters.setRepository(new ScmRepository(repositoryUrl, repositoryCommitUrl));

	parameters.setGitLog(depth: 10);

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


//////////////////////////////////////////////-Notes/////////////////////////////////
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

Task("Calculate-Coverage")
	.IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var testAssemblies = GetFiles("./Sources/WiQuest/**/bin/" + parameters.Configuration + "/*.Test.dll");
	OpenCover(tool => { tool.XUnit2(
			testAssemblies, 
			new XUnit2Settings { 
				ShadowCopy = false
				}
			);
		},
  		new FilePath("./TestResults/coverage.xml"),
  		new OpenCoverSettings()
    	.WithFilter("+[WIQuest*]*")
    	.WithFilter("-[WIQuest*.Test]*"));

		//Workaround - To be Removed
		ReportGenerator("./TestResults/coverage.xml", "./TestResults");
});

Task("Upload-Coverage")
	.IsDependentOn("Calculate-Coverage")
	.WithCriteria(parameters.IsRunningOnAppVeyor)
    .Does(() =>
{
    var buildVersion = string.Format("{0}.build.{1}",
        parameters.BuildVersion.SemVersion,
        BuildSystem.AppVeyor.Environment.Build.Version
    );

    var settings = new CodecovSettings {
        Files = new[] { "TestResults/coverage.xml" },
        //EnvironmentVariables = new Dictionary<string,string> { { "APPVEYOR_BUILD_VERSION", buildVersion } }
    };
	
	Codecov(settings);
});

Task("Clean-Output-Directories")
	.IsDependentOn("Upload-Coverage")
	.Does(() => 
	{
		KitchenAid.cleanOutputDirectories(Context, parameters, projects);
	}
);

Task("Generate-Package-Notes")
	.IsDependentOn("Clean-Output-Directories")
	.Does(() => 
	{
		var file = parameters.OutputPath + "/packagenotes.txt";
		System.IO.File.Create(file).Dispose();
		System.IO.File.WriteAllText(file, parameters.parseGitLog(NoteFormat.Plain));
	}
);

Task("Generate-Release-Notes")
	.IsDependentOn("Clean-Output-Directories")
	.IsDependentOn("Generate-Package-Notes")
	.Does(() => 
	{
			// Using GitReleaseNotes() fails with ErrorCode 2 -> Workaround; should be refactored
			// Credit: https://github.com/mwhelan/Specify/blob/master/build.cake
			// Credit: http://www.michael-whelan.net/continuous-delivery-github-cake-gittools-appveyor/
			
			var releaseNotesExitCode = StartProcess(@"tools\GitReleaseNotes\tools\gitreleasenotes.exe", 
				new ProcessSettings 
				{ 
					Arguments = String.Format(". /OutputFile {0}/releasenotes.md /Version {1} /RepoBranch {2} /verbose /AllTags /AllLabels", 
												parameters.OutputPath,
												parameters.BuildVersion.SemVersion,
												"build-delivery") 
				});


			/*var releaseNote = ParseReleaseNotes(outputPath + "/releasenotes.md");
			Information("Version: {0}", releaseNote.Version);
			foreach(var note in releaseNote.Notes)
			{
    			Information("\t{0}", note);
			}*/
	
	/*if (string.IsNullOrEmpty(System.IO.File.ReadAllText("./artifacts/releasenotes.md")))
		System.IO.File.WriteAllText("./artifacts/releasenotes.md", "No issues closed since last release");*/

		if (releaseNotesExitCode != 0) 
		{
			throw new Exception("Failed to generate release notes");
		}
	}
);

Task("Build-Package")
	.IsDependentOn("Clean-Output-Directories")
	.IsDependentOn("Generate-Package-Notes")
	.Does(() => 
	{
		parameters.BuildSettings.WithProperty("RunOctoPack", "true");
		parameters.BuildSettings.WithProperty("OctoPackPackageVersion", parameters.BuildVersion.SemVersion);
		parameters.BuildSettings.WithProperty("OctoPackReleaseNotesFile", "../../../" + parameters.OutputPath + "/packagenotes.txt");
		
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
			ReleaseNotes = parameters.parseGitLog(NoteFormat.Markdown),
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

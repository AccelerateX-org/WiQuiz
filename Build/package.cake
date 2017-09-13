///////////////////////////////////////////////////////////////////////////////
// Octopus Deployment Package via MSBuild Task
///////////////////////////////////////////////////////////////////////////////

Task("Pre-Package-CleanUp")
	.Does(() =>
	{
		Information("Cleaning...");
    	
		// TODO: GetOutputAssemblies(new FilePath("test.sln"), "Release");

		var solution = ParseSolution(BuildParameters.SolutionFilePath);
		var projects = solution.Projects;
		
		foreach(var project in projects) {
		
            // If no Solution Folder
			if (project.Type != "{2150E333-8FDC-42A3-9474-1A3956D46DE8}") {
                
            Information("Cleaning {0}", project.Path);
                
            var dir = project.Path.GetDirectory();
            
			CleanDirectories(dir + "/bin");
            CleanDirectories(dir + "/obj");
		
		    }
	    }
	}
);

Task("Generate-Changelog")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		EnsureDirectoryExists(BuildParameters.Paths.Directories.Build);

		var file = BuildParameters.Paths.Directories.Build.ToString() + "/Changelog.txt";
		System.IO.File.Create(file).Dispose();
		System.IO.File.WriteAllText(file, GetGitLog(
				format: NoteFormat.Plain,
				depth: 10
			)
		);
	}
);

Task("Build-Package")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Pre-Package-CleanUp")
	.IsDependentOn("Generate-Changelog")
	.Does(() => 
	{
		var msbuildSettings = new MSBuildSettings()
		        .SetPlatformTarget(ToolSettings.BuildPlatformTarget)
                .UseToolVersion(ToolSettings.BuildMSBuildToolVersion)
				.SetMaxCpuCount(ToolSettings.MaxCpuCount)
                .SetConfiguration(BuildParameters.Configuration)
                .WithTarget("Build")
				.WithProperty("RunOctoPack", "true")
				.WithProperty("OctoPackPackageVersion", BuildParameters.Version.SemVersion)
				.WithProperty("OctoPackReleaseNotesFile", MakeAbsolute(BuildParameters.Paths.Directories.Build) + "/Changelog.txt")
				.WithProperty("OctoPackPublishPackageToFileShare", MakeAbsolute(BuildParameters.Paths.Directories.PublishedApplications).ToString());

		MSBuild(BuildParameters.SolutionFilePath, msbuildSettings);	

		if (BuildParameters.IsRunningOnAppVeyor) 
		{
			foreach(var package in GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/*.nupkg"))
    		{
        		AppVeyor.UploadArtifact(package);
    		}	
		}
	}
);

///////////////////////////////////////////////////////////////////////////////
// Target Definiton
///////////////////////////////////////////////////////////////////////////////

Task("Octopus-Packaging")
    .IsDependentOn("Build-Package");

///////////////////////////////////////////////////////////////////////////////
// Helpers
///////////////////////////////////////////////////////////////////////////////    
	
public string GetGitLog(NoteFormat format, int depth = 10) 
{
	var gitLog = GitLog("./", depth);
	
	var log = "";

    if (format == NoteFormat.Plain)
    {
    	log += string.Format("Changelog: Last {0} Commit(s)\n\n", gitLog.Count() > 1 ? gitLog.Count().ToString() : "");
    	foreach (var item in gitLog)
    	{
    		log += string.Format("Sha: {0} - {1} ({2}) - {3} - {4} \n", item.Sha, item.Author.Name, item.Author.Email, item.Author.When, item.MessageShort);
    	}
	}

	if (format == NoteFormat.Markdown) {
		log += string.Format("### Changelog: Last {0} Commit(s)\n", gitLog.Count() > 1 ? gitLog.Count().ToString() : "");
		foreach (var item in gitLog)
		{
			log += string.Format("* __{0}__ (Commit: [{1}]({2}))\n", item.MessageShort, item.Sha, ScmRepository.CommitUrl + item.Sha);  
			log += string.Format("  * Author: {0} ([{1}](mailto:{1}))\n", item.Author.Name, item.Author.Email);
			log += string.Format("  * Date: {0}\n", item.Author.When);
		}            
	}

	return log;
}

public enum NoteFormat {
    Plain,
    Markdown
}

public static class ScmRepository
{
    public static string Url { get; private set; }
    public static string CommitUrl { get; private set; }

    public static void setRepositoryDetails() {
		Url = string.Format("https://github.com/{0}/{1}", BuildParameters.RepositoryOwner, BuildParameters.RepositoryName);
		CommitUrl = Url + "/commit/";
	}
}

    	
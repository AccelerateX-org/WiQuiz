///////////////////////////////////////////////////////////////////////////////
// Octopus Deploy - Deployment
///////////////////////////////////////////////////////////////////////////////


Task("Push-To-Package-Feed")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.IsDependentOn("Build-Package")
	.Does(() => 
	{
        OctoPush(RPS.Api.Octopus.Endpoint, RPS.Api.Octopus.ApiKey, GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/*.nupkg"),
      		new OctopusPushSettings {
        		ReplaceExisting = true
      		}
		);
	}
);

Task("Create-Release-From-Package")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.IsDependentOn("Push-To-Package-Feed")
	.Does(() => 
	{
		OctoCreateRelease(BuildParameters.RepositoryName, new CreateReleaseSettings 
		{
        	Server = RPS.Api.Octopus.Endpoint,
        	ApiKey = RPS.Api.Octopus.ApiKey,
        	ReleaseNumber = RPS.BuildVersion,
			ReleaseNotes = RPS. ParseGitLog(format: NoteFormat.Markdown),
			Packages = new Dictionary<string, string>
            {
                { 
					BuildParameters.RepositoryName, 
					RPS.BuildVersion
				}
            },
      	});
	}	
);

Task("Deploy-Package")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.IsDependentOn("Push-To-Package-Feed")
	.IsDependentOn("Create-Release-From-Package")
	.Does(() => 
	{
		OctoDeployRelease
		(
			RPS.Api.Octopus.Endpoint, 
			RPS.Api.Octopus.ApiKey, 
			BuildParameters.RepositoryName, 
			"Dev",
			RPS.BuildVersion, 
			new OctopusDeployReleaseDeploymentSettings 
			{
        		ShowProgress = true
    		}
		);
        
		var client = RPS.Octopus;
        client.Connect();

        Information("Target: " + client.GetDeploymentInformation(RPS.BuildVersion).Target);
	}	
);


///////////////////////////////////////////////////////////////////////////////
// Target Definiton
///////////////////////////////////////////////////////////////////////////////


Task("Octopus-Deployment")
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .IsDependentOn("Push-To-Package-Feed")
	.IsDependentOn("Create-Release-From-Package")
	.IsDependentOn("Deploy-Package");
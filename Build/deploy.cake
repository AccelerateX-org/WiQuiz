#tool "nuget:?package=OctopusTools"

///////////////////////////////////////////////////////////////////////////////
// Octopus Deploy - Deployment
///////////////////////////////////////////////////////////////////////////////


Task("Push-To-Package-Feed")
	.WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => BuildParameters.IsMainRepository)
	.IsDependentOn("Build-Package")
	.Does(() => 
	{
        OctoPush(RpsApi.Octopus.Url, RpsApi.Octopus.ApiKey, GetFiles(BuildParameters.Paths.Directories.PublishedApplications + "/*.nupkg"),
      		new OctopusPushSettings {
        		ReplaceExisting = true
      		}
		);
	}
);


///////////////////////////////////////////////////////////////////////////////
// Target Definiton
///////////////////////////////////////////////////////////////////////////////


Task("Octopus-Deployment")
    .WithCriteria(() => !BuildParameters.IsPullRequest)
    .IsDependentOn("Push-To-Package-Feed");


///////////////////////////////////////////////////////////////////////////////
// Helpers
///////////////////////////////////////////////////////////////////////////////    


public class ApiCredentials
{
    public string Url { get; private set; }
    public string ApiKey { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }

    public ApiCredentials(string url, string apiKey, string userName = null, string password = null)
    {
        Url = url;
        ApiKey = apiKey;
        UserName = userName;
        Password = password;
    }
}

public static class RpsApi
{
    public static ApiCredentials Octopus { get; private set; }

    public static void SetCredentials(ICakeContext context) 
    {
        if (context == null) 
        {
            throw new ArgumentNullException("Missing context");
        }  
        Octopus = new ApiCredentials(
            url: context.EnvironmentVariable("OCTO_URL"),
            apiKey: context.EnvironmentVariable("OCTO_API_KEY")
        );
    }
}





/*






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
);*/
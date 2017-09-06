#load "./which.cake"

public class CheeseCake 
{
    // Mode
    public RunMode Mode { get; private set; }

    // Current Cake Version
    public string CakeVersion { get; private set; }
    
    // Build Configuration
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public MSBuildSettings BuildSettings { get; set; }
    public string OutputPath { get; set; }
    
    // Build System
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsRunningOnWindows { get; private set; }

    // Packaging
    public string PackagePath { get; private set; }
    public string PackageFilePattern { get; private set; }
    public string PackageFile { get; private set; }

    // Git Information
    public bool IsPullRequest { get; private set; }

    // Git Version
    public WhichCake BuildVersion { get; private set; }

    // OctopusDeploy    
    public ApiCredentials OctopusDeploy { get; private set; }
    
    public bool ShouldPublishToFeed 
    {
        get 
        {
            return false;
        }
    }

    public bool ShouldDeploy
    {
        get 
        {
            return false;
        }
    }

    public string PackageInput 
    {
        get 
        {
            return PackagePath + "/" + PackageFile;
        }
    }

    public string PackageOutput 
    {
        get
        {
            return OutputPath + "/" + PackageFile;
        }
    }

    public void setConfiguration(string config) {
        Configuration = config;
    }

    public void setOutputPath(string path) {
        OutputPath = path;
    }

    public void setPackagePath(string path) {
        PackagePath = path;
    }

    public void setPackageFile(string project, string version) {
        PackageFile = System.String.Format(PackageFilePattern, project, version);
    }
    
    public void setBuildVersion(WhichCake version)
    {
        BuildVersion = version;
    }
    
    public static CheeseCake getRecipe(ICakeContext ctx, BuildSystem buildSystem, RunMode mode = RunMode.Production) 
    {
        if (ctx == null) 
        {
            throw new ArgumentNullException("Missing context @ CheeseCake.getRecipe()");
        }   
        
        return new CheeseCake {
            Mode = mode,
            Target = ctx.Argument("target", "default"),
            Configuration = ctx.Argument("configuration", "release"),
            BuildSettings = new MSBuildSettings 
	            {
		            Verbosity = (mode == RunMode.Debug ? Verbosity.Minimal : Verbosity.Quiet),
		            DetailedSummary = true
	            },
            CakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString(),
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsRunningOnWindows = ctx.IsRunningOnWindows(),
            OctopusDeploy = new ApiCredentials(url: "https://cd.acceleratex.org/octopus/", apiKey: ctx.EnvironmentVariable("OCTO_API_KEY")),
            PackageFilePattern = "{0}.{1}.nupkg"
        };    
    }
}

public enum RunMode {
    Production,
    Debug
}

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
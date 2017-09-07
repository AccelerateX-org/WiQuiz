#load "./which.cake"

public class CheeseCake 
{
    private ICakeContext _context;
    
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
    public ScmRepository Repository { get; private set; }
    public ICollection<GitCommit> GitLog { get; private set; }
    public int GitLogDepth { get; private set; }
    public bool IsPullRequest { get; private set; }

    // Git Version
    public WhichCake BuildVersion { get; private set; }

    // OctopusDeploy    
    public ApiCredentials OctopusDeploy { get; private set; }
    
    public bool ShouldPublishToFeed 
    {
        get 
        {
            return true;
        }
    }

    public bool ShouldDeploy
    {
        get 
        {
            return true;
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
        PackageFile = String.Format(PackageFilePattern, project, version);
    }
    
    public void setBuildVersion(WhichCake version)
    {
        BuildVersion = version;
    }

    public void setRepository(ScmRepository rep) {
        Repository = rep;
    }

    public void setGitLog(int depth) 
    {
        GitLogDepth = depth;
        GitLog = _context.GitLog("./", depth);
    }

    public string parseGitLog(NoteFormat format) 
    {
        var log = "";

        if (format == NoteFormat.Plain)
        {
            log += string.Format("Change History: Last {0} Commit(s)\n\n", GitLog.Count() > 1 ? GitLog.Count().ToString() : "");
            foreach (var item in GitLog)
            {
                log += string.Format("Sha: {0} - {1} ({2}) - {3} - {4} \n", item.Sha, item.Author.Name, item.Author.Email, item.Author.When, item.MessageShort);
            }
        }

        if (format == NoteFormat.Markdown) {
            log += string.Format("### Change History: Last {0} Commit(s)\n", GitLog.Count() > 1 ? GitLog.Count().ToString() : "");
            foreach (var item in GitLog)
            {
                log += string.Format("* __{0}__ (Commit: [{1}]({2}))\n", item.MessageShort, item.Sha, Repository.CommitUrl + item.Sha);  
                log += string.Format("  * Author: {0} ([{1}](mailto:{1}))\n", item.Author.Name, item.Author.Email);
                log += string.Format("  * Date: {0}\n", item.Author.When);
            }            
        }

        return log;
    }
    
    public static CheeseCake getRecipe(ICakeContext ctx, BuildSystem buildSystem, RunMode mode = RunMode.Production) 
    {
        if (ctx == null) 
        {
            throw new ArgumentNullException("Missing context @ CheeseCake.getRecipe()");
        }   
        
        return new CheeseCake {
            _context = ctx,
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

public enum NoteFormat {
    Plain,
    Markdown
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

public class ScmRepository
{
    public string Url { get; private set; }
    public string CommitUrl { get; private set; }

    public ScmRepository(string url, string commitUrl)
    {
        Url = url;
        CommitUrl = commitUrl;
    }
}


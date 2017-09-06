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
    
    // Build System
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsRunningOnWindows { get; private set; }

    // Git Information
    public bool IsPullRequest { get; private set; }

    // Git Version
    public WhichCake BuildVersion { get; private set; }

    public void setConfiguration(string config) {
        Configuration = config;
    }
    
    public void setBuildVersion(WhichCake version)
    {
        BuildVersion = version;
    }
    
    public static CheeseCake getRecipe(ICakeContext ctx, BuildSystem buildSystem, RunMode mode = RunMode.Production) 
    {
        if (ctx == null) 
        {
            throw new ArgumentNullException("Missing context @ CheeseCake");
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
            IsRunningOnWindows = ctx.IsRunningOnWindows()
        };    
    }
}

public enum RunMode {
    Production,
    Debug
}
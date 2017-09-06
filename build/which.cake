public class WhichCake {

    public string Version { get; private set; }  
    public string Major { get; private set; }
    public string Minor { get; private set; }
    public string Patch { get; private set; }
    public string SemVersion { get; private set; }
    public string LegacySemVersion { get; private set; }
    public string PreReleaseTag { get; private set; }

    public static WhichCake getVersion(ICakeContext ctx, CheeseCake parameters) 
    {
        if (ctx == null)
        {
            throw new ArgumentNullException("Missing context @ WhichCake");
        }

        string version = null;
        string major = null;
        string minor = null;
        string patch = null;

        string semVersion = null;
        string legacySemVersion = null;
        string preReleaseTag = null; 
        
        if (ctx.IsRunningOnWindows()) 
        {
            ctx.Information("Calculating Semantic Version");
        }

        if (!parameters.IsLocalBuild)
        {
            ctx.GitVersion(new GitVersionSettings{
                UpdateAssemblyInfo = true,
                OutputType = GitVersionOutput.BuildServer
            });
        }

        GitVersion assertedVersions = ctx.GitVersion(new GitVersionSettings
        {
             OutputType = GitVersionOutput.Json,
        });

        version = assertedVersions.MajorMinorPatch;
        major = assertedVersions.Major.ToString();
        minor = assertedVersions.Minor.ToString();
        patch = assertedVersions.Patch.ToString();

        semVersion = assertedVersions.SemVer;
        legacySemVersion = assertedVersions.LegacySemVerPadded;
        preReleaseTag = assertedVersions.PreReleaseTag; 

        ctx.Information("Calculated Semantic Version: {0}", semVersion);

        return new WhichCake() 
        {
            Version = version,
            Major = major,
            Minor = minor,
            Patch = patch,
            SemVersion = semVersion,
            LegacySemVersion = legacySemVersion,
            PreReleaseTag = preReleaseTag
        };
    }

}
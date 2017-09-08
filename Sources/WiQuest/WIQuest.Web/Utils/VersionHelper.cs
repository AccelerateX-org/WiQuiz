using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Reflection;
using System.Web.WebPages;
using Semver;

namespace WIQuest.Web.Utils
{
    public static class VersionHelper
    {
        public static string CurrentVersion(this HtmlHelper helper)
        {
            try
            {
                var fullSemVer = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                var semVer = SemVersion.Parse(fullSemVer);
                if (!semVer.Prerelease.IsEmpty())
                {
                    return $"v{semVer.Major}.{semVer.Minor}.{semVer.Patch}.{semVer.Prerelease}";
                }
                return $"v{semVer.Major}.{semVer.Minor}.{semVer.Patch}";
            }
            catch (Exception)
            {
                return "0.1.0-LocalBuild";
            }
        }
    }
}
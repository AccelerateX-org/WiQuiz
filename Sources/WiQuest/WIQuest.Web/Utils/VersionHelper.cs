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
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;


                /*

                var semVer =
                    SemVersion.Parse("0.2.0-unstable.4+Branch.develop.Sha.9201bcddb426fd4dde7b73f48b6f4d2ab58917f7");
                if (!semVer.Prerelease.IsEmpty())
                {
                    return $"v{semVer.Major}.{semVer.Minor}.{semVer.Patch}.{semVer.Prerelease}";
                }
                return $"v{semVer.Major}.{semVer.Minor}.{semVer.Patch}";*/
            }
            catch (Exception)
            {
                return "0.1.0-LocalBuild";
            }
        }
    }
}
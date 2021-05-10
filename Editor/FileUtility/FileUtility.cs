using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using System.Text;

namespace SuperUnityBuild.BuildActions
{
    public static class FileUtility
    {
        public enum Operation
        {
            Move,
            Copy,
            Delete,
        }

        public static bool ContainsWildcard(string inputPath)
        {
            return !string.IsNullOrEmpty(inputPath) && Path.GetFileNameWithoutExtension(inputPath).Contains("*");
        }

        public static string ResolvePath(string prototype)
        {
            StringBuilder sb = new StringBuilder(prototype);

            sb.Replace("$VERSION", BuildProject.SanitizeFolderName(BuildSettings.productParameters.buildVersion));
            sb.Replace("$BUILD", BuildSettings.productParameters.buildCounter.ToString());

            return sb.ToString();
        }

        public static string ResolvePerBuildPath(string prototype, BuildReleaseType releaseType, BuildPlatform buildPlatform, BuildArchitecture arch, BuildDistribution dist, DateTime buildTime, string buildPath)
        {
            return BuildProject.ResolvePath(
                prototype
                    .Replace("$BUILDPATH", buildPath)
                    .Replace("$BASEPATH", BuildSettings.basicSettings.baseBuildFolder),
                releaseType, buildPlatform, arch, dist, buildTime);
        }
    }
}

using Ionic.Zip;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public class ZipFileOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction
{
    public string inputPath = "$BUILDPATH";
    public string outputPath = "$BUILDPATH";
    public string outputFileName = "$PRODUCT_NAME-$RELEASE_TYPE-$YEAR_$MONTH_$DAY.zip";

    public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref UnityEditor.BuildOptions options, string configKey, string buildPath)
    {
        string resolvedOutputPath = Path.Combine(outputPath.Replace("$BUILDPATH", buildPath), outputFileName);
        resolvedOutputPath = BuildProject.ResolvePath(resolvedOutputPath, releaseType, platform, architecture, distribution, buildTime);

        string resolvedInputPath = inputPath.Replace("$BUILDPATH", buildPath);
        resolvedInputPath = BuildProject.ResolvePath(resolvedInputPath, releaseType, platform, architecture, distribution, buildTime);

        if (!resolvedOutputPath.EndsWith(".zip"))
            resolvedOutputPath += ".zip";

        PerformZip(Path.GetFullPath(resolvedInputPath), Path.GetFullPath(resolvedOutputPath));
    }

    private void PerformZip(string inputPath, string outputPath)
    {
        try
        {
            if (!Directory.Exists(inputPath))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Zip Operation Failed.", string.Format("Input path does not exist: {0}", inputPath),
                    true, null));
                return;
            }

            // Make sure that all parent directories in path are already created.
            string parentPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }

            // Delete old file if it exists.
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            using (ZipFile zip = new ZipFile(outputPath))
            {
                zip.ParallelDeflateThreshold = -1; // Parallel deflate is bugged in DotNetZip, so we need to disable it.
                zip.AddDirectory(inputPath);
                zip.Save();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }
}

}
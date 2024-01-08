using SuperUnityBuild.BuildTool;
using System;
using System.IO;
#if NET_4_6 || NETSTANDARD2_0_OR_GREATER
using System.IO.Compression;
#else
using Ionic.Zip;
#endif

namespace SuperUnityBuild.BuildActions
{

    public class CustomZipFileOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction
    {
        public string inputPath = "$BUILDPATH";
        public string outputPath = "$BUILDPATH";
        public string outputFileName = "$PRODUCT_NAME-$RELEASE_TYPE-$YEAR_$MONTH_$DAY.zip";

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref UnityEditor.BuildOptions options, string configKey, string buildPath)
        {
            string combinedOutputPath = Path.Combine(outputPath, outputFileName);
            string resolvedOutputPath = BuildAction.ResolvePerBuildExecuteTokens(combinedOutputPath, releaseType, platform, architecture, scriptingBackend, distribution, buildTime, buildPath);

            string resolvedInputPath = BuildAction.ResolvePerBuildExecuteTokens(inputPath, releaseType, platform, architecture, scriptingBackend, distribution, buildTime, buildPath);

            if (!resolvedOutputPath.EndsWith(".zip"))
                resolvedOutputPath += ".zip";

            PerformZip(Path.GetFullPath(resolvedInputPath), Path.GetFullPath(resolvedOutputPath), platform);
        }
        private void PerformZip(string inputPath, string outputPath, BuildPlatform platform)
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
#if NET_4_6 || NETSTANDARD2_0_OR_GREATER
        System.IO.Compression.ZipFile.CreateFromDirectory(inputPath, outputPath);
        if (platform.platformName == "macOS")
        {
          var zipArchive = System.IO.Compression.ZipFile.Open(outputPath, ZipArchiveMode.Update);
          foreach (var entry in zipArchive.Entries)
          {
            if (entry.FullName.Contains("Contents/MacOS"))
              entry.ExternalAttributes = 0755;
          }
          zipArchive.Dispose();
        }
#else
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(outputPath))
                {
                    zip.ParallelDeflateThreshold = -1; // Parallel deflate is bugged in DotNetZip, so we need to disable it.
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    zip.AddDirectory(inputPath);
                    zip.Save();
                }
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }
}

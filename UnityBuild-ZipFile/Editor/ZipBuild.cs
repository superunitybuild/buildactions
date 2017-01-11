using Ionic.Zip;
using System.IO;

namespace SuperSystems.UnityBuild
{

public class ZipFileOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction
{
    public string inputPath = "$BUILDPATH";
    public string outputPath = "$BUILDPATH";
    public string outputFileName = "$PRODUCT_NAME-$RELEASE_TYPE-$YEAR_$MONTH_$DAY.zip";

    public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref UnityEditor.BuildOptions options, string configKey, string buildPath)
    {
        string resolvedOutputPath = Path.Combine(inputPath.Replace("$BUILDPATH", buildPath), outputFileName);
        resolvedOutputPath = BuildProject.ResolvePath(resolvedOutputPath, releaseType, platform, architecture, distribution, buildTime);

        string resolvedInputPath = inputPath.Replace("$BUILDPATH", buildPath);
        resolvedInputPath = BuildProject.ResolvePath(resolvedInputPath, releaseType, platform, architecture, distribution, buildTime);

        if (!resolvedOutputPath.EndsWith(".zip"))
            resolvedOutputPath += ".zip";

        PerformZip(resolvedInputPath, resolvedOutputPath);
    }

    private void PerformZip(string inputPath, string outputPath)
    {
        try
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            using (ZipFile zip = new ZipFile(outputPath))
            {
                zip.ParallelDeflateThreshold = -1; // Parallel deflate is bugged in DotNetZip, so we need to disable it.
                zip.AddDirectory(inputPath);
                zip.Save();
            }
        }
        catch
        {
            // TODO: Log error.
        }
    }
}

}
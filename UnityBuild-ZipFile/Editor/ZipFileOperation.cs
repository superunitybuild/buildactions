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
        string resolvedOutputPath = BuildProject.ResolvePath(outputPath, releaseType, platform, architecture, distribution, buildTime);

        string resolvedOutputFileName = BuildProject.ResolvePath(outputFileName, releaseType, platform, architecture, distribution, buildTime) + ".zip";

        string resolvedInputPath = inputPath.Replace("$BUILDPATH", buildPath);
        resolvedInputPath = BuildProject.ResolvePath(resolvedInputPath, releaseType, platform, architecture, distribution, buildTime);

        PerformZip(resolvedInputPath, resolvedOutputPath, resolvedOutputFileName);
    }

    private void PerformZip(string inputPath, string outputPath, string filename)
    {
        try
        {
            if (File.Exists(outputPath + filename))
            {
                File.Delete(outputPath + filename);
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            using (ZipFile zip = new ZipFile(inputPath + ".zip"))
            {
                zip.ParallelDeflateThreshold = -1; // Parallel deflate is bugged in DotNetZip, so we need to disable it.
                zip.AddDirectory(inputPath);
                zip.Save();
            }
            FileUtil.MoveFileOrDirectory(inputPath + ".zip", outputPath + filename);
        }
            catch
        {
            // TODO: Log error.
        }
    }
}

}
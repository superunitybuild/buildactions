using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public sealed class BuildAssetBundles : PostBuildAction
{
    #region Public Variables

    public string buildPath = Path.Combine("bin", "bundles");

    #endregion

    #region Public Methods

    public override void Execute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution)
    {
        Build(platform);
    }

    public void BuildAll()
    {
        //for (int i = 0; i < BuildProject.platforms.Count; i++)
        //{
        //    BuildPlatform platform = BuildProject.platforms[i];
        //    Build(platform);
        //}
    }

    #endregion

    #region Private Methods

    private void Build(BuildPlatform platform)
    {
        //if (!platform.buildEnabled)
        //    return;

        //// Path where this platform's AssetBundles will be built.
        //string platformBundlePath = BuildAssetBundlesSettings.buildPath + Path.DirectorySeparatorChar + platform.name;

        //// Create build destination directory if it does not exist.
        //if (!Directory.Exists(platformBundlePath))
        //    Directory.CreateDirectory(platformBundlePath);

        //// Build AssetBundles.
        //BuildPipeline.BuildAssetBundles(platformBundlePath, BuildAssetBundleOptions.None, platform.target);

        //// Copy AssetBundles to platform's data directory.
        //if (BuildAssetBundlesSettings.copyToBuild && Directory.Exists(platformBundlePath))
        //{
        //    string bundleDirectory = platform.dataDirectory + "Bundles" + Path.DirectorySeparatorChar;
        //    string targetPath = bundleDirectory + platform.name;

        //    if (Directory.Exists(bundleDirectory))
        //        FileUtil.DeleteFileOrDirectory(bundleDirectory);

        //    Directory.CreateDirectory(bundleDirectory);
        //    FileUtil.CopyFileOrDirectory(platformBundlePath + Path.DirectorySeparatorChar, targetPath);
        //}

        string platformBundlePath = Path.Combine(buildPath, platform.platformName);
    }

    #endregion
}

}
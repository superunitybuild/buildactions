using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public sealed class BuildAssetBundles : PreBuildAction
{
    #region Public Variables

    [FilePath(true, true, "Select AssetBundle output directory.")]
    public string buildPath = Path.Combine("bin", "bundles");

    public bool chunkBased = true;
    public bool uncompressed = false;
    public bool disableWriteTypeTree = false;
    public bool ignoreTypeTreeChanges = false;
    public bool forceRebuild = false;
    public bool appendHash = false;
    public bool strictMode = false;

    #endregion

    #region Public Methods

    public override void PerBuildExecute(
        BuildReleaseType releaseType,
        BuildPlatform platform,
        BuildArchitecture architecture,
        BuildDistribution distribution,
        System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        Build(platform, architecture);
    }

    public void BuildAll()
    {
        string[] buildConfigs = BuildSettings.projectConfigurations.BuildAllKeychains();

        for (int i = 0; i < buildConfigs.Length; i++)
        {
            BuildReleaseType releaseType;
            BuildPlatform platform;
            BuildArchitecture arch;
            BuildDistribution dist;
            string configKey = buildConfigs[i];

            BuildSettings.projectConfigurations.ParseKeychain(configKey, out releaseType, out platform, out arch, out dist);
            Build(platform, arch);
        }
    }

    protected override void DrawProperties(SerializedObject obj)
    {
        base.DrawProperties(obj);

        if (GUILayout.Button("Run Now", GUILayout.ExpandWidth(true)))
        {
            BuildAll();
        }
    }

    #endregion

    #region Private Methods

    private void Build(BuildPlatform platform, BuildArchitecture arch)
    {
        if (!platform.enabled || !arch.enabled)
            return;

        // Create options mask.
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        
        if (uncompressed)
            options |= BuildAssetBundleOptions.UncompressedAssetBundle;
        if (disableWriteTypeTree)
            options |= BuildAssetBundleOptions.DisableWriteTypeTree;
        if (forceRebuild)
            options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
        if (ignoreTypeTreeChanges)
            options |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
        if (appendHash)
            options |= BuildAssetBundleOptions.AppendHashToAssetBundleName;
        if (chunkBased)
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
        if (strictMode)
            options |= BuildAssetBundleOptions.StrictMode;
        
        string platformBundlePath = Path.Combine(buildPath, Path.Combine(platform.platformName, arch.name));

        // Create build destination directory if it does not exist.
        if (!Directory.Exists(platformBundlePath))
            Directory.CreateDirectory(platformBundlePath);

        // Build AssetBundles.
        BuildPipeline.BuildAssetBundles(platformBundlePath, options, arch.target);
    }

    #endregion
}

}
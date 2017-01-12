using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

public sealed class BuildAssetBundles : BuildAction, IPreBuildPerPlatformAction
{
    #region Public Variables

    [FilePath(true, true, "Select AssetBundle output directory.")]
    public string baseBuildPath = Path.Combine("bin", "bundles");

    public string innerBuildPath = Path.Combine("$PLATFORM", "$ARCHITECTURE");

#if UNITY_5_3 || UNITY_5_4_OR_NEWER
    public BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;
#else
    public BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
#endif

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
        EditorGUILayout.PropertyField(obj.FindProperty("baseBuildPath"));
        EditorGUILayout.PropertyField(obj.FindProperty("innerBuildPath"));
        options = (BuildAssetBundleOptions)((int)(BuildAssetBundleOptions)EditorGUILayout.EnumMaskField("Options", (BuildAssetBundleOptions)((int)(options) << 1)) >> 1);
        //options = (BuildAssetBundleOptions)EditorGUILayout.EnumMaskField("Options", options);

        //int i = (int)options;
        //Debug.Log(System.Convert.ToString(i, 2) + ", " + i.ToString());

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

        // Resolve build path.
        StringBuilder platformBundlePath = new StringBuilder(Path.Combine(baseBuildPath, innerBuildPath));
        platformBundlePath.Replace("$PLATFORM", BuildProject.SanitizeFolderName(platform.platformName));
        platformBundlePath.Replace("$ARCHITECTURE", BuildProject.SanitizeFolderName(arch.name));

        // Create build destination directory if it does not exist.
        if (!Directory.Exists(platformBundlePath.ToString()))
            Directory.CreateDirectory(platformBundlePath.ToString());

        // Build AssetBundles.
        BuildPipeline.BuildAssetBundles(platformBundlePath.ToString(), options, arch.target);
    }

    #endregion
}

}
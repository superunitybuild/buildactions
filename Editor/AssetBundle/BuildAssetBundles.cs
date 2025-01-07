using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public sealed class BuildAssetBundles : BuildAction, IPreBuildPerPlatformAction
    {
        #region Public Variables

        [BuildTool.FilePath(true, true, "Select AssetBundle output directory.")]
        public string baseBuildPath = Path.Combine("bin", "bundles");

        public string innerBuildPath = Path.Combine("$PLATFORM", "$TARGET");

        public BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;

        #endregion

        #region Public Methods

        public override void PerBuildExecute(
            BuildReleaseType releaseType,
            BuildPlatform platform,
            BuildTool.BuildTarget target,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            Build(platform, target);
        }

        public void BuildAll()
        {
            string[] buildConfigs = BuildSettings.projectConfigurations.BuildAllKeychains();

            for (int i = 0; i < buildConfigs.Length; i++)
            {
                string configKey = buildConfigs[i];
                _ = BuildSettings.projectConfigurations.ParseKeychain(configKey, out _, out BuildPlatform platform, out BuildTool.BuildTarget target, out _, out _);
                Build(platform, target);
            }
        }

        protected override void DrawProperties(SerializedObject obj)
        {
            _ = EditorGUILayout.PropertyField(obj.FindProperty("baseBuildPath"));
            _ = EditorGUILayout.PropertyField(obj.FindProperty("innerBuildPath"));
            options = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Options", options);

            if (GUILayout.Button("Run Now", GUILayout.ExpandWidth(true)))
                BuildAll();
        }

        #endregion

        #region Private Methods

        private void Build(BuildPlatform platform, BuildTool.BuildTarget target)
        {
            if (!platform.enabled || !target.enabled)
                return;

            // Resolve build path.
            string platformBundlePath = TokensUtility.ResolveBuildConfigurationTokens(
                Path.Combine(baseBuildPath, innerBuildPath),
                null, platform, target, null, null, null
            );

            // Create build destination directory if it does not exist.
            if (!Directory.Exists(platformBundlePath))
                _ = Directory.CreateDirectory(platformBundlePath);

            // Build AssetBundles.
            _ = BuildPipeline.BuildAssetBundles(platformBundlePath, options, target.type);
        }

        #endregion
    }
}

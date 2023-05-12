using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public sealed class BuildAssetBundles : BuildAction, IPreBuildPerPlatformAction
    {
        #region Public Variables

        [BuildTool.FilePath(true, true, "Select AssetBundle output directory.")]
        public string baseBuildPath = Path.Combine("bin", "bundles");

        public string innerBuildPath = Path.Combine("$PLATFORM", "$ARCHITECTURE");

        public BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;

        #endregion

        #region Public Methods

        public override void PerBuildExecute(
            BuildReleaseType releaseType,
            BuildPlatform platform,
            BuildArchitecture architecture,
            BuildScriptingBackend scriptingBackend,
            BuildDistribution distribution,
            DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
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
                BuildArchitecture architecture;
                BuildDistribution distribution;
                BuildScriptingBackend scriptingBackend;
                string configKey = buildConfigs[i];

                BuildSettings.projectConfigurations.ParseKeychain(configKey, out releaseType, out platform, out architecture, out scriptingBackend, out distribution);
                Build(platform, architecture);
            }
        }

        protected override void DrawProperties(SerializedObject obj)
        {
            EditorGUILayout.PropertyField(obj.FindProperty("baseBuildPath"));
            EditorGUILayout.PropertyField(obj.FindProperty("innerBuildPath"));
            options = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("Options", options);

            if (GUILayout.Button("Run Now", GUILayout.ExpandWidth(true)))
                BuildAll();
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

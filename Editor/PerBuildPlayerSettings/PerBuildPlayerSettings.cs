using SuperUnityBuild.BuildTool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SuperUnityBuild.BuildActions
{
    public class PerBuildPlayerSettings : BuildAction, IPreBuildPerPlatformAction, IPostBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        [Header("Resolution")]
        public FullScreenMode fullScreenMode;
        public bool defaultIsNativeResolution;
        public int defaultScreenWidth = 800;
        public int defaultScreenHeight = 600;
        public bool runInBackground;
        [Header("Standalone Player Options")]
        public bool resizableWindow;
        public bool visibleInBackground = true;
        public bool allowFullscreenSwitch;

        [Header("Other Settings")]
        [Tooltip("Preloaded assets to set for this build")] public List<Object> PreloadedAssets = new();
        [Tooltip("Whether or not to preserve existing preloaded assets")] public bool PreservePreloadedAssets = true;

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            // Set resolution & presentation config
            PlayerSettings.fullScreenMode = fullScreenMode;
            PlayerSettings.defaultIsNativeResolution = defaultIsNativeResolution;
            PlayerSettings.defaultScreenHeight = defaultScreenHeight;
            PlayerSettings.defaultScreenWidth = defaultScreenWidth;
            PlayerSettings.runInBackground = runInBackground;
            PlayerSettings.resizableWindow = resizableWindow;
            PlayerSettings.allowFullscreenSwitch = allowFullscreenSwitch;
            PlayerSettings.visibleInBackground = visibleInBackground;

            // Set preloaded assets
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            List<Object> preloadedAssetsList = PreservePreloadedAssets ?
                preloadedAssets.ToList() :
                new List<Object>();

            preloadedAssetsList.AddRange(PreloadedAssets);
            PlayerSettings.SetPreloadedAssets(preloadedAssetsList.ToArray());
        }
    }
}

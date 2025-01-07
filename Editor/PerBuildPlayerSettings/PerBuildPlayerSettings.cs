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
        [Header("Other Settings")]
        [Tooltip("Preloaded assets to set for this build")] public List<Object> PreloadedAssets = new();
        [Tooltip("Whether or not to preserve existing preloaded assets")] public bool PreservePreloadedAssets = true;

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
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

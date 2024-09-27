#if UNITY_XR_MANAGEMENT
using SuperUnityBuild.BuildTool;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.Management;

namespace SuperUnityBuild.BuildActions
{
    public class XRPluginManagement : BuildAction, IPreBuildPerPlatformAction, IPostBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        [Header("XR Settings")]
        [Tooltip("XR plugin loaders to use for this build")] public List<XRLoader> XRPlugins = new();
        [Tooltip("Whether or not to use automatic initialization of XR plugin loaders on startup")] public bool InitializeXROnStartup = true;

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            XRGeneralSettings generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(platform.targetGroup);
            XRManagerSettings settingsManager = generalSettings.Manager;

            generalSettings.InitManagerOnStart = InitializeXROnStartup;

#if UNITY_XR_MANAGEMENT_400_OR_NEWER
            settingsManager.TrySetLoaders(XRPlugins);
#else
            settingsManager.loaders = XRPlugins;
#endif
        }
    }
}
#endif

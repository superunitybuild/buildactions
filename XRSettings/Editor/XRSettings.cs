#if UNITY_2017_4 || UNITY_2018_4 || UNITY_2019_2_OR_NEWER
#define SUPPORTS_OCULUS_V2_SIGNING
#endif

#if !UNITY_2020_2_OR_NEWER
using System;
using System.Collections.Generic;
using SuperUnityBuild.BuildTool;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public class XRSettings : BuildAction, IPreBuildPerPlatformAction, IPostBuildPerPlatformAction
    {
        public enum SDK
        {
            None,
            Cardboard,
            Daydream,
            MockHMD,
            Oculus,
            OpenVR,
            StereoDisplay,
        }

        // Mapping of SDKs enum to Unity's inconsistent internal names
        readonly Dictionary<SDK, string> sdkNames = new Dictionary<SDK, string>()
        {
            {SDK.None, "None"},
            {SDK.Cardboard, "cardboard"},
            {SDK.Daydream, "daydream"},
            {SDK.MockHMD, "MockHMD"},
            {SDK.Oculus, "Oculus"},
            {SDK.OpenVR, "OpenVR"},
            {SDK.StereoDisplay, "stereo"}
        };

        [Header("XR Settings")]
        [Tooltip("VR SDKs for build target, in order of priority")]
        public List<SDK> Sdks = new List<SDK>();
        [Tooltip("Whether or not build target supports VR")]
        public bool VRSupported;
        [Tooltip("Stereo rendering mode to use")]
        public StereoRenderingPath StereoRenderingMode;

#if SUPPORTS_OCULUS_V2_SIGNING
        [Header("SDK-Specific Settings")]
        [Tooltip("Oculus: Whether or not to use v2 APK signing (enable for Quest, disable for Gear VR/Go)")]
        public bool OculusV2Signing = true;
#endif

        [Header("Platform-Specific Settings")]
        [Tooltip("Android: Whether or not build target supports ARCore")]
        public bool ARCoreSupported;

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            // Get valid SDKs for platform
            string[] validSDKs = PlayerSettings.GetAvailableVirtualRealitySDKs(platform.targetGroup);

            // Build list of valid SDKs to set
            List<string> sdks = new List<string>();

            foreach (SDK sdk in Sdks)
            {
                string sdkName = sdkNames[sdk];

                if (Array.IndexOf(validSDKs, sdkName) != -1)
                {
                    sdks.Add(sdkName);
                }
            }

            // Update player settings
            PlayerSettings.SetVirtualRealitySupported(platform.targetGroup, VRSupported);
            PlayerSettings.SetVirtualRealitySDKs(platform.targetGroup, sdks.ToArray());
            PlayerSettings.stereoRenderingPath = StereoRenderingMode;

#if SUPPORTS_OCULUS_V2_SIGNING
            PlayerSettings.VROculus.v2Signing = OculusV2Signing;
#endif

            if (platform.targetGroup == BuildTargetGroup.Android)
            {
                PlayerSettings.Android.ARCoreEnabled = ARCoreSupported;
            }
        }
    }
}
#endif
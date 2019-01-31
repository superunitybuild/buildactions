using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XRSettings : BuildAction, IPreBuildPerPlatformAction
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

        [Header("Platform-Specific Settings")]
        [Tooltip("Whether or not build target supports ARCore (Android only)")]
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

            if (platform.targetGroup == BuildTargetGroup.Android)
            {
                PlayerSettings.Android.ARCoreEnabled = ARCoreSupported;
            }
        }
    }

}

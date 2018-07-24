using System;
using SuperSystems.UnityBuild;
using UnityEditor;

namespace DefaultNamespace
{
    public class UpdateIOSBuildNumber : BuildAction, IPreBuildPerPlatformAction
    {
        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture,
            BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            if (platform.targetGroup == BuildTargetGroup.iOS)
            {
                PlayerSettings.iOS.buildNumber = BuildSettings.productParameters.buildCounter.ToString();
            }
        }
    }
}
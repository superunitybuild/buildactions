using SuperUnityBuild.BuildTool;
using System;
using System.Text;
using UnityEditor;

namespace SuperUnityBuild.BuildActions
{
    public class OverrideDefines : BuildAction, IPreBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        public string removeDefines;
        public string addDefines;

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
            string defaultDefines = BuildProject.GenerateDefaultDefines(releaseType, platform, target, scriptingBackend, distribution);

            StringBuilder mergedDefines = new(BuildProject.MergeDefines(preBuildDefines, defaultDefines));

            if (!string.IsNullOrEmpty(removeDefines))
            {
                string resolvedRemove = TokensUtility.ResolveBuildConfigurationTokens(removeDefines, releaseType, platform, target, scriptingBackend, distribution, buildTime);
                string[] splitRemove = resolvedRemove.Split(';');

                for (int i = 0; i < splitRemove.Length; i++)
                {
                    _ = mergedDefines.Replace(splitRemove[i] + ";", "");
                    _ = mergedDefines.Replace(splitRemove[i], "");
                }
            }

            if (!string.IsNullOrEmpty(addDefines))
            {
                string resolvedAdd = TokensUtility.ResolveBuildConfigurationTokens(addDefines, releaseType, platform, target, scriptingBackend, distribution, buildTime);

                if (mergedDefines.Length > 0)
                    _ = mergedDefines.Append(";");

                _ = mergedDefines.Append(resolvedAdd);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, mergedDefines.ToString());
        }
    }
}

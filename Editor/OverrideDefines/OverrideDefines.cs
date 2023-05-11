using SuperUnityBuild.BuildTool;
using System.Collections;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public class OverrideDefines : BuildAction, IPreBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        public string removeDefines;
        public string addDefines;

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string preBuildDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform.targetGroup);
            string defaultDefines = BuildProject.GenerateDefaultDefines(releaseType, platform, architecture, scriptingBackend, distribution);

            StringBuilder mergedDefines = new StringBuilder(BuildProject.MergeDefines(preBuildDefines, defaultDefines));

            if (!string.IsNullOrEmpty(removeDefines))
            {
                string resolvedRemove = BuildProject.ResolvePath(removeDefines, releaseType, platform, architecture, scriptingBackend, distribution, buildTime);
                string[] splitRemove = resolvedRemove.Split(';');

                for (int i = 0; i < splitRemove.Length; i++)
                {
                    mergedDefines.Replace(splitRemove[i] + ";", "");
                    mergedDefines.Replace(splitRemove[i], "");
                }
            }

            if (!string.IsNullOrEmpty(addDefines))
            {
                string resolvedAdd = BuildProject.ResolvePath(addDefines, releaseType, platform, architecture, scriptingBackend, distribution, buildTime);

                if (mergedDefines.Length > 0)
                    mergedDefines.Append(";");

                mergedDefines.Append(resolvedAdd);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, mergedDefines.ToString());
        }
    }
}

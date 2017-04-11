using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;

namespace SuperSystems.UnityBuild
{

public class OverrideDefines : BuildAction, IPreBuildPerPlatformAction
{
    public string removeDefines;
    public string addDefines;

    public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        StringBuilder defines = new StringBuilder(BuildProject.GenerateDefaultDefines(releaseType, platform, architecture, distribution));
        
        if (!string.IsNullOrEmpty(removeDefines))
        {
            string resolvedRemove = BuildProject.ResolvePath(removeDefines, releaseType, platform, architecture, distribution, buildTime);
            string[] splitRemove = resolvedRemove.Split(';');

            for (int i = 0; i < splitRemove.Length; i++)
            {
                defines.Replace(splitRemove[i] + ";", "");
                defines.Replace(splitRemove[i], "");
            }
        }

        if (!string.IsNullOrEmpty(addDefines))
        {
            string resolvedAdd = BuildProject.ResolvePath(addDefines, releaseType, platform, architecture, distribution, buildTime);

            if (defines.Length > 0)
                defines.Append(";" + resolvedAdd);
            else
                defines.Append(resolvedAdd);
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(platform.targetGroup, defines.ToString());
    }
}

}

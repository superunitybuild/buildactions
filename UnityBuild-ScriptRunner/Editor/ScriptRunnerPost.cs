using UnityEngine;
using UnityEditor;

namespace SuperSystems.UnityBuild
{

public class ScriptRunnerPost : PostBuildAction
{
    [FilePath(false, true, "Select program/script to run.")]
    public string scriptPath = "";
    public string scriptArguments = "";
    public bool perBuild = false;

    public override void Execute()
    {
        if (!perBuild)
        {
            ScriptRunnerUtility.RunScript(scriptPath, scriptArguments);
        }
    }

    public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
    {
        if (perBuild)
        {
            string resolvedScriptPath = BuildProject.GenerateBuildPath(scriptPath, releaseType, platform, architecture, distribution, buildTime);
            string resolvedScriptArgs = BuildProject.GenerateBuildPath(scriptArguments, releaseType, platform, architecture, distribution, buildTime);

            ScriptRunnerUtility.RunScript(resolvedScriptPath, resolvedScriptArgs);
        }
    }
}

}

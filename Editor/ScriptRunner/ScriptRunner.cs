using SuperUnityBuild.BuildTool;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildActions
{
    public class ScriptRunner : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        [BuildTool.FilePath(false, true, "Select program/script to run.")]
        public string scriptPath = "";
        public string scriptArguments = "";

        public override void Execute()
        {
            string resolvedScriptPath = BuildAction.ResolveExecuteTokens(scriptPath);
            string resolvedScriptArguments = BuildAction.ResolveExecuteTokens(scriptArguments);

            RunScript(resolvedScriptPath, resolvedScriptArguments);
        }

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string resolvedScriptPath = BuildAction.ResolvePerBuildExecuteTokens(scriptPath, releaseType, platform, architecture, scriptingBackend, distribution, buildTime, buildPath);
            string resolvedScriptArguments = BuildAction.ResolvePerBuildExecuteTokens(scriptArguments, releaseType, platform, architecture, scriptingBackend, distribution, buildTime, buildPath);

            RunScript(resolvedScriptPath, resolvedScriptArguments);
        }

        private void RunScript(string scriptPath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.GetFullPath(scriptPath);

            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            Process proc = Process.Start(startInfo);
            proc.WaitForExit();
        }
    }
}

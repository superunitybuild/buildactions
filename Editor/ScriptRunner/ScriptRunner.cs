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
            string resolvedScriptPath = ResolveExecuteTokens(scriptPath);
            string resolvedScriptArguments = ResolveExecuteTokens(scriptArguments);

            RunScript(resolvedScriptPath, resolvedScriptArguments);
        }

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string resolvedScriptPath = ResolvePerBuildExecuteTokens(scriptPath, releaseType, platform, target, scriptingBackend, distribution, buildTime, buildPath);
            string resolvedScriptArguments = ResolvePerBuildExecuteTokens(scriptArguments, releaseType, platform, target, scriptingBackend, distribution, buildTime, buildPath);

            RunScript(resolvedScriptPath, resolvedScriptArguments);
        }

        private void RunScript(string scriptPath, string arguments)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = Path.GetFullPath(scriptPath)
            };

            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            Process proc = Process.Start(startInfo);
            proc.WaitForExit();
        }
    }
}

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
            RunScript(scriptPath, scriptArguments);
        }

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string resolvedScriptPath = BuildProject.ResolvePath(scriptPath, releaseType, platform, architecture, scriptingBackend, distribution, buildTime);
            string resolvedScriptArgs = BuildProject.ResolvePath(scriptArguments, releaseType, platform, architecture, scriptingBackend, distribution, buildTime);

            RunScript(resolvedScriptPath, resolvedScriptArgs);
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

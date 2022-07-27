using SuperUnityBuild.BuildTool;
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

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string _scriptPath = (string)scriptPath.Clone();
            _scriptPath = _scriptPath.Replace("$BUILDPATH", buildPath);
            _scriptPath = _scriptPath.Replace("$BASEPATH", BuildSettings.basicSettings.baseBuildFolder);
            string resolvedScriptPath = BuildProject.ResolvePath(_scriptPath, releaseType, platform, architecture, distribution, buildTime);

            string _scriptArguments = (string)scriptArguments.Clone();
            _scriptArguments = _scriptArguments.Replace("$BUILDPATH", buildPath);
            _scriptArguments = _scriptArguments.Replace("$BASEPATH", BuildSettings.basicSettings.baseBuildFolder);
            string resolvedScriptArgs = BuildProject.ResolvePath(_scriptArguments, releaseType, platform, architecture, distribution, buildTime);

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

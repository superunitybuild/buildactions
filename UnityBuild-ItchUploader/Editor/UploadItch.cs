using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SuperSystems.UnityBuild
{

    public class UploadItch : BuildAction, IPostBuildPerPlatformAction
    {
        private const string WINDOWS = "windows";
        private const string OSX = "osx";
        private const string LINUX = "linux";

        [FilePath(false, true, "Path to butler.exe")]
        public string pathToButlerExe = "";
        public string nameOfItchUser = "";
        public string nameOfItchGame = "";

        [Header("The following field overrides default channel name (the per-build architecture target), and will be applied to all builds. Use with care.")]
        public string itchChannelOverride = "";

        #region Public Methods

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            if (!File.Exists(pathToButlerExe))
            {
                UnityEngine.Debug.LogError("Couldn't find butler.exe file at path \"" + pathToButlerExe + "\", please check provided path");
                return;
            }

            buildPath = Path.GetFullPath(buildPath);

            StringBuilder scriptArguments = new StringBuilder("push ");

            switch (architecture.target)
            {
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    scriptArguments.Append("--fix-permissions ");
                    break;
            }

            scriptArguments.Append("\"" + buildPath + "\"" + " " + nameOfItchUser + "/" + nameOfItchGame + ":");

            if (!string.IsNullOrEmpty(itchChannelOverride))
            {
                scriptArguments.Append(itchChannelOverride);
            }
            else
            {
                string itchChannel = GetChannelName(architecture.target);
                if(string.IsNullOrEmpty(itchChannel))
                {
                    UnityEngine.Debug.LogWarning("UploadItch: The current BuildTarget doesn't appear to be a standard Itch.IO build target.");
                }

                scriptArguments.Append(itchChannel);
            }

            //UnityEngine.Debug.Log("Would have run itch uploader with following command line: \"" + pathToButlerExe + " " + scriptArguments + "\"");
            RunScript(pathToButlerExe, scriptArguments.ToString());
        }

        #endregion

        #region Private Methods

        private void RunScript(string scriptPath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.GetFullPath(scriptPath);
            startInfo.UseShellExecute = true;
            startInfo.CreateNoWindow = false;

            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            Process proc = Process.Start(startInfo);
            proc.WaitForExit();
        }

        private static string GetChannelName(BuildTarget target)
        {
            switch (target)
            {
                // Windows
                case BuildTarget.StandaloneWindows:
                    return WINDOWS + "-x86";
                case BuildTarget.StandaloneWindows64:
                    return WINDOWS + "-x64";

                // Linux
                case BuildTarget.StandaloneLinux:
                    return LINUX + "-x86";
                case BuildTarget.StandaloneLinux64:
                    return LINUX + "-x64";
                case BuildTarget.StandaloneLinuxUniversal:
                    return LINUX + "-universal";

                // OSX
                case BuildTarget.StandaloneOSXIntel:
                    return OSX + "-intel";
                case BuildTarget.StandaloneOSXIntel64:
                    return OSX + "-intel64";
                case BuildTarget.StandaloneOSXUniversal:
                    return OSX + "-universal";
            
                default:
                    return null;
            }
        }

        #endregion
    }

}
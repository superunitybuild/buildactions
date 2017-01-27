using UnityEngine;
using UnityEditor;
using System.IO;

namespace SuperSystems.UnityBuild
{

    public class UploadItch : BuildAction, IPostBuildAction, IPostBuildPerPlatformAction
    {
        [FilePath(false, true, "Path to butler.exe")]
        public string pathToButlerExe = "";
        public string nameOfItchUser = "";
        public string nameOfItchGame = "";
        [Header("The following field overrides the default channel name (the per-build architecture target), and will be applied to all builds. Use with care.")]
        public string itchChannelOverride = "";

        #region Public Methods

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            if (!File.Exists(pathToButlerExe))
            {
                UnityEngine.Debug.LogError("Couldn't find butler.exe file at path \"" + pathToButlerExe + "\", please check provided path");
                return;
            }
            string scriptArguments = "";
            if (!string.IsNullOrEmpty(itchChannelOverride))
            {
                scriptArguments = "push " + "\"" + buildPath + "\"" + " " + nameOfItchUser + "/" + nameOfItchGame + ":" + itchChannelOverride;
            }
            else
            {
                string itchChannel = GetChannelName(architecture.target);
                if(string.IsNullOrEmpty(itchChannel))
                {
                    UnityEngine.Debug.LogWarning("UploadItch: The current BuildTarget doesn't appear to be a standard Itch.IO build target.");
                }
                scriptArguments = "push " + "\"" + buildPath + "\"" + " " + nameOfItchUser + "/" + nameOfItchGame + ":" + GetChannelName(architecture.target);
            }

            //UnityEngine.Debug.Log("Would have run itch uploader with following command line: \"" + pathToButlerExe + " " + scriptArguments + "\"");
            RunScript(pathToButlerExe, scriptArguments);
        }

        #endregion

        #region Private Methods
        private const string WINDOWS = "windows";
        private const string OSX = "osx";
        private const string LINUX = "linux";

        private void RunScript(string scriptPath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.GetFullPath(scriptPath);

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
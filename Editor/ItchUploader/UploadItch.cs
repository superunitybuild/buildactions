using SuperUnityBuild.BuildTool;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public class UploadItch : BuildAction, IPostBuildPerPlatformAction
    {
        private const string WINDOWS = "windows";
        private const string OSX = "mac";
        private const string LINUX = "linux";
        private const string ANDROID = "android";
        private const string WEBGL = "webgl";
        private const string UWP = "uwp";

        private const string ARCHITECTURE_X86 = "x86";
        private const string ARCHITECTURE_X64 = "x64";
        private const string ARCHITECTURE_UNIVERSAL = "universal";

        [BuildTool.FilePath(false, true, "Path to butler.exe")]
        public string pathToButlerExe = "";
        public string nameOfItchUser = "";
        public string nameOfItchGame = "";
        public bool useGeneratedBuildVersion = false;

        [Header("Channel name")]
        public string channelName = "$PLATFORM-$ARCHITECTURE";

        [Header("Disable to capture error output for debugging.")]
        public bool showUploadProgress = true;

        [Header("Use with caution. Override applies to all platforms.")]
        public string itchChannelOverride = "";

        #region Public Methods

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            // Verify that butler executable exists.
            if (!File.Exists(pathToButlerExe))
            {
                UnityEngine.Debug.LogError("Couldn't find butler.exe file at path \"" + pathToButlerExe + "\", please check provided path");
                return;
            }

            buildPath = Path.GetFullPath(buildPath);

            // Generate build args for the form: butler push {optional args} {build path} {itch username}/{itch game}:{channel}
            StringBuilder scriptArguments = new StringBuilder("push ");

            switch (architecture.target)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneLinux64:
                    // Fix exe permissions for Linux/OSX.
                    scriptArguments.Append("--fix-permissions ");
                    break;
            }

            if (useGeneratedBuildVersion)
            {
                // Append generated versions string.
                scriptArguments.Append(string.Format("--userversion \"{0}\" ", BuildSettings.productParameters.buildVersion));
            }

            scriptArguments.Append("\"" + buildPath + "\"" + " " + nameOfItchUser + "/" + nameOfItchGame + ":");

            scriptArguments.Append(!string.IsNullOrEmpty(itchChannelOverride) ?
                itchChannelOverride :
                GetChannelName(channelName, architecture.target, releaseType)
            );

            // UnityEngine.Debug.Log("Would have run itch uploader with following command line: \"" + pathToButlerExe + " " + scriptArguments + "\"");
            RunScript(pathToButlerExe, scriptArguments.ToString());
        }

        #endregion

        #region Private Methods

        private void RunScript(string scriptPath, string arguments)
        {
            // Create and start butler process.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.GetFullPath(scriptPath);
            startInfo.UseShellExecute = showUploadProgress;
            startInfo.CreateNoWindow = !showUploadProgress;
            startInfo.RedirectStandardOutput = !showUploadProgress;
            startInfo.RedirectStandardError = !showUploadProgress;

            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            Process proc = Process.Start(startInfo);

            StringBuilder outputText = new StringBuilder();
            if (!showUploadProgress)
            {
                // Capture stdout.
                proc.OutputDataReceived += (sendingProcess, outputLine) =>
                {
                    outputText.AppendLine(outputLine.Data);
                };

                proc.BeginOutputReadLine();
            }

            // Wait for butler to finish.
            proc.WaitForExit();

            // Display error if one occurred.
            if (proc.ExitCode != 0)
            {
                string errString;
                if (showUploadProgress)
                {
                    errString = "Run w/ ShowUploadProgress disabled to capture debug output to console.";
                }
                else
                {
                    errString = "Check console window for debug output.";
                }

                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Itch Upload Failed.", string.Format("Exit code: {0}\n{1}", proc.ExitCode, errString),
                    true, null));

                UnityEngine.Debug.Log("ITCH.IO BUTLER OUTPUT: " + outputText.ToString());
            }
        }

        private static string GetChannelName(string channelNameFormat, BuildTarget target, BuildReleaseType buildReleaseType)
        {
            string platform = string.Empty;
            string architecture = string.Empty;

            switch (target)
            {
                // Windows
                case BuildTarget.StandaloneWindows:
                    platform = WINDOWS;
                    architecture = ARCHITECTURE_X86;
                    break;
                case BuildTarget.StandaloneWindows64:
                    platform = WINDOWS;
                    architecture = ARCHITECTURE_X64;
                    break;

                // Linux
                case BuildTarget.StandaloneLinux64:
                    platform = LINUX;
                    architecture = ARCHITECTURE_X64;
                    break;

                // OSX
                case BuildTarget.StandaloneOSX:
                    platform = OSX;
                    architecture = ARCHITECTURE_UNIVERSAL;
                    break;

                // Android
                case BuildTarget.Android:
                    platform = ANDROID;
                    architecture = ARCHITECTURE_UNIVERSAL;
                    break;

                // WebGL
                case BuildTarget.WebGL:
                    platform = WEBGL;
                    break;

                // UWP
                case BuildTarget.WSAPlayer:
                    platform = UWP;
                    architecture = EditorUserBuildSettings.wsaArchitecture;
                    break;

                // Other
                default:
                    UnityEngine.Debug.LogWarning("UploadItch: The current BuildTarget doesn't appear to be a standard Itch.IO build target.");
                    platform = target.ToString().ToLower();
                    break;
            }

            return channelNameFormat
                .Replace("$PLATFORM", platform)
                .Replace("$ARCHITECTURE", architecture)
                .Replace("$RELEASE_TYPE", buildReleaseType.typeName)
                .Trim('-');
        }

        #endregion
    }
}

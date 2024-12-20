using SuperUnityBuild.BuildTool;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PasswordFieldAttribute : PropertyAttribute{}
[CustomPropertyDrawer(typeof(PasswordFieldAttribute))]
public class PasswordFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
            property.stringValue = EditorGUI.PasswordField(position, label, property.stringValue);
        else
            EditorGUI.LabelField(position, label.text, "Use PasswordField with strings.");
    }
}

/// <summary>
/// What branch to set as live after the build is uploaded.
/// I recommend adding your own branches here which can be done in the Steamworks partner site.
/// Or use the default branches: Default, Beta, Alpha, Experimental.
/// </summary>
public enum SteamBranch
{
    Default,
    Beta,
    Alpha,
    Experimental
}

namespace SuperUnityBuild.BuildActions
{
    public class UploadSteam : BuildAction, IPostBuildPerPlatformAction
    {
        [Header("Required Settings")]
        public int appId = 1000000;
        public int depotId = 1000001;
        [BuildTool.FilePath(true, true, "Eg. C:\\Program Files (x86)\\Steamworks\\...\\steamcmd.exe")]
        public string contentBuilderPath = "";
        private string steamCmdPath = "";
        private string vdfFilePath = "";
        private string depotFilePath = "";

        [Header("Steam Authentication")]
        public string steamUsername = "";
        [PasswordField] public string steamPassword = "";

        [Header("Description for the build")]
        public string buildDescription = "";

        [Header("Set the live branch, leave empty to disable")]
        public SteamBranch setLiveBranch = SteamBranch.Experimental;

        #region Public Methods

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            UpdatePaths();

            // Verify that steamcmd.exe and VDF file exist.
            if (!File.Exists(steamCmdPath))
            {
                UnityEngine.Debug.LogError("Couldn't find steamcmd.exe file at path \"" + steamCmdPath + "\".");
                return;
            }

            if (!File.Exists(vdfFilePath))
            {
                UnityEngine.Debug.LogWarning("Couldn't find VDF file at path \"" + vdfFilePath + "\", trying to create it...");
                bool success = createStandardVdf(vdfFilePath);
                if (!success)
                {
                    UnityEngine.Debug.LogError("Failed to create VDF file at path \"" + vdfFilePath + "\".");
                    return;
                }
            }

            if (!File.Exists(depotFilePath))
            {
                UnityEngine.Debug.LogWarning("Couldn't find depot VDF file at path \"" + depotFilePath + "\", trying to create it...");
                bool success = createStandardDepotVdf(depotFilePath, buildPath);
                if (!success)
                {
                    UnityEngine.Debug.LogError("Failed to create depot VDF file at path \"" + depotFilePath + "\".");
                    return;
                }
            }

            // Update the VDF file with the description if provided.
            string resolvedBuildDescPath = BuildAction.ResolvePerBuildExecuteTokens(
                buildDescription,
                releaseType,
                platform,
                architecture,
                scriptingBackend,
                distribution,
                buildTime,
                buildPath
            );
            string resolvedLiveBranch = BuildAction.ResolvePerBuildExecuteTokens(
                setLiveBranch.ToString().ToLower(),
                releaseType,
                platform,
                architecture,
                scriptingBackend,
                distribution,
                buildTime,
                buildPath
            );
            UpdateVdfFile(vdfFilePath, resolvedBuildDescPath, resolvedLiveBranch);

            // Generate Steam command-line arguments.
            StringBuilder scriptArguments = new StringBuilder();
            scriptArguments.Append($"+login {steamUsername} {steamPassword} ");
            scriptArguments.Append($"+run_app_build_http \"{Path.GetFullPath(vdfFilePath)}\" ");
            scriptArguments.Append("+quit");

            RunScript(steamCmdPath, scriptArguments.ToString());
        }

        #endregion

        #region Private Methods
        private void UpdatePaths()
        {
            steamCmdPath = Path.Combine(contentBuilderPath, "builder", "steamcmd.exe");
            vdfFilePath = Path.Combine(contentBuilderPath, "scripts", $"app_{appId}.vdf");
            depotFilePath = Path.Combine(contentBuilderPath, "scripts", $"depot_{depotId}.vdf");
        }

        private void RunScript(string scriptPath, string arguments)
        {
            // Create and start steamcmd process.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.GetFullPath(scriptPath);
            startInfo.UseShellExecute = true;

            if (!string.IsNullOrEmpty(arguments))
                startInfo.Arguments = arguments;

            // Capture stdout.
            Process proc = Process.Start(startInfo);
            proc.WaitForExit();

            // Display error if one occurred.
            if (proc.ExitCode != 0)
            {
                string errString;
                if (proc.StandardError != null)
                    errString = proc.StandardError.ReadToEnd();
                else
                    errString = "No error output was captured.";

                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Steam Upload Failed.", string.Format("Exit code: {0}\n{1}", proc.ExitCode, errString),
                    true, null));
            }
        }

        private void UpdateVdfFile(string vdfFilePath, string description, string liveBranch)
        {
            string vdfContent = File.ReadAllText(vdfFilePath);

            // Check if "desc" field exists and update it, otherwise add it.
            if (vdfContent.Contains("\"desc\""))
            {
                vdfContent = System.Text.RegularExpressions.Regex.Replace(
                    vdfContent,
                    "\"desc\"\\s*\"[^\"]*\"",
                    $"\"desc\" \"{description}\"");
            }
            else
            {
                // Insert "desc" field after the "appid" field.
                int appidIndex = vdfContent.IndexOf("\"appid\"");
                int insertPosition = vdfContent.IndexOf('\n', appidIndex) + 1;
                vdfContent = vdfContent.Insert(insertPosition, $"\t\"desc\" \"{description}\"\n");
            }

            // Check if "setlive" field exists and update it, otherwise add it.
            if (vdfContent.Contains("\"setlive\""))
            {
                vdfContent = System.Text.RegularExpressions.Regex.Replace(
                    vdfContent,
                    "\"setlive\"\\s*\"[^\"]*\"",
                    $"\"setlive\" \"{liveBranch}\"");
            }
            else
            {
                // Insert "setlive" field after the "desc" field.
                int descIndex = vdfContent.IndexOf("\"desc\"");
                int insertPosition = vdfContent.IndexOf('\n', descIndex) + 1;
                vdfContent = vdfContent.Insert(insertPosition, $"\t\"setlive\" \"{liveBranch}\"\n");
            }

            File.WriteAllText(vdfFilePath, vdfContent);
        }
        
        private bool createStandardVdf(string vdfFilePath)
        {
            string vdfContent = $"\"appbuild\"\n" +
                $"{{\n" +
                $"\t\"appid\" \"{appId}\"\n" +
                $"\t\"desc\" \"\"\n" +
                $"\t\"buildoutput\" \"{Path.Combine(contentBuilderPath, "output")}\"\n" +
                $"\t\"contentroot\" \"\"\n" +
                $"\t\"setlive\" \"\"\n" +
                $"\t\"preview\" \"0\"\n" +
                $"\t\"local\" \"\"\n" +
                $"\t\"depots\" \n" +
                    $"\t{{\n" +
                    $"\t\t\"{depotId}\" \"{Path.GetFullPath(depotFilePath)}\"\n" +
                    $"\t}}\n" +
                $"}}";

            try
            {
                File.WriteAllText(vdfFilePath, vdfContent);
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Failed to create VDF file at path \"" + vdfFilePath + "\". " + e.Message);
                return false;
            }
        }

        private bool createStandardDepotVdf(string depotFilePath, string contentRoot)
        {
            string depotContent = $"\"DepotBuildConfig\"\n" +
                $"{{\n" +
                $"\t\"DepotID\" \"{depotId}\"\n" +
                $"\t\"ContentRoot\" \"{contentRoot}\"\n" +
                $"\t\"FileMapping\"\n" +
                $"\t{{\n" +
                $"\t\t\"LocalPath\" \"*\"\n" +
                $"\t\t\"DepotPath\" \".\"\n" +
                $"\t\t\"recursive\" \"1\"\n" +
                $"\t}}\n" +
                $"\t\"FileExclusion\" \"*.pdb\"\n" +
                $"}}";

            try
            {
                File.WriteAllText(depotFilePath, depotContent);
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Failed to create depot VDF file at path \"" + depotFilePath + "\". " + e.Message);
                return false;
            }
        }
        #endregion
    }
}

using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildActions
{
    using Operation = FileUtility.Operation;

    public class FileOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        [BuildTool.FilePath(false)]
        public string inputPath;
        [BuildTool.FilePath(false)]
        public string outputPath;
        public bool recursiveSearch = true;
        public Operation operation;

        public override void Execute()
        {
            string resolvedInputPath = FileUtility.ResolvePath(inputPath);
            string resolvedOutputPath = FileUtility.ResolvePath(outputPath);

            switch (operation)
            {
                case Operation.Copy:
                    Copy(resolvedInputPath, resolvedOutputPath);
                    break;
                case Operation.Move:
                    Move(resolvedInputPath, resolvedOutputPath);
                    break;
                case Operation.Delete:
                    Delete(resolvedInputPath);
                    break;
            }

            AssetDatabase.Refresh();
        }

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string resolvedInputPath = FileUtility.ResolvePerBuildPath(inputPath, releaseType, platform, architecture, distribution, buildTime, buildPath);
            string resolvedOutputPath = FileUtility.ResolvePerBuildPath(outputPath, releaseType, platform, architecture, distribution, buildTime, buildPath);

            switch (operation)
            {
                case Operation.Copy:
                    Copy(resolvedInputPath, resolvedOutputPath);
                    break;
                case Operation.Move:
                    Move(resolvedInputPath, resolvedOutputPath);
                    break;
                case Operation.Delete:
                    Delete(resolvedInputPath);
                    break;
            }

            AssetDatabase.Refresh();
        }

        protected override void DrawProperties(SerializedObject obj)
        {
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            EditorGUILayout.PropertyField(obj.FindProperty("operation"));

            if (containsWildcard)
                EditorGUILayout.PropertyField(obj.FindProperty("recursiveSearch"));

            EditorGUILayout.PropertyField(obj.FindProperty("inputPath"));

            if (operation != Operation.Delete)
                EditorGUILayout.PropertyField(obj.FindProperty("outputPath"));
        }

        private void Move(string inputPath, string outputPath, bool overwrite = true)
        {
            bool success = true;
            string errorString = "";
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            if (!containsWildcard && !File.Exists(inputPath))
            {
                // Error. Input does not exist.
                success = false;
                errorString = $"Input \"{inputPath}\" does not exist.";
            }

            if (success && !containsWildcard && overwrite && File.Exists(outputPath))
            {
                // Delete previous output.
                success = FileUtil.DeleteFileOrDirectory(outputPath);

                if (!success)
                    errorString = $"Could not overwrite existing file \"{outputPath}\".";
            }

            if (success && containsWildcard)
            {
                string inputDirectory = Path.GetDirectoryName(inputPath);
                string outputDirectory = Path.GetDirectoryName(outputPath);

                SearchOption option = recursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] fileList = Directory.GetFiles(inputDirectory, Path.GetFileName(inputPath), option);

                for (int i = 0; i < fileList.Length; i++)
                {
                    if (!success)
                        break;

                    string fileName = Path.GetFileName(fileList[i]);
                    string outputFile = Path.Combine(outputDirectory, fileName);

                    if (File.Exists(outputFile))
                        success = FileUtil.DeleteFileOrDirectory(outputFile);

                    if (!success)
                        errorString = $"Could not overwrite existing file \"{outputPath}\".";

                    FileUtil.MoveFileOrDirectory(fileList[i], outputFile);
                }
            }
            else if (success)
            {
                FileUtil.MoveFileOrDirectory(inputPath, outputPath);
            }

            if (!success && !string.IsNullOrEmpty(errorString))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "File Move Failed.", errorString,
                    true, null));
            }
        }

        private void Copy(string inputPath, string outputPath, bool overwrite = true)
        {
            bool success = true;
            string errorString = "";
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            if (!containsWildcard && !File.Exists(inputPath))
            {
                // Error. Input does not exist.
                success = false;
                errorString = $"Input \"{inputPath}\" does not exist.";
            }

            if (success && !containsWildcard && overwrite && File.Exists(outputPath))
            {
                // Delete previous output.
                success = FileUtil.DeleteFileOrDirectory(outputPath);

                if (!success)
                    errorString = $"Could not overwrite existing file \"{outputPath}\".";
            }

            if (success && containsWildcard)
            {
                string inputDirectory = Path.GetDirectoryName(inputPath);
                string outputDirectory = Path.GetDirectoryName(outputPath);

                SearchOption option = recursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] fileList = Directory.GetFiles(inputDirectory, Path.GetFileName(inputPath), option);

                for (int i = 0; i < fileList.Length; i++)
                {
                    if (!success)
                        break;

                    string fileName = Path.GetFileName(fileList[i]);
                    string outputFile = Path.Combine(outputDirectory, fileName);

                    if (File.Exists(outputFile))
                        success = FileUtil.DeleteFileOrDirectory(outputFile);

                    if (!success)
                        errorString = $"Could not overwrite existing file \"{outputPath}\".";

                    FileUtil.CopyFileOrDirectory(fileList[i], outputFile);
                }
            }
            else if (success)
            {
                FileUtil.CopyFileOrDirectory(inputPath, outputPath);
            }

            if (!success && !string.IsNullOrEmpty(errorString))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "File Copy Failed.", errorString,
                    true, null));
            }
        }

        private void Delete(string inputPath)
        {
            bool success = true;
            string errorString = "";
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            if (!containsWildcard && File.Exists(inputPath))
            {
                success = FileUtil.DeleteFileOrDirectory(inputPath);

                if (!success)
                    errorString = $"Could not delete file \"{inputPath}\".";
            }
            else if (containsWildcard)
            {
                string inputDirectory = Path.GetDirectoryName(inputPath);

                SearchOption option = recursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] fileList = Directory.GetFiles(inputDirectory, Path.GetFileName(inputPath), option);

                for (int i = 0; i < fileList.Length; i++)
                {
                    if (!success)
                        break;

                    success = FileUtil.DeleteFileOrDirectory(fileList[i]); ;

                    if (!success)
                        errorString = $"Could not delete file \"{fileList[i]}\".";
                }
            }
            else
            {
                // Error. File does not exist.
                success = false;
                errorString = $"Input \"{inputPath}\" does not exist.";
            }

            if (!success && !string.IsNullOrEmpty(errorString))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "File Delete Failed.", errorString,
                    true, null));
            }
        }
    }
}

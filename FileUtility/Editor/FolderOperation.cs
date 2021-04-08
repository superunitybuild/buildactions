using System.Collections;
using System.Diagnostics;
using System.IO;
using SuperUnityBuild.BuildTool;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{

    public class FolderOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction
    {
        public enum Operation
        {
            Move,
            Copy,
            Delete,
        }

        [SuperUnityBuild.BuildTool.FilePath(true)]
        public string inputPath;
        [SuperUnityBuild.BuildTool.FilePath(true)]
        public string outputPath;
        public Operation operation;

        public override void Execute()
        {
            switch (operation)
            {
                case Operation.Copy:
                    Copy(inputPath, outputPath);
                    break;
                case Operation.Move:
                    Move(inputPath, outputPath);
                    break;
                case Operation.Delete:
                    Delete(inputPath);
                    break;
            }

            AssetDatabase.Refresh();
        }

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildDistribution distribution, System.DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string resolvedInputPath = BuildProject.ResolvePath(inputPath.Replace("$BUILDPATH", buildPath), releaseType, platform, architecture, distribution, buildTime);
            string resolvedOutputPath = BuildProject.ResolvePath(outputPath.Replace("$BUILDPATH", buildPath), releaseType, platform, architecture, distribution, buildTime);

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
            EditorGUILayout.PropertyField(obj.FindProperty("operation"));
            EditorGUILayout.PropertyField(obj.FindProperty("inputPath"));

            if (operation != Operation.Delete)
                EditorGUILayout.PropertyField(obj.FindProperty("outputPath"));
        }

        private void Move(string inputPath, string outputPath, bool overwrite = true)
        {
            bool success = true;
            string errorString = "";

            if (!Directory.Exists(inputPath))
            {
                // Error. Input does not exist.
                success = false;
                errorString = "Input does not exist.";
            }

            // Make sure that all parent directories in path are already created.
            string parentPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }

            if (overwrite && Directory.Exists(outputPath))
            {
                // Delete previous output.
                success = FileUtil.DeleteFileOrDirectory(outputPath);

                if (!success)
                    errorString = "Could not overwrite existing folder.";
            }

            FileUtil.MoveFileOrDirectory(inputPath, outputPath);

            if (!success && !string.IsNullOrEmpty(errorString))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Folder Move Failed.", errorString,
                    true, null));
            }
        }

        private void Copy(string inputPath, string outputPath, bool overwrite = true)
        {
            bool success = true;
            string errorString = "";

            if (!Directory.Exists(inputPath))
            {
                // Error. Input does not exist.
                success = false;
                errorString = "Input does not exist.";
            }

            // Make sure that all parent directories in path are already created.
            string parentPath = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(parentPath))
            {
                Directory.CreateDirectory(parentPath);
            }

            if (success && overwrite && Directory.Exists(outputPath))
            {
                // Delete previous output.
                success = FileUtil.DeleteFileOrDirectory(outputPath);

                if (!success)
                    errorString = "Could not overwrite existing folder.";
            }

            FileUtil.CopyFileOrDirectory(inputPath, outputPath);

            if (!success && !string.IsNullOrEmpty(errorString))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Folder Copy Failed.", errorString,
                    true, null));
            }
        }

        private void Delete(string inputPath)
        {
            bool success = true;
            string errorString = "";

            if (Directory.Exists(inputPath))
            {
                FileUtil.DeleteFileOrDirectory(inputPath);
            }
            else
            {
                // Error. File does not exist.
                success = false;
                errorString = "Input does not exist.";
            }

            if (!success && !string.IsNullOrEmpty(errorString))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    "Folder Delete Failed.", errorString,
                    true, null));
            }
        }
    }

}
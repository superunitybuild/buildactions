using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using UnityEditor;

namespace SuperUnityBuild.BuildActions
{
    public static class FileUtility
    {
        public enum Operation
        {
            Move,
            Copy,
            Delete,
        }

        public enum PathType
        {
            Input,
            Output
        }

        public static bool ContainsWildcard(string inputPath)
        {
            return !string.IsNullOrEmpty(inputPath) && Path.GetFileNameWithoutExtension(inputPath).Contains("*");
        }

        public static bool Delete(string path, string failureErrorString, out string errorString)
        {
            errorString = "";

            bool success = FileUtil.DeleteFileOrDirectory(path);

            if (!success)
                errorString = failureErrorString;

            return success;
        }

        public static Action<string, string> GetCopyOrMoveAction(bool isCopy)
        {
            return isCopy ?
                FileUtil.CopyFileOrDirectory :
                FileUtil.MoveFileOrDirectory;
        }

        public static void OperationComplete(bool success, string errorTitle, string errorString)
        {
            if (!success && !string.IsNullOrEmpty(errorString) && !string.IsNullOrEmpty(errorTitle))
            {
                BuildNotificationList.instance.AddNotification(new BuildNotification(
                    BuildNotification.Category.Error,
                    errorTitle, errorString,
                    true, null));
            }
        }

        public static bool ValidatePath(string path, FileUtility.PathType pathType, bool checkForExistence, bool isFile, out string errorString)
        {
            errorString = "";

            if (string.IsNullOrEmpty(path))
            {
                errorString = $"{pathType} path not specified.";
                return false;
            }

            if (
                checkForExistence &&
                (
                    (isFile && !File.Exists(path)) ||
                    (!isFile && !Directory.Exists(path))
                )
            )
            {
                errorString = $"{pathType} path \"{path}\" does not exist.";
                return false;
            }

            return true;
        }
    }
}

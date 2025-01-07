using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using UnityEditor;
using Operation = SuperUnityBuild.BuildActions.FileUtility.Operation;

namespace SuperUnityBuild.BuildActions
{
    public class FolderOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        [BuildTool.FilePath(true)]
        public string inputPath;
        [BuildTool.FilePath(true)]
        public string outputPath;
        public Operation operation;

        public override void Execute()
        {
            string resolvedInputPath = ResolveExecuteTokens(inputPath);
            string resolvedOutputPath = ResolveExecuteTokens(outputPath);

            PerformOperation(resolvedInputPath, resolvedOutputPath);
        }

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string resolvedInputPath = ResolvePerBuildExecuteTokens(inputPath, releaseType, platform, target, scriptingBackend, distribution, buildTime, buildPath);
            string resolvedOutputPath = ResolvePerBuildExecuteTokens(outputPath, releaseType, platform, target, scriptingBackend, distribution, buildTime, buildPath);

            PerformOperation(resolvedInputPath, resolvedOutputPath);
        }

        protected override void DrawProperties(SerializedObject obj)
        {
            _ = EditorGUILayout.PropertyField(obj.FindProperty("operation"));
            _ = EditorGUILayout.PropertyField(obj.FindProperty("inputPath"));

            if (operation != Operation.Delete)
                _ = EditorGUILayout.PropertyField(obj.FindProperty("outputPath"));
        }

        private void PerformOperation(string inputPath, string outputPath)
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

        private void Copy(string inputPath, string outputPath, bool overwrite = true)
        {
            CopyOrMove(true, inputPath, outputPath, overwrite);
        }

        private void Move(string inputPath, string outputPath, bool overwrite = true)
        {
            CopyOrMove(false, inputPath, outputPath, overwrite);
        }

        private void CopyOrMove(bool isCopy, string inputPath, string outputPath, bool overwrite = true)
        {
            Action<string, string> fileOperation = FileUtility.GetCopyOrMoveAction(isCopy);
            bool success = ValidatePath(inputPath, FileUtility.PathType.Input, true, out string errorString);
            if (success)
                success = ValidatePath(outputPath, FileUtility.PathType.Output, false, out errorString);

            if (success)
            {
                // Make sure that all parent directories in path are already created.
                string parentPath = Path.GetDirectoryName(outputPath);

                if (!Directory.Exists(parentPath))
                    _ = Directory.CreateDirectory(parentPath);

                if (overwrite && Directory.Exists(outputPath))
                    success = FileUtility.Delete(outputPath, $"Could not overwrite existing folder \"{outputPath}\".", out errorString);

                if (success)
                    fileOperation(inputPath, outputPath);
            }

            FileUtility.OperationComplete(success, $"Folder {(isCopy ? "Copy" : "Move")} Failed.", errorString);
        }

        private void Delete(string inputPath)
        {
            bool success = ValidatePath(inputPath, FileUtility.PathType.Input, true, out string errorString);
            if (success)
                success = FileUtility.Delete(inputPath, $"Could not delete folder \"{inputPath}\".", out errorString);

            FileUtility.OperationComplete(success, "Folder Delete Failed.", errorString);
        }

        private bool ValidatePath(string path, FileUtility.PathType pathType, bool checkForExistence, out string errorString)
        {
            return FileUtility.ValidatePath(path, pathType, checkForExistence, false, out errorString);
        }
    }
}

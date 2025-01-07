using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using UnityEditor;
using Operation = SuperUnityBuild.BuildActions.FileUtility.Operation;

namespace SuperUnityBuild.BuildActions
{
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
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            _ = EditorGUILayout.PropertyField(obj.FindProperty("operation"));

            if (containsWildcard)
                _ = EditorGUILayout.PropertyField(obj.FindProperty("recursiveSearch"));

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

            bool success = true;
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            success = ValidatePath(inputPath, FileUtility.PathType.Input, !containsWildcard, out string errorString);

            if (success)
                success = ValidatePath(outputPath, FileUtility.PathType.Output, false, out errorString);

            if (success)
            {
                if (containsWildcard)
                {
                    string outputDirectory = Path.GetDirectoryName(outputPath);

                    _ = WildcardOperation(inputPath, recursiveSearch, (string filePath) =>
                    {
                        string fileName = Path.GetFileName(filePath);
                        string outputFile = Path.Combine(outputDirectory, fileName);
                        bool shouldContinue = true;

                        if (File.Exists(outputFile))
                            shouldContinue = FileUtility.Delete(outputFile, $"Could not overwrite existing file \"{outputFile}\".", out string errorString);

                        if (shouldContinue)
                            fileOperation(filePath, outputFile);

                        return shouldContinue;
                    });
                }
                else
                {
                    if (overwrite && File.Exists(outputPath))
                        success = FileUtility.Delete(outputPath, $"Could not overwrite existing file \"{outputPath}\".", out errorString);

                    if (success)
                        fileOperation(inputPath, outputPath);
                }
            }

            FileUtility.OperationComplete(success, $"File {(isCopy ? "Copy" : "Move")} Failed.", errorString);
        }

        private void Delete(string inputPath)
        {
            bool success = true;
            bool containsWildcard = FileUtility.ContainsWildcard(inputPath);

            success = ValidatePath(inputPath, FileUtility.PathType.Input, true, out string errorString);

            if (success)
            {
                success = containsWildcard ?
                    WildcardOperation(inputPath, recursiveSearch, (string filePath) =>
                        FileUtility.Delete(filePath, $"Could not delete file \"{filePath}\".", out string errorString)
                    ) :
                    FileUtility.Delete(inputPath, $"Could not delete file \"{inputPath}\".", out errorString);
            }

            FileUtility.OperationComplete(success, "File Delete Failed.", errorString);
        }

        private bool ValidatePath(string path, FileUtility.PathType pathType, bool checkForExistence, out string errorString)
        {
            return FileUtility.ValidatePath(path, pathType, checkForExistence, true, out errorString);
        }

        private static bool WildcardOperation(string path, bool recursiveSearch, Func<string, bool> operation)
        {
            bool success = false;

            string directory = Path.GetDirectoryName(path);

            SearchOption option = recursiveSearch ?
                SearchOption.AllDirectories :
                SearchOption.TopDirectoryOnly;

            string[] fileList = Directory.GetFiles(directory, Path.GetFileName(path), option);

            for (int i = 0; i < fileList.Length; i++)
            {
                success = operation.Invoke(fileList[i]);

                if (!success)
                    break;
            }

            return success;
        }
    }
}

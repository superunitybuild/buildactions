using System.IO;
using SuperUnityBuild.BuildTool;
using UnityEditor;

namespace SuperUnityBuild.BuildActions
{
    public class FileOperation : BuildAction, IPreBuildAction, IPreBuildPerPlatformAction, IPostBuildAction, IPostBuildPerPlatformAction
    {
        public enum Operation
        {
            Move,
            Copy,
            Delete,
        }

        [SuperUnityBuild.BuildTool.FilePath(false)]
        public string inputPath;
        [SuperUnityBuild.BuildTool.FilePath(false)]
        public string outputPath;
        public bool recursiveSearch = true;
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
            bool containsWildcard = string.IsNullOrEmpty(inputPath) ? false : Path.GetFileNameWithoutExtension(inputPath).Contains("*");

            EditorGUILayout.PropertyField(obj.FindProperty("operation"));

            if (containsWildcard)
                EditorGUILayout.PropertyField(obj.FindProperty("recursiveSearch"));

            EditorGUILayout.PropertyField(obj.FindProperty("inputPath"));

            if (operation != Operation.Delete)
                EditorGUILayout.PropertyField(obj.FindProperty("outputPath"));
        }

        private void Move(string inputPath, string outputPath, bool overwrite = true)
        {
            bool containsWildcard = string.IsNullOrEmpty(inputPath) ? false : Path.GetFileNameWithoutExtension(inputPath).Contains("*");

            if (!containsWildcard && !File.Exists(inputPath))
            {
                // Error. Input does not exist.
                return;
            }

            if (!containsWildcard && overwrite && File.Exists(outputPath))
            {
                // Delete previous output.
                FileUtil.DeleteFileOrDirectory(outputPath);
            }

            if (containsWildcard)
            {
                string inputDirectory = Path.GetDirectoryName(inputPath);
                string outputDirectory = Path.GetDirectoryName(outputPath);

                SearchOption option = recursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] fileList = Directory.GetFiles(inputDirectory, Path.GetFileName(inputPath), option);

                for (int i = 0; i < fileList.Length; i++)
                {
                    string fileName = Path.GetFileName(fileList[i]);
                    string outputFile = Path.Combine(outputDirectory, fileName);

                    if (File.Exists(outputFile))
                        FileUtil.DeleteFileOrDirectory(outputFile);

                    FileUtil.MoveFileOrDirectory(fileList[i], outputFile);
                }
            }
            else
            {
                FileUtil.MoveFileOrDirectory(inputPath, outputPath);
            }
        }

        private void Copy(string inputPath, string outputPath, bool overwrite = true)
        {
            bool containsWildcard = string.IsNullOrEmpty(inputPath) ? false : Path.GetFileNameWithoutExtension(inputPath).Contains("*");

            if (!containsWildcard && !File.Exists(inputPath))
            {
                // Error. Input does not exist.
                return;
            }

            if (!containsWildcard && overwrite && File.Exists(outputPath))
            {
                // Delete previous output.
                FileUtil.DeleteFileOrDirectory(outputPath);
            }

            if (containsWildcard)
            {
                string inputDirectory = Path.GetDirectoryName(inputPath);
                string outputDirectory = Path.GetDirectoryName(outputPath);

                SearchOption option = recursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] fileList = Directory.GetFiles(inputDirectory, Path.GetFileName(inputPath), option);

                for (int i = 0; i < fileList.Length; i++)
                {
                    string fileName = Path.GetFileName(fileList[i]);
                    string outputFile = Path.Combine(outputDirectory, fileName);

                    if (File.Exists(outputFile))
                        FileUtil.DeleteFileOrDirectory(outputFile);

                    FileUtil.CopyFileOrDirectory(fileList[i], outputFile);
                }
            }
            else
            {
                FileUtil.CopyFileOrDirectory(inputPath, outputPath);
            }
        }

        private void Delete(string inputPath)
        {
            bool containsWildcard = string.IsNullOrEmpty(inputPath) ? false : Path.GetFileNameWithoutExtension(inputPath).Contains("*");

            if (!containsWildcard && File.Exists(inputPath))
            {
                FileUtil.DeleteFileOrDirectory(inputPath);
            }
            else if (containsWildcard)
            {
                string inputDirectory = Path.GetDirectoryName(inputPath);

                SearchOption option = recursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] fileList = Directory.GetFiles(inputDirectory, Path.GetFileName(inputPath), option);

                for (int i = 0; i < fileList.Length; i++)
                {
                    FileUtil.DeleteFileOrDirectory(fileList[i]);
                }
            }
            else
            {
                // Error. File does not exist.
            }
        }
    }
}
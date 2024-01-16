using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace SuperUnityBuild.BuildActions
{
    public class GithubReleaseCreator : BuildAction, IPostBuildAction, IPostBuildPerPlatformAction
    {
        [Header("About the Owner")]
        [SerializeField] private string owner = "YourGitHubUsername";
        [SerializeField] private string repo = "YourRepositoryName";
        [SerializeField] private string token = "YourGitHubToken";

        [Header("About the Release")]
        [SerializeField] private string releaseName = "v1.0.0";
        [SerializeField] private string folderPath = "Assets/FolderToZip";
        [Multiline][SerializeField] private string releaseNotes = "Release Notes";

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            string _folderPath = ResolvePerBuildExecuteTokens(folderPath, releaseType, platform, architecture, scriptingBackend, distribution, buildTime, buildPath);
            UploadRelease(_folderPath);
        }

        private void UploadRelease(string _folderPath)
        {
            string zipFilePath = ZipFolder(_folderPath);
            if (string.IsNullOrEmpty(zipFilePath))
            {
                Debug.LogError("Failed to zip the folder. Please check the folder path.");
                return;
            }

            string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases";
            string json = $"{{\"tag_name\": \"{releaseName}\", \"name\": \"{releaseName}\", \"body\": \"{releaseNotes}\"}}";

            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"token {token}");
                request.SendWebRequest();

                while (!request.isDone)
                {
                    // Wait for the request to complete
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to create release: {request.error}");
                    return;
                }

                Debug.Log($"Release {releaseName} created successfully");
            }

            apiUrl = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            string releaseID;

            using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
            {
                request.SetRequestHeader("Authorization", $"token {token}");
                request.SendWebRequest();

                while (!request.isDone)
                {
                    // Wait for the request to complete
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string releasesJson = request.downloadHandler.text;
                    string lineTwo = releasesJson.Split("\n")[1];
                    releaseID = lineTwo.Substring(lineTwo.Length - 11, 9);
                }
                else
                {
                    Debug.LogError($"Failed to fetch releases: {request.error}");
                    return;
                }
            }

            apiUrl = $"https://uploads.github.com/repos/{owner}/{repo}/releases/{releaseID}/assets?name={Path.GetFileName(zipFilePath)}";

            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] zipFileBytes = File.ReadAllBytes(zipFilePath);
                request.uploadHandler = new UploadHandlerRaw(zipFileBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/zip");
                request.SetRequestHeader("Authorization", $"token {token}");
                request.SendWebRequest();

                while (!request.isDone)
                {
                    // Wait for the request to complete
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to upload zipfile: {request.error}");
                }
                else
                {
                    Debug.Log($"Zipfile uploaded successfully");
                }
            }
        }

        private string ZipFolder(string folderPath)
        {
            try
            {
                string buildParentFolder = Directory.GetParent(folderPath).FullName;
                string zipFileName = $"{Path.GetFileName(buildParentFolder) +"-"+  Path.GetFileName(folderPath)}.zip";
                string zipFilePath = Path.Combine(buildParentFolder + "\\", zipFileName);

                if (File.Exists(zipFilePath)) File.Delete(zipFilePath);
                ZipFile.CreateFromDirectory(folderPath, zipFilePath);
                return zipFilePath;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to zip folder: {e.Message}");
                return null;
            }
        }
    }
}
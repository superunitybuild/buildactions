using Mono.CSharp;
using SuperUnityBuild.BuildTool;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public class GithubReleaseCreator : BuildAction, IPostBuildPerPlatformAction
    {
        public string Owner = "your-github-username";
        public string Repo = "your-repo-name";
        public string Token = "your-personal-access-token";
        public string ReleaseBody = "Description of the release";
        private string TagName = "v1.0.0";
        private string ReleaseName = "Release v1.0.0";
        private string FolderPath = "Path/To/Your/Folder";
        private static readonly int MaxRetryAttempts = 3;
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildArchitecture architecture, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            Debug.Log(buildPath);
            FolderPath = buildPath;
            TagName = "v" + BuildSettings.productParameters.buildVersion;
            ReleaseName = "Release v" + BuildSettings.productParameters.buildVersion;
            _ = CheckAndCreateOrUpdateRelease();
        }

        private async Task CheckAndCreateOrUpdateRelease()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(10);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Unity-GithubRelease");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", Token);

                    var existingReleaseId = await GetExistingReleaseId(client);
                    if (existingReleaseId.HasValue)
                    {
                        Debug.Log("Release already exists. Uploading assets to the existing release.");
                        await UploadAssetsToRelease(client, existingReleaseId.Value);
                    }
                    else
                    {
                        Debug.Log("Release does not exist. Creating a new release.");
                        await CreateAndUploadNewRelease(client);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in CheckAndCreateOrUpdateRelease: {ex.Message}");
            }
        }

        private async Task<int?> GetExistingReleaseId(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync($"https://api.github.com/repos/{Owner}/{Repo}/releases/tags/{TagName}");
                if (response.IsSuccessStatusCode)
                {
                    var releaseResponse = JsonUtility.FromJson<ReleaseResponse>(await response.Content.ReadAsStringAsync());
                    return releaseResponse.id;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    Debug.LogError($"Failed to check existing release: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in GetExistingReleaseId: {ex.Message}");
                return null;
            }
        }

        private async Task CreateAndUploadNewRelease(HttpClient client)
        {
            var releaseData = new ReleaseData
            {
                tag_name = TagName,
                name = ReleaseName,
                body = ReleaseBody,
                draft = false,
                prerelease = false
            };

            Debug.Log("Creating release with data: " + JsonUtility.ToJson(releaseData));

            try
            {
                var content = new StringContent(JsonUtility.ToJson(releaseData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://api.github.com/repos/{Owner}/{Repo}/releases", content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("Release created successfully.");
                    var releaseResponse = JsonUtility.FromJson<ReleaseResponse>(await response.Content.ReadAsStringAsync());
                    await UploadAssetsToRelease(client, releaseResponse.id);
                }
                else
                {
                    Debug.LogError($"Failed to create release: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in CreateAndUploadNewRelease: {ex.Message}");
            }
        }

        private async Task UploadAssetsToRelease(HttpClient client, int releaseId)
        {
            var zipFilePath = ZipFolder(FolderPath);

            if (!File.Exists(zipFilePath))
            {
                Debug.LogError($"Zip file not found at {zipFilePath}");
                return;
            }

            var fileName = Path.GetFileName(zipFilePath);
            var url = $"https://uploads.github.com/repos/{Owner}/{Repo}/releases/{releaseId}/assets?name={fileName}";

            Debug.Log($"Uploading {fileName} to {url}");

            try
            {
                var fileBytes = File.ReadAllBytes(zipFilePath);
                Debug.Log($"Read {fileBytes.Length} bytes from {zipFilePath}");

                using (var fileContent = new ByteArrayContent(fileBytes))
                {
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

                    Debug.Log("Starting upload...");

                    for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
                    {
                        try
                        {
                            var response = await client.PostAsync(url, fileContent);

                            if (response.IsSuccessStatusCode)
                            {
                                Debug.Log($"Successfully uploaded {fileName}");
                                return;
                            }
                            else
                            {
                                var responseBody = await response.Content.ReadAsStringAsync();
                                Debug.LogError($"Failed to upload {fileName}: {response.StatusCode} - {responseBody}");
                                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                                {
                                    Debug.LogError("Upload failed due to rate limiting or permissions issue.");
                                    break; // No point in retrying if it's a permissions issue
                                }
                                else if (response.StatusCode == System.Net.HttpStatusCode.RequestEntityTooLarge)
                                {
                                    Debug.LogError("Upload failed due to the file being too large.");
                                    break; // No point in retrying if the file size is the issue
                                }
                            }
                        }
                        catch (HttpRequestException httpEx)
                        {
                            Debug.LogError($"HttpRequestException on attempt {attempt}: {httpEx.Message}");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Exception on attempt {attempt}: {ex.Message}");
                        }

                        if (attempt < MaxRetryAttempts)
                        {
                            Debug.Log($"Retrying upload in {RetryDelay.TotalSeconds} seconds...");
                            await Task.Delay(RetryDelay);
                        }
                    }

                    Debug.LogError("Failed to upload after multiple attempts.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in UploadAssetsToRelease: {ex.Message}");
                Debug.LogError($"Stack Trace: {ex.StackTrace}");
            }
        }

        private string ZipFolder(string folderPath)
        {
            var folderName = new DirectoryInfo(folderPath).Name;
            var zipFilePath = Path.Combine(Path.GetDirectoryName(folderPath), Directory.GetParent(folderPath).Name + ".zip");

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            Debug.Log($"Creating zip file: {zipFilePath} from folder: {folderPath}");
            ZipFile.CreateFromDirectory(folderPath, zipFilePath);
            Debug.Log($"Zip file created: {zipFilePath}");

            return zipFilePath;
        }

        [Serializable]
        private class ReleaseResponse
        {
            public int id;
        }

        [Serializable]
        private class ReleaseData
        {
            public string tag_name;
            public string name;
            public string body;
            public bool draft;
            public bool prerelease;
        }
    }
}

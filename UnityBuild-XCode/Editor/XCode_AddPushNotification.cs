using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddPushNotification : BuildAction, IPostProcessPerPlatformAction
    {
        [SerializeField] private bool development = false;
        [SerializeField] private string entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";
        
        public override void PostProcessExecute(BuildTarget target, string buildPath)
        {
            ProjectCapabilityManager capabilities =
                new ProjectCapabilityManager(PBXProject.GetPBXProjectPath(buildPath), buildPath + "/" + entitlementsPath, PBXProject.GetUnityTargetName());
            capabilities.AddPushNotifications(development);
            capabilities.WriteToFile();
        }
    }
}
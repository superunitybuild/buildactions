using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddPushNotification : XCode_Action
    {
        [SerializeField] private bool development = false;
        [SerializeField] private string entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";
        
        protected override void Process(BuildTarget target, string buildPath)
        {
            ProjectCapabilityManager capabilities =
                new ProjectCapabilityManager(PBXProject.GetPBXProjectPath(buildPath), buildPath + "/" + entitlementsPath, PBXProject.GetUnityTargetName());
            capabilities.AddPushNotifications(development);
            capabilities.WriteToFile();
        }
    }
}
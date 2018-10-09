using System.IO;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddAppGroups : XCode_Action
    {
        [SerializeField] private string entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";
        [SerializeField] private string[] appGroups;

        protected override void Process(BuildTarget buildTarget, string buildPath)
        {
            string entitlementPath = buildPath + entitlementsPath; //Path.Combine(buildPath, entitlementsPath);
            
            PlistDocument entitlements = new PlistDocument();
            
            if(File.Exists(entitlementPath))
                entitlements.ReadFromFile(entitlementPath);
            else
            {
                entitlements.Create();
            }
            
            PlistElementArray elementArray = new PlistElementArray();
            entitlements.root["com.apple.security.application-groups"] = elementArray;
            foreach (string appGroup in appGroups)
            {
                elementArray.values.Add(new PlistElementString(appGroup));
            }
            
            entitlements.WriteToFile(entitlementPath);
        }
    }
}
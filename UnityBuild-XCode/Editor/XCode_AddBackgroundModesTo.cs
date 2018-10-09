using System;
using SuperSystems.UnityBuild;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddBackgroundModesTo : XCode_Action
    {
        [SerializeField] private string entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";
        [SerializeField] private BackgroundModesOptions[] modes;

        protected override void Process(BuildTarget buildTarget, string buildPath)
        {
            ProjectCapabilityManager capabilities =
                new ProjectCapabilityManager(PBXProject.GetPBXProjectPath(buildPath), entitlementsPath,
                    PBXProject.GetUnityTargetName());
            System.Array.ForEach(modes,
                delegate(BackgroundModesOptions modesOptions) { capabilities.AddBackgroundModes(modesOptions); });
            capabilities.WriteToFile();
        }
    }
}
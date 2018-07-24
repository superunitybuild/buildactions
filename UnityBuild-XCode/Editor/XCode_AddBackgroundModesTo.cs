using System;
using SuperSystems.UnityBuild;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddBackgroundModesTo : BuildAction, IPostProcessPerPlatformAction
    {
        [SerializeField] private string entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";
        [SerializeField] private BackgroundModesOptions[] modes;

        public override void PostProcessExecute(BuildTarget buildTarget, string buildPath)
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
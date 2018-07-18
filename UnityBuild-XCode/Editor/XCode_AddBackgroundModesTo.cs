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
        [SerializeField] private BackgroundModesOptions[] modes;

        public override void PostProcessExecute(BuildTarget buildTarget, string buildPath)
        {
            ProjectCapabilityManager capabilities =
                new ProjectCapabilityManager(buildPath, PlayerSettings.applicationIdentifier, "Unity-iPhone");
            System.Array.ForEach(modes,
                delegate(BackgroundModesOptions modesOptions) { capabilities.AddBackgroundModes(modesOptions); });
            capabilities.WriteToFile();
        }
    }
}
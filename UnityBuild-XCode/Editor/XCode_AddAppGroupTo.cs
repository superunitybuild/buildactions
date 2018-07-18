using System;
using System.Collections;
using System.Collections.Generic;
using SuperSystems.UnityBuild;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddAppGroupTo : BuildAction, IPostProcessPerPlatformAction
    {
        [SerializeField] private string[] appGroups;

        public override void PostProcessExecute(BuildTarget buildTarget, string buildPath)
        {
            ProjectCapabilityManager capabilities =
                new ProjectCapabilityManager(buildPath, PlayerSettings.applicationIdentifier, PBXProject.GetUnityTargetName());
            capabilities.AddAppGroups(appGroups);
            capabilities.WriteToFile();
        }
    }
}
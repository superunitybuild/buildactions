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
    public class XCode_EnableCapabilities : BuildAction, IPostProcessPerPlatformAction
    {
        public enum Capability
        {
            AppGroups,
            BackgroundModes,
            PushNotification
        }
        
        [SerializeField] private string targetName = "Unity-iPhone";
        [SerializeField] private string entitlementsPath = "Unity-iPhone/Unity-iPhone.entitlements";
        [SerializeField] private Capability[] capabilities;

        public override void PostProcessExecute(BuildTarget buildTarget, string buildPath)
        {
            PBXProject project = new PBXProject();
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            project.ReadFromFile(projectPath);

            foreach (Capability capability in capabilities)
            {
                project.AddCapability(project.TargetGuidByName(targetName), GetCapabilityInstance(capability), entitlementsPath);
            }

            project.WriteToFile(projectPath);
        }

        PBXCapabilityType GetCapabilityInstance(Capability c)
        {
            switch (c)
            {
                    case Capability.AppGroups:
                        return PBXCapabilityType.AppGroups;
                    case Capability.BackgroundModes:
                        return PBXCapabilityType.BackgroundModes;
                    case Capability.PushNotification:
                        return PBXCapabilityType.PushNotifications;
                    default:
                        throw new Exception("Invalid capability");
            }
        }
    }
}
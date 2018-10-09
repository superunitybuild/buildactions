using System;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_SetBuildProperty : XCode_Action
    {
        [SerializeField] private string targetName = "Unity-iPhone";
        [SerializeField] private string propertyName;
        [SerializeField] private string value;

        protected override void Process(BuildTarget target, string buildPath)
        {
            Debug.Log("Running SetBuildProperty action");
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);
            project.SetBuildProperty(project.TargetGuidByName(targetName), propertyName, value);
            project.WriteToFile(projectPath);
        }
    }
}
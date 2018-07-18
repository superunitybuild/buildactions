using System;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddFramework : BuildAction, IPostProcessPerPlatformAction
    {
        [SerializeField] private string targetName = "Unity-iPhone";
        [SerializeField] private string frameworkName;
        [SerializeField] private bool weak;

        public override void PostProcessExecute(BuildTarget buildTarget, string buildPath)
        {
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);
            project.AddFrameworkToProject(project.TargetGuidByName(targetName), frameworkName + ".framework", weak);
            project.WriteToFile(projectPath);
        }
    }
}
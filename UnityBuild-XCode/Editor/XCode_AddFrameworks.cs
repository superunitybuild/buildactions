using System;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddFrameworks : XCode_Action
    {
        [SerializeField] private string targetName = "Unity-iPhone";
        [SerializeField] private string[] frameworks;
        [SerializeField] private bool weak;

        protected override void Process(BuildTarget buildTarget, string buildPath)
        {
            string projectPath = PBXProject.GetPBXProjectPath(buildPath);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projectPath);

            foreach (string framework in frameworks)
            {
                project.AddFrameworkToProject(project.TargetGuidByName(targetName), framework, weak);
            }

            project.WriteToFile(projectPath);
        }
    }
}
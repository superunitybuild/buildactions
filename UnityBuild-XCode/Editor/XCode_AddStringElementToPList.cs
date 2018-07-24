using System.IO;
using Sabresaurus.PlayerPrefsExtensions;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddStringElementToPList : BuildAction, IPostProcessPerPlatformAction
    {
        [SerializeField] private string key;
        [SerializeField] private string value;
        
        public override void PostProcessExecute(BuildTarget target, string buildPath)
        {
            string filePath = Path.Combine(buildPath, "Info.plist");
            PlistDocument pList = new PlistDocument();
            pList.ReadFromFile(filePath);
            pList.root.SetString(key, value);
            pList.WriteToFile(filePath);
        }
    }
}
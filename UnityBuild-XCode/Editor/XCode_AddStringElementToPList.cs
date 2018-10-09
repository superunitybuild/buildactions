using System.IO;
using SuperSystems.UnityBuild.Interfaces;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SuperSystems.UnityBuild
{
    public class XCode_AddStringElementToPList : XCode_Action
    {
        [SerializeField] private string key;
        [SerializeField] private string value;
        
        protected override void Process(BuildTarget target, string buildPath)
        {
            string filePath = Path.Combine(buildPath, "Info.plist");
            PlistDocument pList = new PlistDocument();
            pList.ReadFromFile(filePath);
            pList.root.SetString(key, value);
            pList.WriteToFile(filePath);
        }
    }
}
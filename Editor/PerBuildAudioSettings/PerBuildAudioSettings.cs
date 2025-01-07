using SuperUnityBuild.BuildTool;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SuperUnityBuild.BuildActions
{
    public class PerBuildAudioSettings : BuildAction, IPreBuildPerPlatformAction, IPostBuildPerPlatformAction, IPreBuildPerPlatformActionCanConfigureEditor
    {
        [Tooltip("Spatializer plugin to use for this build")] public string SpatializerPlugin = "";

        public override void PerBuildExecute(BuildReleaseType releaseType, BuildPlatform platform, BuildTool.BuildTarget target, BuildScriptingBackend scriptingBackend, BuildDistribution distribution, DateTime buildTime, ref BuildOptions options, string configKey, string buildPath)
        {
            // Set spatializer plugin
            AudioSettings.SetSpatializerPluginName(SpatializerPlugin);
        }

        private int index = 0;
        private string[] spatializerPluginNames;

        private void OnEnable()
        {
            spatializerPluginNames = AudioSettings.GetSpatializerPluginNames();
            spatializerPluginNames = spatializerPluginNames.Prepend("None").ToArray();
            index = Math.Max(Array.IndexOf(spatializerPluginNames, SpatializerPlugin), 0);
        }

        protected override void DrawProperty(SerializedProperty prop)
        {
            switch (prop.name)
            {
                case "SpatializerPlugin":
                    index = EditorGUILayout.Popup(prop.displayName, index, spatializerPluginNames);
                    prop.stringValue = index == 0 ? "" : spatializerPluginNames[index];
                    break;

                default:
                    base.DrawProperty(prop);
                    break;
            }
        }
    }
}

using Toolset.Core;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// Editor window used to show the user controls for the static data flow.
    /// </summary>
    public class StaticDataControlWindow : EditorWindow
    {
        private const float c_spacingValue = 10f;
        
        /// <summary>
        /// Opens the Proto Editor Window.
        /// </summary>
        [MenuItem("Toolset/Data/Open Static Data Control Window")]
        public static void OpenProtoJsonConverterWindow()
        {
            GetWindow<StaticDataControlWindow>("Static Data");
        }

        public void OnGUI()
        {
            GUILayout.Label("Global Controls");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Proto Models"))
            {
                StaticDataControlUtils.GenerateProto();
            }
            
            if (GUILayout.Button("Copy JSON Data To Streaming Assets"))
            {
                StaticDataControlUtils.CopyDataJsonIntoStreamingAssets();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(c_spacingValue);
            
            GUILayout.Label("Create JSON Model Instances");
            bool dirty = false;
            for (int i = 0; i < StaticDataControlUtils.CompiledClassNames.Count; ++i)
            {
                if (GUILayout.Button("Create new ".StringBuilderAppend(StaticDataControlUtils.CompiledClassNames[i])))
                {
                    SaveManager.SerializeGeneratedModelToJson(StaticDataControlUtils.CompiledClassNames[i], ToolsetEditorConstants.s_pathToJsonDataDirectory);
                    dirty = true;
                }
            }
            
            if (dirty)
                AssetDatabase.Refresh();
        }
    }
}

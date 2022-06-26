using Toolset.Core;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// Editor window used to show the user controls for the static data flow.
    /// </summary>
    public class RecordEditorWindow : EditorWindow
    {
        private const float c_spacingValue = 10f;
        
        /// <summary>
        /// Opens the Static Data Control Window.
        /// </summary>
        [MenuItem("Toolset/Static Data/Record Editor")]
        public static void OpenProtoJsonConverterWindow()
        {
            GetWindow<RecordEditorWindow>("Records");
        }

        public void OnGUI()
        {
            GUILayout.Label("Global Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Copy JSON Data To Streaming Assets"))
            {
                StaticDataControlUtils.CopyDataJsonIntoStreamingAssets();
            }

            GUILayout.Space(c_spacingValue);
            
            GUILayout.Label("Create JSON Model Instances", EditorStyles.boldLabel);
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

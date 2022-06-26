using Toolset.Core;
using Toolset.Core.EditorTools;
using UnityEditor;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// Editor window used to show the user controls for the static data flow.
    /// </summary>
    public class RecordEditorWindow : ToolsetEditorWindow
    {
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
            BoldLabel("Global Controls");

            if (BaseButton("Copy JSON Data To Streaming Assets", "Copies Data records into configured Streaming Assets data directory for runtime use in builds"))
            {
                StaticDataControlUtils.CopyDataJsonIntoStreamingAssets();
            }

            EditorGUILayout.Space();
            
            BoldLabel("Create JSON Model Instances", "Creates and saves a new record of a specific data model type");
            using (var assetScope = new AssetDirtyScope())
            {
                for (int i = 0; i < StaticDataControlUtils.CompiledClassNames.Count; ++i)
                {
                    if (BaseButton("Create new ".StringBuilderAppend(StaticDataControlUtils.CompiledClassNames[i])))
                    {
                        SaveManager.SerializeGeneratedModelToJson(StaticDataControlUtils.CompiledClassNames[i],
                            ToolsetEditorConstants.s_pathToJsonDataDirectory);
                        assetScope.SetDirty();
                    }
                }
            }
        }
    }
}

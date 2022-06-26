using System.Collections.Generic;
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
        private List<RecordCategoryListItem> m_recordCategories = new List<RecordCategoryListItem>();
        
        /// <summary>
        /// Opens the Record Editor Window.
        /// </summary>
        [MenuItem("Toolset/Static Data/Record Editor")]
        public static void OpenProtoJsonConverterWindow()
        {
            GetWindow<RecordEditorWindow>("Records");
        }
        
        public void OnEnable()
        {
            PopulateWindow();
        }

        public void OnGUI()
        {
            BoldLabel("Global Controls");

            if (BaseButton("Copy JSON Data To Streaming Assets", "Copies Data records into configured Streaming Assets data directory for runtime use in builds"))
            {
                StaticDataControlUtils.CopyDataJsonIntoStreamingAssets();
            }

            EditorGUILayout.Space();
            
            BoldLabel("Edit Records", "Create and edit records of a specific data model type");
            foreach (RecordCategoryListItem listItem in m_recordCategories)
            {
                listItem.OnGui();
            }
        }

        private void PopulateWindow()
        {
            m_recordCategories.Clear();
            foreach (string className in StaticDataControlUtils.CompiledClassNames)
            {
                m_recordCategories.Add(new RecordCategoryListItem(className));
            }
        }
    }
}

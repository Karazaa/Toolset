using System.Collections.Generic;
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
            GUILayout.Label("Global Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Copy JSON Data To Streaming Assets"))
            {
                StaticDataControlUtils.CopyDataJsonIntoStreamingAssets();
            }

            GUILayout.Space(c_spacingValue);
            
            GUILayout.Label("Edit Records", EditorStyles.boldLabel);
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

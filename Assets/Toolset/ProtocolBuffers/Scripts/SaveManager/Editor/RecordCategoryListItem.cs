using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolset.Core;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class RecordCategoryListItem
    {
        public string ClassName { get; }
        private List<RecordListItem> m_recordListItems = new List<RecordListItem>();
        private bool m_showContent;
        
        public RecordCategoryListItem(string className)
        {
            ClassName = className;
            
            IEnumerable<string> filePaths =
                SaveManager.GetSerializedJsonModelFilePaths(ClassName,
                    ToolsetEditorConstants.s_pathToJsonDataDirectory);

            foreach (string filePath in filePaths)
            {
                m_recordListItems.Add(new RecordListItem(filePath));
            }
        }

        public void OnGui()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            m_showContent = EditorGUILayout.Foldout(m_showContent, ClassName, EditorStyles.foldoutHeader);

            if (m_showContent)
            {
                EditorGUI.indentLevel++;
                
                foreach (RecordListItem listItem in m_recordListItems)
                {
                    listItem.OnGui();
                }

                bool dirty = false;
                
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * ToolsetEditorConstants.c_editorSpaceIndentLevel);
                if (GUILayout.Button("Create new ".StringBuilderAppend(ClassName)))
                {
                    m_recordListItems.Add(new RecordListItem(
                        SaveManager.SerializeGeneratedModelToJson(ClassName, ToolsetEditorConstants.s_pathToJsonDataDirectory)));
                    dirty = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
                
                if (dirty)
                    AssetDatabase.Refresh();
                
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}

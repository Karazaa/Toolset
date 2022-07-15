using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolset.Core;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public class RecordCategoryListItem
    {
        public Type ClassType { get; }
        public string ClassName { get; }
        private List<RecordListItem> m_recordListItems = new List<RecordListItem>();
        private bool m_showContent;
        
        public RecordCategoryListItem(string className)
        {
            ClassName = className;
            
            IEnumerable<string> filePaths =
                SaveManager.GetSerializedJsonModelFilePaths(ClassName,
                    ToolsetEditorConstants.s_pathToJsonDataDirectory);

            ClassType = StaticDataControlUtils.GetTypeAcrossAllAssemblies(ClassName);
            
            foreach (string filePath in filePaths)
            {
                m_recordListItems.Add(new RecordListItem(ClassType, filePath));
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
                    m_recordListItems.Add(new RecordListItem(ClassType,
                        SaveManager.SerializeGeneratedModelToJson(ClassName, ToolsetEditorConstants.s_pathToJsonDataDirectory)));
                    dirty = true;
                }
                if (GUILayout.Button("Save all ".StringBuilderAppend(ClassName)))
                {
                    foreach (RecordListItem recordListItem in m_recordListItems)
                    {
                        recordListItem.Save();
                    }
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

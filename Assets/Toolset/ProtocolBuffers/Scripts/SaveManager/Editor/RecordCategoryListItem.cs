using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Toolset.Core;
using Toolset.Core.EditorTools;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public class RecordCategoryListItem : ToolsetEditorGUI
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
                if (BaseButton("Create New ".StringBuilderAppend(ClassName)))
                {
                    m_recordListItems.Add(new RecordListItem(ClassType,
                        SaveManager.SerializeGeneratedModelToJson(ClassName, ToolsetEditorConstants.s_pathToJsonDataDirectory)));
                    dirty = true;
                }
                
                if (m_recordListItems.Count > 0 && BaseButton("Save All ".StringBuilderAppend(ClassName.StringBuilderAppend("s"))))
                {
                    foreach (RecordListItem recordListItem in m_recordListItems)
                    {
                        recordListItem.Save();
                    }
                }
                
                if (m_recordListItems.Count > 0 && BaseButton("Delete Last ".StringBuilderAppend(ClassName)))
                {
                    int lastItemIndex = m_recordListItems.Count - 1;
                    m_recordListItems[lastItemIndex].Delete();
                    m_recordListItems.RemoveAt(lastItemIndex);
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

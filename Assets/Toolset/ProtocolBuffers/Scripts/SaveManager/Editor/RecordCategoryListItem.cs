using UnityEditor;
using UnityEngine;
using Toolset.Core;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class RecordCategoryListItem
    {
        public RecordCategoryListItem(string className)
        {
            ClassName = className;
        }
        
        public string ClassName { get; }
        
        private bool m_showContent;
        
        public void OnGui()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            m_showContent = EditorGUILayout.Foldout(m_showContent, ClassName, EditorStyles.foldoutHeader);

            if (m_showContent)
            {
                EditorGUI.indentLevel++;
                
                // TODO: MANAGE RECORD LIST ITEMS

                bool dirty = false;
                
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * ToolsetEditorConstants.c_editorSpaceIndentLevel);
                if (GUILayout.Button("Create new ".StringBuilderAppend(ClassName)))
                {
                    SaveManager.SerializeGeneratedModelToJson(ClassName, ToolsetEditorConstants.s_pathToJsonDataDirectory);
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

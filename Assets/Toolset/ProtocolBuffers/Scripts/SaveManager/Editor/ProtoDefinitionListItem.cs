using Toolset.Core;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// A list item that is rendered in the ProtoDefinitionWindow custom editor. Used to display
    /// a single proto file, it just implements an OnGui() method.
    /// </summary>
    public class ProtoDefinitionListItem : ProtoWriteable
    {
        private bool m_showContent;

        public ProtoDefinitionListItem() : base() { }
        
        public ProtoDefinitionListItem(string fileContents) : base(fileContents) { }
        
        public void OnGui()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            m_showContent = EditorGUILayout.Foldout(m_showContent, ClassName, EditorStyles.foldoutHeader);

            if (m_showContent)
            {
                EditorGUI.indentLevel++;
                ClassName = EditorGUILayout.TextField("Proto Class Name", ClassName);

                EditorGUI.indentLevel++;
                foreach (ProtoField field in Fields)
                {
                    EditorGUILayout.Space();

                    if (field.IsGuid())
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * ToolsetEditorConstants.c_editorSpaceIndentLevel);
                        BaseLabel(field.FieldName);
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        field.FieldName = EditorGUILayout.TextField("Field Name", field.FieldName);
                        field.FieldType = (ProtoFieldEnum) EditorGUILayout.EnumPopup("Field Type", field.FieldType);
                    }
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * ToolsetEditorConstants.c_editorSpaceIndentLevel);
                if (GUILayout.Button("Add New Field"))
                {
                    Fields.Add(new ProtoField());
                }
                
                if (Fields.Count > 1 && GUILayout.Button("Remove Last Field"))
                {
                    Fields.RemoveAt(Fields.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}

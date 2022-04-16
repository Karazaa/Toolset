using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class ProtoDefinitionListItem : ProtoWriteable
    {
        private bool m_showContent;

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
                    field.FieldName = EditorGUILayout.TextField("Field Name", field.FieldName);
                    field.FieldType = (ProtoFieldEnum) EditorGUILayout.EnumPopup("Field Type", field.FieldType);
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add New Field"))
                {
                    Fields.Add(new ProtoField());
                }
                
                if (Fields.Count > 0 && GUILayout.Button("Remove Last Field"))
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
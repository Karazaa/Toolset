using UnityEditor;
using System;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public class RecordListItem : JsonWriteable
    {
        private bool m_showContent;

        public RecordListItem(Type classType, string filePath) : base(classType, filePath) { }

        public void OnGui()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            m_showContent = EditorGUILayout.Foldout(m_showContent, RecordName, EditorStyles.foldoutHeader);

            if (m_showContent)
            {
                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                foreach (PropertyInfoValue propertyInfoValue in ExistingProperties)
                {
                    propertyInfoValue.DoEditorGuiLayout();
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}

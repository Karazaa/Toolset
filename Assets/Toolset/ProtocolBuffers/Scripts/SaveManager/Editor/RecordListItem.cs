using UnityEditor;
using UnityEngine;
using System.IO;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class RecordListItem
    {
        public string RecordName { get; }
        private bool m_showContent;

        public RecordListItem(string filePath)
        {
            RecordName = Path.GetFileNameWithoutExtension(filePath);
        }

        public void OnGui()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            m_showContent = EditorGUILayout.Foldout(m_showContent, RecordName, EditorStyles.foldoutHeader);
            
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}

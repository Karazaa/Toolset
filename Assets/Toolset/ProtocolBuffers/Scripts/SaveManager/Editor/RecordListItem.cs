using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class RecordListItem
    {
        public string RecordName { get; }
        private List<(PropertyInfo, object)> m_existingProperties = new List<(PropertyInfo, object)>();
        private bool m_showContent;

        public RecordListItem(Type classType, string filePath)
        {
            RecordName = Path.GetFileNameWithoutExtension(filePath);

            MethodInfo method = typeof(SaveManager).GetMethod("DeserializeObjectFromJson");
            MethodInfo generic = method?.MakeGenericMethod(classType);
            object result = generic?.Invoke(this, new object[]{filePath});
            
            PropertyInfo[] propertyInfos = classType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                m_existingProperties.Add((propertyInfo, propertyInfo.GetValue(result)));
            }
            
            // TODO: Use m_existingProperties to generate editors
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

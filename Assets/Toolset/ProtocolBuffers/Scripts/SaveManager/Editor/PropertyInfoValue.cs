using Newtonsoft.Json;
using System.Reflection;
using Toolset.Core;
using Toolset.Core.EditorTools;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public class PropertyInfoValue : ToolsetEditorGUI
    {
        public PropertyInfo PropertyInfo;
        public object Value;

        public void DoEditorGuiLayout()
        {
            if (PropertyInfo.PropertyType == typeof(int))
            {
                Value = EditorGUILayout.IntField(PropertyInfo.Name, (int) Value);
            }
            else if (PropertyInfo.PropertyType == typeof(string))
            {
                if (PropertyInfo.Name == ToolsetEditorConstants.c_protoGuidFieldName)
                {
                    EditorGUI.BeginDisabledGroup(PropertyInfo.Name == ToolsetEditorConstants.c_protoGuidFieldName);
                    EditorGUILayout.TextField(PropertyInfo.Name, (string) Value);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    Value = EditorGUILayout.TextField(PropertyInfo.Name, (string) Value);
                }
            }
            else if (PropertyInfo.PropertyType == typeof(float))
            {
                Value = EditorGUILayout.FloatField(PropertyInfo.Name, (float) Value);
            }
            else if (PropertyInfo.PropertyType == typeof(long))
            {
                Value = EditorGUILayout.LongField(PropertyInfo.Name, (long) Value);
            }
            else if (PropertyInfo.PropertyType == typeof(double))
            {
                Value = EditorGUILayout.DoubleField(PropertyInfo.Name, (double) Value);
            }
            else
            {
                string newValue = EditorGUILayout.TextField(PropertyInfo.Name, (string) Value);
                if (newValue != null)
                    Value = JsonConvert.DeserializeObject(newValue, PropertyInfo.PropertyType);
            }
        }
    }
}

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
        public PropertyInfo PropertyInfo { get; set; }
        public object Value { get; set; }

        public void DoEditorGuiLayout()
        {
            if (PropertyInfo.PropertyType == typeof(int))
            {
                Value = EditorGUILayout.IntField(PropertyInfo.Name, (int) Value);
            }
            else if (PropertyInfo.PropertyType == typeof(ToolsetGuid))
            {
                Value ??= new ToolsetGuid();

                ToolsetGuid valueAsGuid = (ToolsetGuid) Value;
                if (PropertyInfo.Name == ToolsetEditorConstants.c_protoGuidFieldName)
                {
                    EditorGUI.BeginDisabledGroup(PropertyInfo.Name == ToolsetEditorConstants.c_protoGuidFieldName);
                    EditorGUILayout.TextField(PropertyInfo.Name, valueAsGuid.Guid);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    valueAsGuid.Guid = EditorGUILayout.TextField(PropertyInfo.Name, valueAsGuid.Guid);
                }
            }
            else if (PropertyInfo.PropertyType == typeof(string))
            {
                Value = EditorGUILayout.TextField(PropertyInfo.Name, (string) Value);
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

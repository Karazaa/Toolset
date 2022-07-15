using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEditor;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public class PropertyInfoValue
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
                Value = JsonConvert.DeserializeObject(newValue, PropertyInfo.PropertyType);
            }
        }
    }
    
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public abstract class JsonWriteable
    {
        protected string RecordName { get; }
        protected List<PropertyInfoValue> ExistingProperties { get; } = new List<PropertyInfoValue>();

        private string m_filePath;
        private object m_result;

        protected JsonWriteable(Type classType, string filePath)
        {
            m_filePath = filePath;
            RecordName = Path.GetFileNameWithoutExtension(filePath);
            
            MethodInfo deserializeMethod = typeof(SaveManager).GetMethod("DeserializeObjectFromJson");
            MethodInfo genericDeserializeMethod = deserializeMethod?.MakeGenericMethod(classType);
            
            m_result = genericDeserializeMethod?.Invoke(this, new object[]{filePath});
            
            PropertyInfo[] propertyInfos = classType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ExistingProperties.Add(new PropertyInfoValue { PropertyInfo = propertyInfo, Value = propertyInfo.GetValue(m_result) });
            }
        }

        /// <summary>
        /// TODO: Fill me out
        /// </summary>
        public void Save()
        {
            foreach (PropertyInfoValue propertyInfoValue in ExistingProperties)
            {
                propertyInfoValue.PropertyInfo.SetValue(m_result, propertyInfoValue.Value);
            }

            SaveManager.SerializeObjectToJsonAndSave(m_filePath, m_result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Toolset.Core.EditorTools;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// TODO: Fill me out
    /// </summary>
    public abstract class JsonWriteable : ToolsetEditorGUI
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
        public virtual void Save()
        {
            foreach (PropertyInfoValue propertyInfoValue in ExistingProperties)
            {
                propertyInfoValue.PropertyInfo.SetValue(m_result, propertyInfoValue.Value);
            }

            SaveManager.SerializeObjectToJsonAndSave(m_filePath, m_result);
        }

        /// <summary>
        /// TODO: Fill me out
        /// </summary>
        public virtual void Delete()
        {
            SaveManager.DeleteFileAndMeta(m_filePath);
        }
    }
}

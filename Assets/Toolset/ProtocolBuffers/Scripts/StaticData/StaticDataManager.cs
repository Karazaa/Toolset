using System;
using System.Collections.Generic;
using System.IO;
using Toolset.Core;

namespace Toolset.ProtocolBuffers
{
    /// <summary>
    /// Static class that facilitates retrieving static data at runtime.
    /// </summary>
    public static class StaticDataManager
    {
        private static readonly Dictionary<string, object> s_data = new Dictionary<string, object>();
        private static bool s_isInitialized = false;

        /// <summary>
        /// Initializes the Static Data Manager, causing it to load JSON records from StreamingAssets into memory.
        /// </summary>
        public static void Initialize()
        {
            if (s_isInitialized)
                return;

            string[] classNames = File.ReadAllText(Path.Combine(ToolsetRuntimeConstants.s_pathToStreamingAssetsDirectory, ToolsetRuntimeConstants.s_staticDataManifestFileName)).Split("\n");
            
            foreach (string className in classNames)
            {
                Type type = ToolsetGlobalUtils.GetTypeAcrossAllAssemblies(className); 
                List<object> records = SaveManager.DeserializeJsonModelsOfType(ToolsetRuntimeConstants.s_pathToJsonStreamingAssetsDirectory, type);
                
                foreach (object record in records)
                {
                    ToolsetGuid guid = type?.GetProperty(ToolsetGlobalConstants.c_protoGuidFieldName)?.GetValue(record) as ToolsetGuid;
                    if (guid != null)
                        s_data.Add(guid.Guid, record);
                }
            }

            s_isInitialized = true;
        }

        /// <summary>
        /// Uninitializes the StaticDataManager causing it to drop its dictionary of records from memory.
        /// </summary>
        public static void Uninitialize()
        {
            s_data.Clear();
            s_isInitialized = false;
        }
        
        /// <summary>
        /// Attempts to get a record from static data.
        /// </summary>
        /// <param name="guid">The ToolsetGuid of the record.</param>
        /// <param name="output">The record to output.</param>
        /// <typeparam name="TType">The type of the record to retrieve.</typeparam>
        /// <returns>Whether or not the operation was successful.</returns>
        public static bool TryGet<TType>(ToolsetGuid guid, out TType output)
        {
            bool result = s_data.TryGetValue(guid.Guid, out object record);
            output = (TType) record;
            return result;
        }
    }
}

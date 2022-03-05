using System;
using System.Collections;
using System.Collections.Generic;
using Toolset.Core;
using UnityEditor;
using UnityEngine;

namespace Toolset.ProtocolBuffers
{
    public class ProtoJsonConverterWindow : EditorWindow
    {
        /// <summary>
        /// Opens the Proto JSON Converter Window.
        /// </summary>
        [MenuItem("Toolset/Data/Open Proto JSON Converter")]
        public static void OpenProtoJsonConverterWindow()
        {
            GetWindow<ProtoJsonConverterWindow>("Proto JSON Converter");
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Generate Proto Models"))
            {
                ProtoJsonConverter.GeneratePersistentProto();
            }

            bool dirty = false;
            for (int i = 0; i < ProtoJsonConverter.CompiledClassNames.Count; ++i)
            {
                if (GUILayout.Button("Create new ".StringBuilderAppend(ProtoJsonConverter.CompiledClassNames[i])))
                {
                    ProtoJsonConverter.SerializeGenerateModelToJson(ProtoJsonConverter.CompiledClassNames[i]);
                    dirty = true;
                }
            }
            
            if (dirty)
                AssetDatabase.Refresh();
        }
    }
}

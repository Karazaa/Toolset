using System;
using System.Collections.Generic;
using Toolset.Core;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// A class of utilities for proto generation and serialization to assist the StaticDataControlWindow.
    /// </summary>
    public static class StaticDataControlUtils
    {
        public static List<string> CompiledClassNames = new List<string>();
        private const string c_classIdentifier = "public partial class ";

        /// <summary>
        /// Generates CSharp files from proto definitions
        /// </summary>
        public static void GenerateProto()
        {
            SaveManager.GenerateCSharpFromProto(ToolsetEditorConstants.s_pathToProtoDataDirectory, ToolsetEditorConstants.s_pathToProtoModelGeneratedDirectory);
        }

        [DidReloadScripts]
        public static void OnReload()
        {
            List<string> modelContents =
                SaveManager.LoadAllFileContentsFromDirectory(ToolsetEditorConstants.s_pathToProtoModelGeneratedDirectory, "*.cs");
            
            CompiledClassNames = new List<string>();
            for (int i = 0; i < modelContents.Count; ++i)
            {
                CompiledClassNames = GetAllClassNamesForFile(modelContents[i], CompiledClassNames);
            }
        }

        /// <summary>
        /// Copies JSON record files from the data directory specified in ToolsetRuntimeConstants into the
        /// StreamingAssets folder so they can be accessed at runtime in builds if needed.
        /// </summary>
        public static void CopyDataJsonIntoStreamingAssets()
        {
            SaveManager.CopyDirectory(ToolsetEditorConstants.s_pathToJsonDataDirectory, ToolsetRuntimeConstants.s_pathToJsonStreamingAssetsDirectory, true);

            string manifestContent = string.Empty;
            for (int i = 0; i < CompiledClassNames.Count; ++i)
            {
                if (i == 0)
                {
                    manifestContent = CompiledClassNames[i];
                }
                else
                {
                    manifestContent = manifestContent.StringBuilderAppend("\n", CompiledClassNames[i]);
                }
            }

            SaveManager.SaveContentsToFile(ToolsetRuntimeConstants.s_staticDataManifestFileName, ToolsetRuntimeConstants.s_pathToStreamingAssetsDirectory, manifestContent);

            Debug.Log("Copied entire contents of JSON Data Directory to StreamingAssets!");
            UnityEditor.AssetDatabase.Refresh();
        }

        private static List<string> GetAllClassNamesForFile(string rawText, List<string> classNames)
        {
            int index = rawText.IndexOf(c_classIdentifier);

            if (index == -1)
                return classNames;

            rawText = rawText.Remove(0, index + c_classIdentifier.Length).TrimStart();

            int spaceIndex = rawText.IndexOf(' ');
            string className = rawText.Substring(0, spaceIndex);
            if (className != ToolsetEditorConstants.c_toolsetGuidClassName)
                classNames.Add(className);

            rawText = rawText.Remove(0, spaceIndex);

            return GetAllClassNamesForFile(rawText, classNames);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ProtoBuf.Reflection;
using Toolset.Core;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Toolset.ProtocolBuffers
{
    /// <summary>
    /// Class that generates C# classes from proto models and then
    /// </summary>
    public static class ProtoJsonConverter
    {
        public static List<string> CompiledClassNames = new List<string>();
        private const string c_classIdentifier = "public partial class ";

        public static void GeneratePersistentProto()
        {
            SaveManager.GenerateCSharpFromProto(ToolsetConstants.s_pathToProtoModelSourceDirectory, ToolsetConstants.s_pathToProtoModelGeneratedDirectory);
        }

        [DidReloadScripts]
        public static void OnReload()
        {
            List<string> modelContents =
                SaveManager.LoadAllFileContentsFromDirectory(ToolsetConstants.s_pathToProtoModelGeneratedDirectory, "*.cs");
            
            CompiledClassNames = new List<string>();
            for (int i = 0; i < modelContents.Count; ++i)
            {
                CompiledClassNames = GetAllClassNamesForFile(modelContents[i], CompiledClassNames);
            }
        }
        
        public static void SerializeGenerateModelToJson(string className)
        {
            Type generatedType = Type.GetType(className);
            if (generatedType == null)
                return;
            
            var instance = Activator.CreateInstance(generatedType);

            string directoryPath = ToolsetConstants.s_pathToJsonInstanceDirectory.StringBuilderAppend("/", className);

            Directory.CreateDirectory(directoryPath);
            IEnumerable<string> filePaths = Directory.EnumerateFiles(directoryPath, "*.json");

            string classNameWithSuffix = className.StringBuilderAppend("_", filePaths.Count(), ".json");

            SaveManager.SerializeObjectAsJsonAndSave(classNameWithSuffix, directoryPath, instance);
        }

        private static List<string> GetAllClassNamesForFile(string rawText, List<string> classNames)
        {
            int index = rawText.IndexOf(c_classIdentifier);

            if (index == -1)
                return classNames;

            rawText = rawText.Remove(0, index + c_classIdentifier.Length).TrimStart();

            int spaceIndex = rawText.IndexOf(' ');
            classNames.Add(rawText.Substring(0, spaceIndex));

            rawText = rawText.Remove(0, spaceIndex);

            return GetAllClassNamesForFile(rawText, classNames);
        }
    }
}

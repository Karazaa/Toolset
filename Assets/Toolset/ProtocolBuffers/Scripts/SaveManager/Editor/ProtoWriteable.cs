using System.Collections.Generic;
using System;
using System.IO;
using Toolset.Core;
using Toolset.Core.EditorTools;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// An abstract class representing some proto definition file. Implemented by proto definition editors.
    /// </summary>
    public abstract class ProtoWriteable : ToolsetEditorGUI
    {
        protected string ClassName { get; set; } = "NewProtoDefinition";
        protected List<ProtoField> Fields { get; set; } = new List<ProtoField>();

        private const string c_protoFileName = "{0}.proto";
        private const string c_protoFileHeader = "syntax = \"proto3\";\nimport \"ToolsetGuid.proto\";\n\nmessage {0} {1}";
        private const string c_protoField = "  {0} {1} = {2};\n";
        private const string c_protoFileFooter = "}";
        private const string c_protoClassWord = "message";
        private const int c_protoFileLineMinLength = 5;

        protected ProtoWriteable()
        {
            Fields.Add(new ProtoField(ProtoFieldEnum.Guid, ToolsetEditorConstants.c_protoGuidFieldName));
        }
        
        protected ProtoWriteable(string fileContent)
        {
            string[] lines = fileContent.Split('\n');
            
            if (lines.Length < c_protoFileLineMinLength)
                throw new InvalidOperationException("[Toolset.ProtoWriteable] Could not parse Proto Definition from file contents, the file contains {0} lines, but there must be at least {1}"
                    .StringBuilderFormat(lines.Length, c_protoFileLineMinLength));
            
            int classNameLine = 0;
            for (int i = 0; i < lines.Length - 1; ++i)
            {
                if (lines[i].StartsWith(c_protoClassWord))
                {
                    ClassName = lines[i].Split(' ')[1];
                    classNameLine = i;
                }
                
                if (classNameLine != 0 && i > classNameLine)
                    Fields.Add(new ProtoField(lines[i]));
            }
        }
        
        /// <summary>
        /// Gets all of the contents of the proto file as a single string for writing to disk.
        /// </summary>
        /// <returns>All of the contents of a single proto file.</returns>
        public virtual string GetProtoFileContents()
        {
            string contents = c_protoFileHeader.StringBuilderFormat(ClassName, "{\n");

            for (int i = 0; i < Fields.Count; ++i)
            {
                contents = contents.StringBuilderAppend(c_protoField.StringBuilderFormat
                (
                    Fields[i].FieldType.GetAsProtoTypeString(),
                    Fields[i].FieldName,
                    i + 1
                ));
            }

            return contents.StringBuilderAppend(c_protoFileFooter);
        }
        
        /// <summary>
        /// TODO: Fill me out
        /// </summary>
        /// <returns></returns>
        public virtual void Save()
        {
            string fileName = c_protoFileName.StringBuilderFormat(ClassName);
            SaveManager.SaveContentsToFile(fileName, ToolsetEditorConstants.s_pathToProtoDataDirectory, GetProtoFileContents());
            Debug.Log("Saved new Proto Definition: ".StringBuilderAppend(fileName));
        }

        /// <summary>
        /// TODO: Fill me out
        /// </summary>
        public virtual void Delete()
        {
            SaveManager.DeleteFileAndMeta(Path.Combine(ToolsetEditorConstants.s_pathToProtoDataDirectory,
                c_protoFileName.StringBuilderFormat(ClassName)));
        }
    }
}

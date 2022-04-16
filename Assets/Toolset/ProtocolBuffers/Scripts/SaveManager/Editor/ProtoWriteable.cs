using System.Collections;
using System.Collections.Generic;
using Toolset.Core;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public abstract class ProtoWriteable
    {
        protected string ClassName { get; set; } = "NewProtoDefinition";
        protected List<ProtoField> Fields { get; set; } = new List<ProtoField>();

        private const string c_protoFileName = "{0}.proto";
        private const string c_protoFileHeader = "syntax = \"proto3\";\n\nmessage {0} {1}";
        private const string c_protoField = "  {0} {1} = {2};\n";
        private const string c_protoFileFooter = "}";

        public virtual string GetProtoFileName()
        {
            return c_protoFileName.StringBuilderFormat(ClassName);
        }
        
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
    }
}

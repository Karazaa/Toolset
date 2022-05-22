using System.Collections.Generic;
using Toolset.Core;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// An abstract class representing some proto definition file. Implemented by proto definition editors.
    /// </summary>
    public abstract class ProtoWriteable
    {
        protected string ClassName { get; set; } = "NewProtoDefinition";
        protected List<ProtoField> Fields { get; set; } = new List<ProtoField>();

        private const string c_protoFileName = "{0}.proto";
        private const string c_protoFileHeader = "syntax = \"proto3\";\n\nmessage {0} {1}";
        private const string c_protoField = "  {0} {1} = {2};\n";
        private const string c_protoFileFooter = "}";

        /// <summary>
        /// Gets the file name for the proto definition.
        /// </summary>
        /// <returns>The .proto file name.</returns>
        public virtual string GetProtoFileName()
        {
            return c_protoFileName.StringBuilderFormat(ClassName);
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
    }
}

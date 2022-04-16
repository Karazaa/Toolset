using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public abstract class ProtoWriteable
    {
        protected string ClassName { get; set; } = "NewProtoDefinition";
        protected List<ProtoField> Fields { get; set; } = new List<ProtoField>();

        protected virtual string GetProtoFileContents()
        {
            string contents = string.Empty;

            return contents;
        }
    }
}

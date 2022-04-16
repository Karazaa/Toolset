using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    public class ProtoField
    {
        public string FieldName { get; set; } = "NewFieldName";
        public ProtoFieldEnum FieldType { get; set; }
    }
    
    public enum ProtoFieldEnum
    {
        String,
        Bool,
        Bytes,
        Double,
        Float,
        Int32,
        Int64,
        UInt32,
        UInt64,
        SInt32,
        SInt64,
        Fixed32,
        Fixed64,
        SFixed32,
        SFixed64,
    }
}

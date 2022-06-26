using System;
using System.Security.Permissions;
using UnityEngine;

namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// A struct that defines field name and field type for a proto field.
    /// </summary>
    public class ProtoField
    {
        public ProtoField() { }

        public ProtoField(string lineContent)
        {
            string[] tokens = lineContent.Split(' ');
            
            for (int i = 0; i < tokens.Length; ++i)
            {
                if (tokens[i] != String.Empty)
                {
                    FieldType = ProtoFieldExtensions.GetProtoFieldEnumFromString(tokens[i]);

                    if(i + 1 >= tokens.Length)
                        throw new InvalidOperationException("[Toolset.ProtoWriteable] Could not parse Proto Field from line contents! Proto file has invalid syntax.");
                    
                    FieldName = tokens[i + 1];
                    return;
                }
            }
            
            throw new InvalidOperationException("[Toolset.ProtoWriteable] Could not parse Proto Field from line contents! Proto file has invalid syntax.");
        }
        
        public string FieldName { get; set; } = "NewFieldName";
        public ProtoFieldEnum FieldType { get; set; }
    }
    
    /// <summary>
    /// An enum that defines all possible proto types in the Proto3 syntax
    /// </summary>
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

    /// <summary>
    /// Extensions for converting ProtoFieldEnum values into proper string representations.
    /// </summary>
    public static class ProtoFieldExtensions
    {
        public static string GetAsProtoTypeString(this ProtoFieldEnum enumValue)
        {
            switch (enumValue)
            {
                case ProtoFieldEnum.String:
                    return "string";
                case ProtoFieldEnum.Bool:
                    return "bool";
                case ProtoFieldEnum.Bytes:
                    return "bytes";
                case ProtoFieldEnum.Double:
                    return "double";
                case ProtoFieldEnum.Float:
                    return "float";
                case ProtoFieldEnum.Int32:
                    return "int32";
                case ProtoFieldEnum.Int64:
                    return "int64";
                case ProtoFieldEnum.UInt32:
                    return "uint32";
                case ProtoFieldEnum.UInt64:
                    return "uint64";
                case ProtoFieldEnum.SInt32:
                    return "sint32";
                case ProtoFieldEnum.SInt64:
                    return "sint64";
                case ProtoFieldEnum.Fixed32:
                    return "fixed32";
                case ProtoFieldEnum.Fixed64:
                    return "fixed64";
                case ProtoFieldEnum.SFixed32:
                    return "sfixed32";
                case ProtoFieldEnum.SFixed64:
                    return "sfixed64";
            }

            return null;
        }

        public static ProtoFieldEnum GetProtoFieldEnumFromString(string value)
        {
            switch (value)
            {
                case "string":
                    return ProtoFieldEnum.String;
                case "bool":
                    return ProtoFieldEnum.Bool;
                case "bytes":
                    return ProtoFieldEnum.Bytes;
                case "double":
                    return ProtoFieldEnum.Double;
                case "float":
                    return ProtoFieldEnum.Float;
                case "int32":
                    return ProtoFieldEnum.Int32;
                case "int64":
                    return ProtoFieldEnum.Int64;
                case "uint32":
                    return ProtoFieldEnum.UInt32;
                case "uint64":
                    return ProtoFieldEnum.UInt64;
                case "sint32":
                    return ProtoFieldEnum.SInt32;
                case "sint64":
                    return ProtoFieldEnum.SInt64;
                case "fixed32":
                    return ProtoFieldEnum.Fixed32;
                case "fixed64":
                    return ProtoFieldEnum.Fixed64;
                case "sfixed32":
                    return ProtoFieldEnum.SFixed32;
                case "sfixed64":
                    return ProtoFieldEnum.SFixed64;
            }

            return ProtoFieldEnum.Bytes;
        }
    }
}

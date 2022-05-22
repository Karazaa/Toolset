namespace Toolset.ProtocolBuffers.StaticDataEditor
{
    /// <summary>
    /// A struct that defines field name and field type for a proto field.
    /// </summary>
    public class ProtoField
    {
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
    }
}

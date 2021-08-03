using ProtoBuf;
using System.Collections.Generic;

/// <summary>
/// Example model for Protobuf serialization/deserialization.
/// </summary>
[ProtoContract]
public class ExampleProtobufModel
{
    [ProtoMember(1)]
    public int ExampleInt { get; set; }
    [ProtoMember(2)]
    public string ExampleString { get; set; }
    [ProtoMember(3)]
    public ExampleInternalProtobufModel ExampleInternalModel { get; set; }
    [ProtoMember(4)]
    public List<int> ExampleIntList { get; set; }
}

/// <summary>
/// Example model for Protobuf serialization/deserialization inside of another .
/// </summary>
[ProtoContract]
public class ExampleInternalProtobufModel
{
    [ProtoMember(1)]
    public string ExampleString1 { get; set; }
    [ProtoMember(2)]
    public string ExampleString2 { get; set; }
    [ProtoMember(3)]
    public string ExampleString3 { get; set; }
}

/// <summary>
/// Example model that doesn't have proper attributes associated with it.
/// </summary>
public class ExampleFaultyProtobufModel
{

}

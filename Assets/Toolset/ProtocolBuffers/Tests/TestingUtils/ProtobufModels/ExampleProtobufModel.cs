using ProtoBuf;

[ProtoContract]
public class ExampleProtobufModel
{
    [ProtoMember(1)]
    public int ExampleInt { get; set; }
    [ProtoMember(2)]
    public string ExampleString { get; set; }
    [ProtoMember(3)]
    public ExampleInternalProtobufModel ExampleInternalModel { get; set; }
}

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

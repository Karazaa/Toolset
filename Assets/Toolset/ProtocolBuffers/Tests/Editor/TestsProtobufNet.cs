using NUnit.Framework;
using ProtoBuf;
using System.IO;

public class TestsProtobufNet
{
    private static string c_testFilePath = "Assets/Toolset/ProtocolBuffers/Tests/Editor/TestsProtobufNet.bin";

    [Test]
    public void TestSerializeAndDeserialize()
    {
        FileStream fileStream = File.Create(c_testFilePath);

        ExampleProtobufModel modelToSerialize = new ExampleProtobufModel()
        {
            ExampleInt = 123,
            ExampleString = "This is the string I want to save.",
            ExampleInternalModel = new ExampleInternalProtobufModel()
            {
                ExampleString1 = "This is the first internal string.",
                ExampleString2 = "This is the second internal string.",
                ExampleString3 = "This is the third internal string.",
            }
        };

        Serializer.Serialize(fileStream, modelToSerialize);

        fileStream.Close();

        Assert.IsTrue(File.Exists(c_testFilePath));

        fileStream = File.OpenRead(c_testFilePath);

        ExampleProtobufModel modelToDeserialize;
        modelToDeserialize = Serializer.Deserialize<ExampleProtobufModel>(fileStream);

        fileStream.Close();

        Assert.AreEqual(modelToSerialize.ExampleInt, modelToDeserialize.ExampleInt);
        Assert.AreEqual(modelToSerialize.ExampleString, modelToDeserialize.ExampleString);
        Assert.AreEqual(modelToSerialize.ExampleInternalModel.ExampleString1, modelToDeserialize.ExampleInternalModel.ExampleString1);
        Assert.AreEqual(modelToSerialize.ExampleInternalModel.ExampleString2, modelToDeserialize.ExampleInternalModel.ExampleString2);
        Assert.AreEqual(modelToSerialize.ExampleInternalModel.ExampleString3, modelToDeserialize.ExampleInternalModel.ExampleString3);
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(c_testFilePath);
    }
}

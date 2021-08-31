using NUnit.Framework;
using ProtoBuf;
using System.IO;
using System.Collections.Generic;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit tests used to validate the protobuf-net dll.
    /// </summary>
    public class TestsProtoBufNet
    {
        private static string c_testFilePath = "Assets/Toolset/ProtocolBuffers/Tests/Editor/ProtobufNet/TestsProtobufNet.tso";

        [Test]
        public void TestSerializeAndDeserialize()
        {
            FileStream fileStream = File.Create(c_testFilePath);

            ExampleProtoBufModel modelToSerialize = new ExampleProtoBufModel()
            {
                ExampleInt = 123,
                ExampleString = "This is the string I want to save.",
                ExampleInternalModel = new ExampleInternalProtoBufModel()
                {
                    ExampleString1 = "This is the first internal string.",
                    ExampleString2 = "This is the second internal string.",
                    ExampleString3 = "This is the third internal string.",
                },
                ExampleIntList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };

            Serializer.Serialize(fileStream, modelToSerialize);

            fileStream.Close();

            Assert.IsTrue(File.Exists(c_testFilePath));

            fileStream = File.OpenRead(c_testFilePath);

            ExampleProtoBufModel modelToDeserialize;
            modelToDeserialize = Serializer.Deserialize<ExampleProtoBufModel>(fileStream);

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
            if (File.Exists(c_testFilePath))
                File.Delete(c_testFilePath);
        }
    }
}
using NUnit.Framework;
using Toolset.Core.Tests;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit tests used to validate the ProtoBufUtils class.
    /// </summary>
    public class TestsProtoBufUtils
    {
        [Test]
        public void TestIsSerializableProtoBuf()
        {
            Assert.IsTrue(ProtoBufUtils.IsSerializableProtoBuf(typeof(ExampleProtoBufModel)));
            Assert.IsTrue(ProtoBufUtils.IsSerializableProtoBuf(typeof(ExampleInternalProtoBufModel)));
            Assert.IsTrue(ProtoBufUtils.IsSerializableProtoBuf(typeof(ExamplePersistentProto)));
            Assert.IsFalse(ProtoBufUtils.IsSerializableProtoBuf(typeof(ExampleFaultyProtoBufModel)));
            Assert.IsFalse(ProtoBufUtils.IsSerializableProtoBuf(null));
        }

        [Test]
        public void TestSerializeAndDeserialize()
        {
            ExampleProtoBufModel model = ProtoTestingUtils.GenerateRandomValidProtobuf();
            byte[] modelBytes = ProtoBufUtils.Serialize(model);
            ExampleProtoBufModel deserializedModel = ProtoBufUtils.Deserialize<ExampleProtoBufModel>(modelBytes);
            ProtoTestingUtils.AssertModelsAreEqual(model, deserializedModel);

            ExamplePersistentProto generatedModel = ProtoTestingUtils.GenerateRandomPersistentProto();
            byte[] generatedModelBytes = ProtoBufUtils.Serialize(generatedModel);
            ExamplePersistentProto deserializedGeneratedModel = ProtoBufUtils.Deserialize<ExamplePersistentProto>(generatedModelBytes);
            ProtoTestingUtils.AssertGeneratedModelsAreEqual(generatedModel, deserializedGeneratedModel);

            Assert.IsNull(ProtoBufUtils.Serialize<ExampleProtoBufModel>(null));
            Assert.IsNull(ProtoBufUtils.Serialize(new ExampleFaultyProtoBufModel()));

            Assert.IsNull(ProtoBufUtils.Deserialize<ExampleProtoBufModel>(null));
            Assert.IsNull(ProtoBufUtils.Deserialize<ExampleFaultyProtoBufModel>(modelBytes));

            // Use a ThrowsAny since we don't have access to ProtoBufNet's exception class for some reason.
            ToolsetAssert.ThrowsAny(() =>
            {
                ProtoBufUtils.Deserialize<ExampleProtoBufModel>(generatedModelBytes);
            });
        }
    }
}

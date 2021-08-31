using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Static class of some utility methods that assist with testing ProtoBufNet classes.
    /// </summary>
    public static class ProtoTestingUtils
    {
        public static void AssertModelsAreEqual(ExampleProtoBufModel expected, ExampleProtoBufModel actual)
        {
            Assert.AreEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreEqual(expected.ExampleInternalModel.ExampleString1, actual.ExampleInternalModel.ExampleString1);
            Assert.AreEqual(expected.ExampleInternalModel.ExampleString2, actual.ExampleInternalModel.ExampleString2);
            Assert.AreEqual(expected.ExampleInternalModel.ExampleString3, actual.ExampleInternalModel.ExampleString3);
        }

        public static void AssertGeneratedModelsAreEqual(ExamplePersistentProto expected, ExamplePersistentProto actual)
        {
            Assert.AreEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreEqual(expected.LastUpdated, actual.LastUpdated);
        }

        public static void AssertModelsAreNotEqual(ExampleProtoBufModel expected, ExampleProtoBufModel actual)
        {
            Assert.AreNotEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreNotEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString1, actual.ExampleInternalModel.ExampleString1);
            Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString2, actual.ExampleInternalModel.ExampleString2);
            Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString3, actual.ExampleInternalModel.ExampleString3);
        }

        public static void AssertGeneratedModelsAreNotEqual(ExamplePersistentProto expected, ExamplePersistentProto actual)
        {
            Assert.AreNotEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreNotEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreNotEqual(expected.LastUpdated, actual.LastUpdated);
        }

        public static ExampleProtoBufModel GenerateRandomValidProtobuf()
        {
            Random random = new Random();
            ExampleProtoBufModel output = new ExampleProtoBufModel()
            {
                ExampleInt = random.Next(),
                ExampleString = random.Next().ToString(),
                ExampleInternalModel = new ExampleInternalProtoBufModel()
                {
                    ExampleString1 = random.Next().ToString(),
                    ExampleString2 = random.Next().ToString(),
                    ExampleString3 = random.Next().ToString()
                },
                ExampleIntList = new List<int>() { }
            };

            for (int i = 0; i < random.Next(1, 10); ++i)
            {
                output.ExampleIntList.Add(random.Next());
            }

            return output;
        }

        public static ExamplePersistentProto GenerateRandomPersistentProto()
        {
            Random random = new Random();
            ExamplePersistentProto output = new ExamplePersistentProto()
            {
                ExampleInt = random.Next(),
                ExampleString = random.Next().ToString(),
                LastUpdated = DateTime.Now.AddSeconds(random.Next(1, 10000))
            };

            return output;
        }
    }
}

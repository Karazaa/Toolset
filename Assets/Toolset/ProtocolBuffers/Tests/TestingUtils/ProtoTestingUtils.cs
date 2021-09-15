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
        public static readonly List<string> m_batchModelNames = new List<string>() { "example1", "example2", "example3", "example4", "example5" };
        public static readonly List<string> m_batchModelNamesWithSubdirectory = new List<string>() { "sub_directory/example1", "sub_directory/example2", "sub_directory/example3", "sub_directory/example4", "sub_directory/example5" };
        
        public static readonly ExampleProtoBufModel m_staticModel1 = new ExampleProtoBufModel()
        {
            ExampleInt = 456,
            ExampleString = "Some random words.",
            ExampleInternalModel = new ExampleInternalProtoBufModel()
            {
                ExampleString1 = "string 1",
                ExampleString2 = "string 2",
                ExampleString3 = "string 3"
            },
            ExampleIntList = new List<int>() { 9, 8, 7, 6, 5, 4, 3, 2, 1 }
        };
        public static readonly ExampleProtoBufModel m_staticModel2 = new ExampleProtoBufModel()
        {
            ExampleInt = 789,
            ExampleString = "More random words",
            ExampleInternalModel = new ExampleInternalProtoBufModel()
            {
                ExampleString1 = "string A",
                ExampleString2 = "string B",
                ExampleString3 = "string C"
            },
            ExampleIntList = new List<int>() { 4, 4, 4, 4 }
        };

        public static readonly ExamplePersistentProto m_staticGeneratedModel1 = new ExamplePersistentProto()
        {
            ExampleString = "Hello",
            ExampleInt = 2,
            LastUpdated = DateTime.Now,
        };
        public static readonly ExamplePersistentProto m_staticGeneratedModel2 = new ExamplePersistentProto()
        {
            ExampleString = "Goodbye",
            ExampleInt = 999,
            LastUpdated = DateTime.Now.AddDays(10),
        };

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

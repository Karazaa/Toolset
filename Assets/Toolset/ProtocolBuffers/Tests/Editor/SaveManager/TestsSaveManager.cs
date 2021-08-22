using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit tests used to validate the SaveManager class.
    /// </summary>
    public class TestsSaveManager
    {
        private Random m_random;
        private readonly List<string> m_batchModelNames = new List<string>() { "example1", "example2", "example3", "example4", "example5" };
        private readonly List<string> m_batchModelNamesWithSubdirectory = new List<string>() { "sub_directory/example1", "sub_directory/example2", "sub_directory/example3", "sub_directory/example4", "sub_directory/example5" };
        private readonly string m_pathToProtoSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/ProtoFiles";
        private readonly string m_pathToProtoGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/Generated";

        private const string c_exampleProtoFileName = "addressbook";
        private const string c_expectedGeneratedCSharpFileName = "addressbook.cs";
        private const string c_secondExpectedGeneratedCSharpFileName = "exampleproto.cs";

        [SetUp]
        public void SetUp()
        {
            m_random = new Random();
        }

        [Test]
        public void TestLoadWithoutSaveFile()
        {
            ExampleProtobufModel nullModel = SaveManager.LoadModel<ExampleProtobufModel>("somefakefilepath");

            Assert.IsNull(nullModel);
        }

        [Test]
        public void TestSaveFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.SaveModel("Faulty", new ExampleFaultyProtobufModel());
            });
        }

        [Test]
        public void TestLoadFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.LoadModel<ExampleFaultyProtobufModel>("Faulty");
            });
        }

        [Test]
        public void TestLoadByTypeFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.LoadModelsByType<ExampleFaultyProtobufModel>();
            });
        }

        [Test]
        public void TestDeleteFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.DeleteModel<ExampleFaultyProtobufModel>("Faulty");
            });
        }

        [Test]
        public void TestDeleteByTypeFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.DeleteModelsByType<ExampleFaultyProtobufModel>();
            });
        }

        [Test]
        public void TestSaveInvalidFileName()
        {
            AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.SaveModel(callbackParameter, new ExampleProtobufModel());
            });
        }

        [Test]
        public void TestLoadInvalidFileName()
        {
            AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.LoadModel<ExampleProtobufModel>(callbackParameter);
            });
        }

        [Test]
        public void TestDeleteInvalidFileName()
        {
            AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.DeleteModel<ExampleProtobufModel>(callbackParameter);
            });
        }

        [Test]
        public void TestSaveAndLoad()
        {
            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestOverwriteAndLoad. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            ExampleProtobufModel modelToSave = new ExampleProtobufModel()
            {
                ExampleInt = 456,
                ExampleString = "Some random words.",
                ExampleInternalModel = new ExampleInternalProtobufModel()
                {
                    ExampleString1 = "string 1",
                    ExampleString2 = "string 2",
                    ExampleString3 = "string 3"
                },
                ExampleIntList = new List<int>() { 9, 8, 7, 6, 5, 4, 3, 2, 1 }
            };

            SaveManager.SaveModel(m_batchModelNames[0], modelToSave);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[0])));

            ExampleProtobufModel modelToLoad = SaveManager.LoadModel<ExampleProtobufModel>(m_batchModelNames[0]);

            AssertModelsAreEqual(modelToSave, modelToLoad);

            ExamplePersistentProto exampleGeneratedProto = new ExamplePersistentProto()
            {
                ExampleString = "Hello",
                ExampleInt = 2,
                LastUpdated = DateTime.Now,
            };

            SaveManager.SaveModel(m_batchModelNamesWithSubdirectory[0], exampleGeneratedProto);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0])));

            ExamplePersistentProto generatedModelToLoad = SaveManager.LoadModel<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0]);

            AssertGeneratedModelsAreEqual(exampleGeneratedProto, generatedModelToLoad);
        }

        [Test]
        public void TestOverwriteAndLoad()
        {
            TestSaveAndLoad();

            ExampleProtobufModel preSaveLoadedModel = SaveManager.LoadModel<ExampleProtobufModel>(m_batchModelNames[0]);
            ExamplePersistentProto preSaveGeneratedLoadedModel = SaveManager.LoadModel<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0]);

            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestSaveAndLoad. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            ExampleProtobufModel modelToSave = new ExampleProtobufModel()
            {
                ExampleInt = 789,
                ExampleString = "More random words",
                ExampleInternalModel = new ExampleInternalProtobufModel()
                {
                    ExampleString1 = "string A",
                    ExampleString2 = "string B",
                    ExampleString3 = "string C"
                },
                ExampleIntList = new List<int>() { 4, 4, 4, 4 }
            };

            AssertModelsAreNotEqual(modelToSave, preSaveLoadedModel);

            SaveManager.SaveModel(m_batchModelNames[0], modelToSave);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[0])));

            ExampleProtobufModel modelToLoad = SaveManager.LoadModel<ExampleProtobufModel>(m_batchModelNames[0]);

            AssertModelsAreEqual(modelToSave, modelToLoad);

            ExamplePersistentProto generatedModelToSave = new ExamplePersistentProto()
            {
                ExampleString = "Goodbye",
                ExampleInt = 999,
                LastUpdated = DateTime.Now.AddDays(10),
            };

            AssertGeneratedModelsAreNotEqual(generatedModelToSave, preSaveGeneratedLoadedModel);

            SaveManager.SaveModel(m_batchModelNamesWithSubdirectory[0], generatedModelToSave);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0])));

            ExamplePersistentProto generatedModelToLoad = SaveManager.LoadModel<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0]);

            AssertGeneratedModelsAreEqual(generatedModelToSave, generatedModelToLoad);
        }

        [Test]
        public void TestLoadAll()
        {
            Dictionary<string, ExampleProtobufModel> modelsToSave = new Dictionary<string, ExampleProtobufModel>();

            for (int i = 0; i < m_batchModelNames.Count; ++i)
            {
                modelsToSave.Add(m_batchModelNames[i], GenerateRandomValidProtobuf());
            }

            foreach (KeyValuePair<string, ExampleProtobufModel> pair in modelsToSave)
            {
                SaveManager.SaveModel(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, ExampleProtobufModel> pair in modelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(pair.Key)));
            }

            Dictionary<string, ExampleProtobufModel> loadedModels = SaveManager.LoadModelsByType<ExampleProtobufModel>();

            foreach (KeyValuePair<string, ExampleProtobufModel> pair in loadedModels)
            {
                AssertModelsAreEqual(modelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }

            Dictionary<string, ExamplePersistentProto> generatedModelsToSave = new Dictionary<string, ExamplePersistentProto>();

            for (int i = 0; i < m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                generatedModelsToSave.Add(m_batchModelNamesWithSubdirectory[i], GenerateRandomPersistentProto());
            }

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedModelsToSave)
            {
                SaveManager.SaveModel(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedModelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(pair.Key)));
            }

            Dictionary<string, ExamplePersistentProto> generatedLoadedModels = SaveManager.LoadModelsByType<ExamplePersistentProto>();

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedLoadedModels)
            {
                AssertGeneratedModelsAreEqual(generatedModelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }
        }

        [Test]
        public void TestDelete()
        {
            SaveManager.SaveModel(m_batchModelNames[0], GenerateRandomValidProtobuf());

            string filePath = SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[0]);
            Assert.IsTrue(File.Exists(filePath));

            SaveManager.DeleteModel<ExampleProtobufModel>(m_batchModelNames[0]);
            Assert.IsFalse(File.Exists(filePath));

            SaveManager.SaveModel(m_batchModelNamesWithSubdirectory[0], GenerateRandomPersistentProto());

            string persistentFilePath = SaveManager.GetDataFilePathForType<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0]);
            Assert.IsTrue(File.Exists(persistentFilePath));

            SaveManager.DeleteModel<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[0]);
            Assert.IsFalse(File.Exists(persistentFilePath));
        }

        [Test]
        public void TestDeleteAllByType()
        {
            for (int i = 0; i < m_batchModelNames.Count; ++i)
            {
                SaveManager.SaveModel(m_batchModelNames[i], GenerateRandomValidProtobuf());
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[i])));
            }

            SaveManager.DeleteModelsByType<ExampleProtobufModel>();

            for (int i = 0; i < m_batchModelNames.Count; ++i)
            {
                Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[i])));
            }

            for (int i = 0; i < m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                SaveManager.SaveModel(m_batchModelNamesWithSubdirectory[i], GenerateRandomPersistentProto());
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[i])));
            }

            SaveManager.DeleteModelsByType<ExamplePersistentProto>();

            for (int i = 0; i < m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[i])));
            }
        }

        [Test]
        public void TestGenerateSingleCSharpFromProto()
        {
            SaveManager.GenerateSingleCSharpFromProto(m_pathToProtoSourceDirectory, m_pathToProtoGeneratedDirectory, c_exampleProtoFileName, false);
            Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName)));
        }

        [Test]
        public void TestGenerateCSharpFromProto()
        {
            SaveManager.GenerateCSharpFromProto(m_pathToProtoSourceDirectory, m_pathToProtoGeneratedDirectory, false);
            Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName)));
            Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_secondExpectedGeneratedCSharpFileName)));
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < m_batchModelNames.Count; ++i)
            {
                SaveManager.DeleteFileAndMeta(SaveManager.GetDataFilePathForType<ExampleProtobufModel>(m_batchModelNames[i]));
            }

            for (int i = 0; i < m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                SaveManager.DeleteFileAndMeta(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(m_batchModelNamesWithSubdirectory[i]));
            }

            SaveManager.DeleteDirectoryAndMetaRecursively(SaveManager.GetDataDirectoryPathForType<ExampleProtobufModel>());
            SaveManager.DeleteDirectoryAndMetaRecursively(SaveManager.GetDataDirectoryPathForType<ExamplePersistentProto>());

            if (Directory.Exists(m_pathToProtoGeneratedDirectory))
            {
                SaveManager.DeleteFileAndMeta(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName));
                SaveManager.DeleteFileAndMeta(Path.Combine(m_pathToProtoGeneratedDirectory, c_secondExpectedGeneratedCSharpFileName));
                SaveManager.DeleteDirectoryAndMetaRecursively(m_pathToProtoGeneratedDirectory);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            AssetDatabase.Refresh();
        }

        [MenuItem("Toolset/Testing/Generate Persistent Proto")]
        public static void GeneratePersistentProto()
        {
            string pathToPersistentProtoSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/PersistentTesting/ProtoFiles";
            string pathToPersistentProtoGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/PersistentTesting/Generated";
            SaveManager.GenerateCSharpFromProto(pathToPersistentProtoSourceDirectory, pathToPersistentProtoGeneratedDirectory);
        }

        private void AssertModelsAreEqual(ExampleProtobufModel expected, ExampleProtobufModel actual)
        {
            Assert.AreEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreEqual(expected.ExampleInternalModel.ExampleString1, actual.ExampleInternalModel.ExampleString1);
            Assert.AreEqual(expected.ExampleInternalModel.ExampleString2, actual.ExampleInternalModel.ExampleString2);
            Assert.AreEqual(expected.ExampleInternalModel.ExampleString3, actual.ExampleInternalModel.ExampleString3);
        }

        private void AssertGeneratedModelsAreEqual(ExamplePersistentProto expected, ExamplePersistentProto actual)
        {
            Assert.AreEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreEqual(expected.LastUpdated, actual.LastUpdated);
        }

        private void AssertModelsAreNotEqual(ExampleProtobufModel expected, ExampleProtobufModel actual)
        {
            Assert.AreNotEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreNotEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString1, actual.ExampleInternalModel.ExampleString1);
            Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString2, actual.ExampleInternalModel.ExampleString2);
            Assert.AreNotEqual(expected.ExampleInternalModel.ExampleString3, actual.ExampleInternalModel.ExampleString3);
        }

        private void AssertGeneratedModelsAreNotEqual(ExamplePersistentProto expected, ExamplePersistentProto actual)
        {
            Assert.AreNotEqual(expected.ExampleInt, actual.ExampleInt);
            Assert.AreNotEqual(expected.ExampleString, actual.ExampleString);
            Assert.AreNotEqual(expected.LastUpdated, actual.LastUpdated);
        }

        private ExampleProtobufModel GenerateRandomValidProtobuf()
        {
            ExampleProtobufModel output = new ExampleProtobufModel()
            {
                ExampleInt = m_random.Next(),
                ExampleString = m_random.Next().ToString(),
                ExampleInternalModel = new ExampleInternalProtobufModel()
                {
                    ExampleString1 = m_random.Next().ToString(),
                    ExampleString2 = m_random.Next().ToString(),
                    ExampleString3 = m_random.Next().ToString()
                },
                ExampleIntList = new List<int>() { }
            };

            for (int i = 0; i < m_random.Next(1, 10); ++i)
            {
                output.ExampleIntList.Add(m_random.Next());
            }

            return output;
        }

        private ExamplePersistentProto GenerateRandomPersistentProto()
        {
            ExamplePersistentProto output = new ExamplePersistentProto()
            {
                ExampleInt = m_random.Next(),
                ExampleString = m_random.Next().ToString(),
                LastUpdated = DateTime.Now.AddSeconds(m_random.Next(1, 10000))
            };

            return output;
        }

        private void AssertExceptionsOnInvalidFileNames(Action<string> callbackToTest)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest(null);
            });

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest(string.Empty);
            });

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                callbackToTest("    ");
            });

            for (int i = 0; i < invalidPathChars.Length; ++i)
            {
                ToolsetAssert.Throws<InvalidOperationException>(() =>
                {
                    callbackToTest(invalidPathChars[i].ToString());
                });
            }
        }
    }
}
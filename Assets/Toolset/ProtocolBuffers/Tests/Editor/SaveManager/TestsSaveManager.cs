using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using Toolset.Global.Utils;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit tests used to validate the SaveManager class.
    /// </summary>
    public class TestsSaveManager
    {
        private Random m_random;
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
            ExampleProtoBufModel nullModel = SaveManager.LoadModel<ExampleProtoBufModel>("somefakefilepath");

            Assert.IsNull(nullModel);
        }

        [Test]
        public void TestSaveFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.SaveModel("Faulty", new ExampleFaultyProtoBufModel());
            });
        }

        [Test]
        public void TestLoadFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.LoadModel<ExampleFaultyProtoBufModel>("Faulty");
            });
        }

        [Test]
        public void TestLoadByTypeFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.LoadModelsByType<ExampleFaultyProtoBufModel>();
            });
        }

        [Test]
        public void TestDeleteFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.DeleteModel<ExampleFaultyProtoBufModel>("Faulty");
            });
        }

        [Test]
        public void TestDeleteByTypeFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.DeleteModelsByType<ExampleFaultyProtoBufModel>();
            });
        }

        [Test]
        public void TestSaveInvalidFileName()
        {
            SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.SaveModel(callbackParameter, new ExampleProtoBufModel());
            });
        }

        [Test]
        public void TestLoadInvalidFileName()
        {
            SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.LoadModel<ExampleProtoBufModel>(callbackParameter);
            });
        }

        [Test]
        public void TestDeleteInvalidFileName()
        {
            SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.DeleteModel<ExampleProtoBufModel>(callbackParameter);
            });
        }

        [Test]
        public void TestSaveAndLoad()
        {
            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestOverwriteAndLoad. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.m_staticModel1);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0])));

            ExampleProtoBufModel modelToLoad = SaveManager.LoadModel<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);

            ProtoTestingUtils.AssertModelsAreEqual(ProtoTestingUtils.m_staticModel1, modelToLoad);

            SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.m_staticGeneratedModel1);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0])));

            ExamplePersistentProto generatedModelToLoad = SaveManager.LoadModel<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);

            ProtoTestingUtils.AssertGeneratedModelsAreEqual(ProtoTestingUtils.m_staticGeneratedModel1, generatedModelToLoad);
        }

        [Test]
        public void TestOverwriteAndLoad()
        {
            TestSaveAndLoad();

            ExampleProtoBufModel preSaveLoadedModel = SaveManager.LoadModel<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            ExamplePersistentProto preSaveGeneratedLoadedModel = SaveManager.LoadModel<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);

            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestSaveAndLoad. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            ProtoTestingUtils.AssertModelsAreNotEqual(ProtoTestingUtils.m_staticModel2, preSaveLoadedModel);

            SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.m_staticModel2);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0])));

            ExampleProtoBufModel modelToLoad = SaveManager.LoadModel<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);

            ProtoTestingUtils.AssertModelsAreEqual(ProtoTestingUtils.m_staticModel2, modelToLoad);

            ProtoTestingUtils.AssertGeneratedModelsAreNotEqual(ProtoTestingUtils.m_staticGeneratedModel2, preSaveGeneratedLoadedModel);

            SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.m_staticGeneratedModel2);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0])));

            ExamplePersistentProto generatedModelToLoad = SaveManager.LoadModel<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);

            ProtoTestingUtils.AssertGeneratedModelsAreEqual(ProtoTestingUtils.m_staticGeneratedModel2, generatedModelToLoad);
        }

        [Test]
        public void TestSaveAndLoadAll()
        {
            Dictionary<string, ExampleProtoBufModel> modelsToSave = new Dictionary<string, ExampleProtoBufModel>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNames.Count; ++i)
            {
                modelsToSave.Add(ProtoTestingUtils.m_batchModelNames[i], ProtoTestingUtils.GenerateRandomValidProtobuf());
            }

            SaveManager.SaveModelsByType(modelsToSave);

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in modelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(pair.Key)));
            }

            Dictionary<string, ExampleProtoBufModel> loadedModels = SaveManager.LoadModelsByType<ExampleProtoBufModel>();

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in loadedModels)
            {
                ProtoTestingUtils.AssertModelsAreEqual(modelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }

            Dictionary<string, ExamplePersistentProto> generatedModelsToSave = new Dictionary<string, ExamplePersistentProto>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                generatedModelsToSave.Add(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i], ProtoTestingUtils.GenerateRandomPersistentProto());
            }

            SaveManager.SaveModelsByType(generatedModelsToSave);

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedModelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(pair.Key)));
            }

            Dictionary<string, ExamplePersistentProto> generatedLoadedModels = SaveManager.LoadModelsByType<ExamplePersistentProto>();

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedLoadedModels)
            {
                ProtoTestingUtils.AssertGeneratedModelsAreEqual(generatedModelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }
        }

        [Test]
        public void TestDelete()
        {
            SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.GenerateRandomValidProtobuf());

            string filePath = SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            Assert.IsTrue(File.Exists(filePath));

            SaveManager.DeleteModel<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            Assert.IsFalse(File.Exists(filePath));

            SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.GenerateRandomPersistentProto());

            string persistentFilePath = SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);
            Assert.IsTrue(File.Exists(persistentFilePath));

            SaveManager.DeleteModel<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);
            Assert.IsFalse(File.Exists(persistentFilePath));
        }

        [Test]
        public void TestDeleteAllByType()
        {
            for (int i = 0; i < ProtoTestingUtils.m_batchModelNames.Count; ++i)
            {
                SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNames[i], ProtoTestingUtils.GenerateRandomValidProtobuf());
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[i])));
            }

            SaveManager.DeleteModelsByType<ExampleProtoBufModel>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNames.Count; ++i)
            {
                Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[i])));
            }

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                SaveManager.SaveModel(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i], ProtoTestingUtils.GenerateRandomPersistentProto());
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i])));
            }

            SaveManager.DeleteModelsByType<ExamplePersistentProto>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i])));
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
            SaveManager.DeleteModelsByType<ExampleProtoBufModel>();
            SaveManager.DeleteModelsByType<ExamplePersistentProto>();

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
    }
}
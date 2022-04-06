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
        private readonly string m_pathToProtoSourceDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/Editor/ProtoFiles";
        private readonly string m_pathToProtoGeneratedDirectory = UnityEngine.Application.dataPath + "/Toolset/ProtocolBuffers/Tests/TestingUtils/Editor/Generated";

        private const string c_exampleProtoFileName = "addressbook";
        private const string c_expectedGeneratedCSharpFileName = "addressbook.cs";
        private const string c_secondExpectedGeneratedCSharpFileName = "exampleproto.cs";
        private const string c_metaSuffix = ".meta";

        [Test]
        public void TestLoadWithoutSaveFile()
        {
            ExampleProtoBufModel nullModel = SaveManager.LoadModelInstance<ExampleProtoBufModel>("somefakefilepath");

            Assert.IsNull(nullModel);
        }

        [Test]
        public void TestSaveFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.SaveModelInstance("Faulty", new ExampleFaultyProtoBufModel());
            });
        }

        [Test]
        public void TestLoadFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.LoadModelInstance<ExampleFaultyProtoBufModel>("Faulty");
            });
        }

        [Test]
        public void TestLoadByTypeFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.LoadModelInstanceByType<ExampleFaultyProtoBufModel>();
            });
        }

        [Test]
        public void TestLoadModelsByTypeWithoutDirectory()
        {
            Dictionary<string, ExampleProtoBufModel> emptyDict = SaveManager.LoadModelInstanceByType<ExampleProtoBufModel>();

            Assert.AreEqual(0, emptyDict.Count);
        }

        [Test]
        public void TestDeleteWithoutSaveFile()
        {
            Assert.IsFalse(SaveManager.DeleteModelInstance<ExampleProtoBufModel>("somefakefilepath"));
        }

        [Test]
        public void TestDeleteFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.DeleteModelInstance<ExampleFaultyProtoBufModel>("Faulty");
            });
        }

        [Test]
        public void TestDeleteByTypeFaultyProtobufModel()
        {
            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                SaveManager.DeleteModelInstancesByType<ExampleFaultyProtoBufModel>();
            });
        }

        [Test]
        public void TestDeleteModelsByTypeWithoutDirectory()
        {
            Assert.IsFalse(SaveManager.DeleteModelInstancesByType<ExampleProtoBufModel>());
        }

        [Test]
        public void TestSaveInvalidFileName()
        {
            SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.SaveModelInstance(callbackParameter, new ExampleProtoBufModel());
            });
        }

        [Test]
        public void TestLoadInvalidFileName()
        {
            SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.LoadModelInstance<ExampleProtoBufModel>(callbackParameter);
            });
        }

        [Test]
        public void TestDeleteInvalidFileName()
        {
            SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNames((callbackParameter) =>
            {
                SaveManager.DeleteModelInstance<ExampleProtoBufModel>(callbackParameter);
            });
        }

        [Test]
        public void TestSaveAndLoad()
        {
            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestOverwriteAndLoad. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.m_staticModel1);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0])));

            ExampleProtoBufModel modelToLoad = SaveManager.LoadModelInstance<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);

            ProtoTestingUtils.AssertModelsAreEqual(ProtoTestingUtils.m_staticModel1, modelToLoad);

            SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.m_staticGeneratedModel1);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0])));

            ExamplePersistentProto generatedModelToLoad = SaveManager.LoadModelInstance<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);

            ProtoTestingUtils.AssertGeneratedModelsAreEqual(ProtoTestingUtils.m_staticGeneratedModel1, generatedModelToLoad);
        }

        [Test]
        public void TestOverwriteAndLoad()
        {
            TestSaveAndLoad();

            ExampleProtoBufModel preSaveLoadedModel = SaveManager.LoadModelInstance<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            ExamplePersistentProto preSaveGeneratedLoadedModel = SaveManager.LoadModelInstance<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);

            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestSaveAndLoad. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            ProtoTestingUtils.AssertModelsAreNotEqual(ProtoTestingUtils.m_staticModel2, preSaveLoadedModel);

            SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.m_staticModel2);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0])));

            ExampleProtoBufModel modelToLoad = SaveManager.LoadModelInstance<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);

            ProtoTestingUtils.AssertModelsAreEqual(ProtoTestingUtils.m_staticModel2, modelToLoad);

            ProtoTestingUtils.AssertGeneratedModelsAreNotEqual(ProtoTestingUtils.m_staticGeneratedModel2, preSaveGeneratedLoadedModel);

            SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.m_staticGeneratedModel2);

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0])));

            ExamplePersistentProto generatedModelToLoad = SaveManager.LoadModelInstance<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);

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

            SaveManager.SaveModelInstancesByType(modelsToSave);

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in modelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExampleProtoBufModel>(pair.Key)));
            }

            Dictionary<string, ExampleProtoBufModel> loadedModels = SaveManager.LoadModelInstanceByType<ExampleProtoBufModel>();

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in loadedModels)
            {
                ProtoTestingUtils.AssertModelsAreEqual(modelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }

            Dictionary<string, ExamplePersistentProto> generatedModelsToSave = new Dictionary<string, ExamplePersistentProto>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                generatedModelsToSave.Add(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i], ProtoTestingUtils.GenerateRandomPersistentProto());
            }

            SaveManager.SaveModelInstancesByType(generatedModelsToSave);

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedModelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExamplePersistentProto>(pair.Key)));
            }

            Dictionary<string, ExamplePersistentProto> generatedLoadedModels = SaveManager.LoadModelInstanceByType<ExamplePersistentProto>();

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedLoadedModels)
            {
                ProtoTestingUtils.AssertGeneratedModelsAreEqual(generatedModelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }
        }

        [Test]
        public void TestDelete()
        {
            SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.GenerateRandomValidProtobuf());

            string filePath = SaveManager.GetDataFilePathForModelType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            string metaPath = filePath + c_metaSuffix;
            File.Create(metaPath).Close();

            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(File.Exists(metaPath));

            Assert.IsTrue(SaveManager.DeleteModelInstance<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]));
            Assert.IsFalse(File.Exists(filePath));
            Assert.IsFalse(File.Exists(metaPath));

            SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.GenerateRandomPersistentProto());

            string persistentFilePath = SaveManager.GetDataFilePathForModelType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);
            string persistentMetaPath = persistentFilePath + c_metaSuffix;
            File.Create(persistentMetaPath).Close();

            Assert.IsTrue(File.Exists(persistentFilePath));
            Assert.IsTrue(File.Exists(persistentMetaPath));

            Assert.IsTrue(SaveManager.DeleteModelInstance<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]));
            Assert.IsFalse(File.Exists(persistentFilePath));
            Assert.IsFalse(File.Exists(persistentMetaPath));
        }

        [Test]
        public void TestDeleteAllByType()
        {
            for (int i = 0; i < ProtoTestingUtils.m_batchModelNames.Count; ++i)
            {
                SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNames[i], ProtoTestingUtils.GenerateRandomValidProtobuf());
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[i])));
            }

            string directoryMeta = SaveManager.GetDataDirectoryPathForModelType<ExampleProtoBufModel>() + c_metaSuffix;
            File.Create(directoryMeta).Close();
            Assert.IsTrue(File.Exists(directoryMeta));

            Assert.IsTrue(SaveManager.DeleteModelInstancesByType<ExampleProtoBufModel>());

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNames.Count; ++i)
            {
                Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForModelType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[i])));
            }
            Assert.IsFalse(File.Exists(directoryMeta));

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                SaveManager.SaveModelInstance(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i], ProtoTestingUtils.GenerateRandomPersistentProto());
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForModelType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i])));
            }

            string persistentDirectoryMeta = SaveManager.GetDataDirectoryPathForModelType<ExamplePersistentProto>() + c_metaSuffix;
            File.Create(persistentDirectoryMeta).Close();
            Assert.IsTrue(File.Exists(persistentDirectoryMeta));

            Assert.IsTrue(SaveManager.DeleteModelInstancesByType<ExamplePersistentProto>());

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                Assert.IsFalse(File.Exists(SaveManager.GetDataFilePathForModelType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i])));
            }
            Assert.IsFalse(File.Exists(persistentDirectoryMeta));
        }

        [Test]
        public void TestGenerateSingleCSharpFromProto()
        {
            SaveManager.GenerateSingleCSharpFromProto(m_pathToProtoSourceDirectory, m_pathToProtoGeneratedDirectory, c_exampleProtoFileName);
            Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName)));
        }

        [Test]
        public void TestGenerateCSharpFromProto()
        {
            SaveManager.GenerateCSharpFromProto(m_pathToProtoSourceDirectory, m_pathToProtoGeneratedDirectory);
            Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_expectedGeneratedCSharpFileName)));
            Assert.IsTrue(File.Exists(Path.Combine(m_pathToProtoGeneratedDirectory, c_secondExpectedGeneratedCSharpFileName)));
        }

        [TearDown]
        public void TearDown()
        {
            SaveManager.DeleteModelInstancesByType<ExampleProtoBufModel>();
            SaveManager.DeleteModelInstancesByType<ExamplePersistentProto>();

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
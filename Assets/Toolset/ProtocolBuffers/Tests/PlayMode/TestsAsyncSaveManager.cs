using NUnit.Framework;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using Toolset.Core;
using Toolset.Core.Tests;

namespace Toolset.ProtocolBuffers.Tests
{
    /// <summary>
    /// Class of unit/integration tests used to validate the async methods within the SaveManager class.
    /// </summary>
    public class TestsAsyncSaveManager
    {
        [UnityTest]
        public IEnumerator TestSaveAndLoadAsync()
        {
            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestOverwriteAndLoadAsync. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            yield return SaveManager.SaveModelAsync(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.m_staticModel1).GetAsIEnumerator();

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0])));

            Task<ExampleProtoBufModel> loadTask1 = SaveManager.LoadModelAsync<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            yield return loadTask1.GetAsIEnumerator();
            ExampleProtoBufModel modelToLoad = loadTask1.Result;

            ProtoTestingUtils.AssertModelsAreEqual(ProtoTestingUtils.m_staticModel1, modelToLoad);

            yield return SaveManager.SaveModelAsync(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.m_staticGeneratedModel1).GetAsIEnumerator();

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0])));

            Task<ExamplePersistentProto> loadTask2 = SaveManager.LoadModelAsync<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);
            yield return loadTask2.GetAsIEnumerator();
            ExamplePersistentProto generatedModelToLoad = loadTask2.Result;

            ProtoTestingUtils.AssertGeneratedModelsAreEqual(ProtoTestingUtils.m_staticGeneratedModel1, generatedModelToLoad);
        }

        [UnityTest]
        public IEnumerator TestOverwriteAndLoadAsync()
        {
            yield return TestSaveAndLoadAsync();

            // Note: We can't lean on GenerateRandomValidProtobuf for generation here, because we need to guarantee that 
            // this saved model has different values from the ones in TestSaveAndLoadAsync. Although generating these models
            // randomly has an exteremely low chance to populate the same values, it is best to avoid any potential false
            // fail edge case in these tests.
            Task<ExampleProtoBufModel> loadTask1 = SaveManager.LoadModelAsync<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            yield return loadTask1.GetAsIEnumerator();
            ExampleProtoBufModel preSaveLoadedModel = loadTask1.Result;

            Task<ExamplePersistentProto> loadTask2 = SaveManager.LoadModelAsync<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);
            yield return loadTask2.GetAsIEnumerator();
            ExamplePersistentProto preSaveGeneratedLoadedModel = loadTask2.Result;

            ProtoTestingUtils.AssertModelsAreNotEqual(ProtoTestingUtils.m_staticModel2, preSaveLoadedModel);

            yield return SaveManager.SaveModelAsync(ProtoTestingUtils.m_batchModelNames[0], ProtoTestingUtils.m_staticModel2).GetAsIEnumerator();

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0])));

            loadTask1 = SaveManager.LoadModelAsync<ExampleProtoBufModel>(ProtoTestingUtils.m_batchModelNames[0]);
            yield return loadTask1.GetAsIEnumerator();
            ExampleProtoBufModel modelToLoad = loadTask1.Result;

            ProtoTestingUtils.AssertModelsAreEqual(ProtoTestingUtils.m_staticModel2, modelToLoad);

            ProtoTestingUtils.AssertGeneratedModelsAreNotEqual(ProtoTestingUtils.m_staticGeneratedModel2, preSaveGeneratedLoadedModel);

            yield return SaveManager.SaveModelAsync(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0], ProtoTestingUtils.m_staticGeneratedModel2).GetAsIEnumerator();

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0])));

            loadTask2 = SaveManager.LoadModelAsync<ExamplePersistentProto>(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[0]);
            yield return loadTask2.GetAsIEnumerator();
            ExamplePersistentProto generatedModelToLoad = loadTask2.Result;

            ProtoTestingUtils.AssertGeneratedModelsAreEqual(ProtoTestingUtils.m_staticGeneratedModel2, generatedModelToLoad);
        }

        [UnityTest]
        public IEnumerator TestLoadAllAsync()
        {
            Dictionary<string, ExampleProtoBufModel> modelsToSave = new Dictionary<string, ExampleProtoBufModel>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNames.Count; ++i)
            {
                modelsToSave.Add(ProtoTestingUtils.m_batchModelNames[i], ProtoTestingUtils.GenerateRandomValidProtobuf());
            }

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in modelsToSave)
            {
                yield return SaveManager.SaveModelAsync(pair.Key, pair.Value).GetAsIEnumerator();
            }

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in modelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExampleProtoBufModel>(pair.Key)));
            }

            Task<Dictionary<string, ExampleProtoBufModel>> loadTask1 = SaveManager.LoadModelsByTypeAsync<ExampleProtoBufModel>();
            yield return loadTask1.GetAsIEnumerator();
            Dictionary<string, ExampleProtoBufModel>  loadedModels = loadTask1.Result;

            foreach (KeyValuePair<string, ExampleProtoBufModel> pair in loadedModels)
            {
                ProtoTestingUtils.AssertModelsAreEqual(modelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }

            Dictionary<string, ExamplePersistentProto> generatedModelsToSave = new Dictionary<string, ExamplePersistentProto>();

            for (int i = 0; i < ProtoTestingUtils.m_batchModelNamesWithSubdirectory.Count; ++i)
            {
                generatedModelsToSave.Add(ProtoTestingUtils.m_batchModelNamesWithSubdirectory[i], ProtoTestingUtils.GenerateRandomPersistentProto());
            }

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedModelsToSave)
            {
                yield return SaveManager.SaveModelAsync(pair.Key, pair.Value).GetAsIEnumerator();
            }

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedModelsToSave)
            {
                Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(pair.Key)));
            }

            Task<Dictionary<string, ExamplePersistentProto>> loadTask2 = SaveManager.LoadModelsByTypeAsync<ExamplePersistentProto>();
            yield return loadTask2.GetAsIEnumerator();
            Dictionary<string, ExamplePersistentProto> generatedLoadedModels = loadTask2.Result;

            foreach (KeyValuePair<string, ExamplePersistentProto> pair in generatedLoadedModels)
            {
                ProtoTestingUtils.AssertGeneratedModelsAreEqual(generatedModelsToSave[Path.GetFileNameWithoutExtension(pair.Key)], pair.Value);
            }
        }

        [Test]
        public async void TestSaveAsyncFaultyProtobufModel()
        {
            await ToolsetAssert.ThrowsAsync<InvalidOperationException>(SaveManager.SaveModelAsync("Faulty", new ExampleFaultyProtoBufModel()));
        }

        [Test]
        public async void TestLoadAsyncFaultyProtobufModel()
        {
            await ToolsetAssert.ThrowsAsync<InvalidOperationException>(SaveManager.LoadModelAsync<ExampleFaultyProtoBufModel>("Faulty"));
        }

        [Test]
        public async void TestSaveAsyncInvalidFileName()
        {
            await SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNamesAsync(SaveAsyncWrapper);
        }

        [Test]
        public async void TestLoadAsyncInvalidFileName()
        {
            await SaveManagerTestingUtils.AssertExceptionsOnInvalidFileNamesAsync(LoadAsyncWrapper);
        }

        [TearDown]
        public void TearDown()
        {
            SaveManager.DeleteModelsByType<ExampleProtoBufModel>();
            SaveManager.DeleteModelsByType<ExamplePersistentProto>();
        }

        private async Task SaveAsyncWrapper(string filename)
        {
            await SaveManager.SaveModelAsync(filename, new ExamplePersistentProto());
        }

        private async Task LoadAsyncWrapper(string filename)
        {
            await SaveManager.LoadModelAsync<ExamplePersistentProto>(filename);
        }
    }
}

using NUnit.Framework;
using System;
using System.IO;
using System.Collections;
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
        private const string c_asyncFileName = "asyncSaveAndLoad";

        [UnityTest]
        public IEnumerator TestSaveLoadAsync()
        {
            ExamplePersistentProto data = ProtoTestingUtils.GenerateRandomPersistentProto();

            yield return SaveManager.SaveModelAsync(c_asyncFileName, data).GetAsIEnumerator();

            Assert.IsTrue(File.Exists(SaveManager.GetDataFilePathForType<ExamplePersistentProto>(c_asyncFileName)));

            Task<ExamplePersistentProto> loadTask = SaveManager.LoadModelAsync<ExamplePersistentProto>(c_asyncFileName);
            yield return loadTask.GetAsIEnumerator();

            ProtoTestingUtils.AssertGeneratedModelsAreEqual(data, loadTask.Result);
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

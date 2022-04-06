using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Integration tests to validate the GameObjectPool class.
    /// </summary>
    public class TestsGameObjectPools
    {
        private const string c_exampleScenePath = "Assets/Toolset/Core/Tests/TestingUtils/MonoBehaviors/GameObjectPools/ExampleSceneGameObjectPools.unity";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(c_exampleScenePath, new LoadSceneParameters(LoadSceneMode.Additive));
            while (!operation.isDone)
            {
                yield return null;
            }
            ExamplePoolable.InstanceCount = 0;
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestIsSchemaSet()
        {
            while (!GameObjectPool<ExamplePoolable>.I.IsSchemaSet())
            {
                yield return null;
            }
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestInstantiatePoolable()
        {
            yield return TestIsSchemaSet();

            ExamplePoolable instance = GameObjectPool<ExamplePoolable>.I.Take();
            Assert.IsTrue(instance.gameObject.activeInHierarchy);

            yield return null;

            Assert.AreEqual(1, ExamplePoolable.InstanceCount);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestTakeThenReturn()
        {
            yield return TestIsSchemaSet();

            ExamplePoolable instance = GameObjectPool<ExamplePoolable>.I.Take();
            Assert.IsTrue(instance.gameObject.activeInHierarchy);

            yield return null;

            Assert.AreEqual(1, ExamplePoolable.InstanceCount);

            yield return null;

            GameObjectPool<ExamplePoolable>.I.Return(instance);
            Assert.IsFalse(instance.gameObject.activeInHierarchy);

            yield return null;

            Assert.AreEqual(1, ExamplePoolable.InstanceCount);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestTakeFromInactive()
        {
            yield return TestTakeThenReturn();

            ExamplePoolable instance = GameObjectPool<ExamplePoolable>.I.Take();
            Assert.IsTrue(instance.gameObject.activeInHierarchy);

            yield return null;

            Assert.AreEqual(1, ExamplePoolable.InstanceCount);
        }

        [Test]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public void TestExceptionIfSchemaNotSet()
        {
            ExampleFaultyPoolable instance = null;

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                instance = GameObjectPool<ExampleFaultyPoolable>.I.Take();
            });

            ToolsetAssert.Throws<InvalidOperationException>(() =>
            {
                GameObjectPool<ExampleFaultyPoolable>.I.Return(instance);
            });
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            AsyncOperation operation = EditorSceneManager.UnloadSceneAsync(c_exampleScenePath);
            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
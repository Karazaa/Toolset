using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using NUnit.Framework;

public class TestsGameObjectPools
{
    private const string c_exampleScenePath = "Assets/Toolset/Core/Tests/TestingUtils/GameObjectPools/ExampleSceneGameObjectPools.unity";
    public const int c_timeoutMilliseconds = 10000;

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
    [Timeout(c_timeoutMilliseconds)]
    public IEnumerator TestIsSchemaSet()
    {
        while (!GameObjectPool<ExamplePoolable>.I.IsSchemaSet())
        {
            yield return null;
        }
    }

    [UnityTest]
    [Timeout(c_timeoutMilliseconds)]
    public IEnumerator TestInstantiatePoolable()
    {
        yield return TestIsSchemaSet();

        ExamplePoolable instance = GameObjectPool<ExamplePoolable>.I.Take();
        Assert.IsTrue(instance.gameObject.activeInHierarchy);

        yield return null;

        Assert.AreEqual(1, ExamplePoolable.InstanceCount);
    }

    [UnityTest]
    [Timeout(c_timeoutMilliseconds)]
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
    [Timeout(c_timeoutMilliseconds)]
    public IEnumerator TestTakeFromInactive()
    {
        yield return TestTakeThenReturn();

        ExamplePoolable instance = GameObjectPool<ExamplePoolable>.I.Take();
        Assert.IsTrue(instance.gameObject.activeInHierarchy);

        yield return null;

        Assert.AreEqual(1, ExamplePoolable.InstanceCount);
    }

    [Test]
    [Timeout(c_timeoutMilliseconds)]
    public void TestExceptionIfSchemaNotSet()
    {
        ExampleFaultyPoolable instance = null;

        Assert.Throws<InvalidOperationException>(() => 
        {
            instance = GameObjectPool<ExampleFaultyPoolable>.I.Take();
        });

        Assert.Throws<InvalidOperationException>(() =>
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

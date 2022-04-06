using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class used for running integration tests on the AsyncOperationExtensions class.
    /// </summary>
    public class TestsAsyncOperationExtensions
    {
        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestGetAsIEnumerator()
        {
            GameObject searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNull(searchTarget);

            yield return SceneManager.LoadSceneAsync(ExampleRoutines.c_exampleSceneNameRoutines, LoadSceneMode.Additive).GetAsIEnumerator();

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNotNull(searchTarget);

            yield return SceneManager.UnloadSceneAsync(ExampleRoutines.c_exampleSceneNameRoutines).GetAsIEnumerator();

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNull(searchTarget);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestResetDoesNothing()
        {
            AsyncOperation loadOperationToYieldOn = SceneManager.LoadSceneAsync(ExampleRoutines.c_exampleSceneNameRoutines, LoadSceneMode.Additive);
            AsyncOperationEnumerator loadOperationAsEnumerator = loadOperationToYieldOn.GetAsIEnumerator() as AsyncOperationEnumerator;

            yield return loadOperationAsEnumerator;

            Assert.IsTrue(loadOperationToYieldOn.isDone);
            loadOperationAsEnumerator.Reset();
            Assert.IsTrue(loadOperationToYieldOn.isDone);

            AsyncOperation unloadOperationToYieldOn = SceneManager.UnloadSceneAsync(ExampleRoutines.c_exampleSceneNameRoutines);
            AsyncOperationEnumerator unloadOperationAsEnumerator = unloadOperationToYieldOn.GetAsIEnumerator() as AsyncOperationEnumerator;

            yield return unloadOperationAsEnumerator;

            Assert.IsTrue(unloadOperationToYieldOn.isDone);
            unloadOperationAsEnumerator.Reset();
            Assert.IsTrue(unloadOperationToYieldOn.isDone);
        }
    }
}

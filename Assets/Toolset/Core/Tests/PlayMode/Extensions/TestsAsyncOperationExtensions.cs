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

            yield return SceneManager.LoadSceneAsync(ToolsetTestingConstants.c_exampleSceneNameRoutines, LoadSceneMode.Additive).GetAsIEnumerator();

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNotNull(searchTarget);

            yield return SceneManager.UnloadSceneAsync(ToolsetTestingConstants.c_exampleSceneNameRoutines);

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNull(searchTarget);
        }
    }
}

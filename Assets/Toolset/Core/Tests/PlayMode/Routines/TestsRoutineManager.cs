using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Integrtion tests to validate the RoutineManager class.
    /// </summary>
    public class TestsRoutineManager
    {
        private const int c_timeoutMilliseconds = 10000;

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestStartRoutineNominal()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsNominalFinished);

            RoutineManager.I.StartRoutine(runner.NominalRootRoutine);

            while (!runner.IsNominalFinished)
            {
                yield return null;
            }
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestStartRoutineFaulty()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsFaultyFinished);

            bool exceptionOccurred = false;
            RoutineManager.I.StartRoutine(runner.FaultyRootRoutine, (exception) => {
                exceptionOccurred = true;
            });

            while (!runner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
        }
    }
}

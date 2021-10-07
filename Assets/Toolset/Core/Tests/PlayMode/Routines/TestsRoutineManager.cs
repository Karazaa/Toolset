using System;
using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;

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
        public IEnumerator TestStartRoutineWaitNominal()
        {
            IEnumerator TestForTimeScale(float timeScale)
            {
                Time.timeScale = timeScale;

                ExampleRoutineRunner runner = new ExampleRoutineRunner();
                Assert.IsFalse(runner.IsNominalWaitFinished);

                RoutineManager.I.StartRoutine(runner.NominalWaitRootRoutine);

                DateTime started = DateTime.Now;
                while (!runner.IsNominalWaitFinished)
                {
                    yield return null;
                }
                DateTime finished = DateTime.Now;

                Assert.Greater((finished - started).TotalSeconds, 1.5/timeScale);
            }

            yield return TestForTimeScale(2.0f);
            yield return TestForTimeScale(0.5f);
            yield return TestForTimeScale(1.0f);
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestStartRoutineFaulty()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsFaultyFinished);

            bool exceptionOccurred = false;
            RoutineManager.I.StartRoutine(runner.FaultyRootRoutine, (exception) =>
            {
                exceptionOccurred = true;
            });

            while (!runner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitFaulty()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsFaultyWaitFinished);

            bool exceptionOccurred = false;
            RoutineManager.I.StartRoutine(runner.FaultyRootRoutine, (exception) =>
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            while (!runner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
        }

        [UnityTest]
        [Timeout(c_timeoutMilliseconds)]
        public IEnumerator TestStartRoutineYieldInstructionFaulty()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsFaultyYieldInstructionFinished);

            bool exceptionOccurred = false;
            RoutineManager.I.StartRoutine(runner.FaultyRootRoutine, (exception) => 
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            while (!runner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
        }
    }
}

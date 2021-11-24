using System;
using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Integrtion tests to validate the RoutineManager class.
    /// </summary>
    public class TestsRoutineManager
    {
        private const string c_searchTargetName = "SearchTarget";
        private const float c_expectedWaitSeconds = 1.5f;

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
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
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitNominal()
        {
            IEnumerator TestForTimeScale(float timeScale)
            {
                Time.timeScale = timeScale;

                ExampleRoutineRunner runner = new ExampleRoutineRunner();
                Assert.IsFalse(runner.IsNominalWaitFinished);

                runner.WaitTime = c_expectedWaitSeconds;
                RoutineManager.I.StartRoutine(runner.NominalWaitRootRoutine);

                DateTime started = DateTime.Now;
                while (!runner.IsNominalWaitFinished)
                {
                    yield return null;
                }
                DateTime finished = DateTime.Now;

                Assert.Greater((finished - started).TotalSeconds, 1.5 / timeScale);
            }

            yield return TestForTimeScale(2.0f);
            yield return TestForTimeScale(0.5f);
            yield return TestForTimeScale(1.0f);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitRealtimeNominal()
        {
            IEnumerator TestForTimeScale(float timeScale)
            {
                Time.timeScale = timeScale;

                ExampleRoutineRunner runner = new ExampleRoutineRunner();
                Assert.IsFalse(runner.IsWaitRealtimeFinished);

                runner.WaitTime = c_expectedWaitSeconds;
                RoutineManager.I.StartRoutine(runner.WaitRealtimeRoutine);

                DateTime started = DateTime.Now;
                while (!runner.IsWaitRealtimeFinished)
                {
                    yield return null;
                }
                DateTime finished = DateTime.Now;

                Assert.Greater((finished - started).TotalSeconds, c_expectedWaitSeconds);
            }

            yield return TestForTimeScale(1.0f);
            yield return TestForTimeScale(2.0f);
            yield return TestForTimeScale(10.0f);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
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
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitFaulty()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsFaultyWaitFinished);

            bool exceptionOccurred = false;
            runner.WaitTime = c_expectedWaitSeconds;
            RoutineManager.I.StartRoutine(runner.FaultyWaitRootRoutine, (exception) =>
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
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
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

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineAsyncOperation()
        {
            ExampleRoutineRunner runner = new ExampleRoutineRunner();
            Assert.IsFalse(runner.IsLoadExampleSceneFinished);
            Assert.IsFalse(runner.IsUnloadExampleSceneFinished);

            GameObject searchTarget = GameObject.Find(c_searchTargetName);
            Assert.IsNull(searchTarget);

            RoutineManager.I.StartRoutine(runner.LoadExampleSceneRoutine);

            while (!runner.IsLoadExampleSceneFinished)
            {
                yield return null;
            }

            searchTarget = GameObject.Find(c_searchTargetName);
            Assert.IsNotNull(searchTarget);

            RoutineManager.I.StartRoutine(runner.UnloadExampleSceneRoutine);

            while (!runner.IsUnloadExampleSceneFinished)
            {
                yield return null;
            }

            searchTarget = GameObject.Find(c_searchTargetName);
            Assert.IsNull(searchTarget);
        }
    }
}

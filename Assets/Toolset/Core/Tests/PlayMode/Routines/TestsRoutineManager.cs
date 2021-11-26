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
        private const float c_expectedWaitSeconds = 0.5f;

        private ExampleRoutineRunner m_routineRunner;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            m_routineRunner = new ExampleRoutineRunner();
            yield break;
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineNominal()
        {
            Assert.IsFalse(m_routineRunner.IsNominalFinished);

            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.NominalRootRoutine);

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_routineRunner.IsNominalFinished)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitNominal()
        {
            IEnumerator TestForTimeScale(float timeScale)
            {
                Time.timeScale = timeScale;

                m_routineRunner = new ExampleRoutineRunner();
                Assert.IsFalse(m_routineRunner.IsNominalWaitFinished);

                m_routineRunner.WaitTimeSeconds = c_expectedWaitSeconds;
                RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.NominalWaitRootRoutine);

                DateTime started = DateTime.Now;

                Assert.IsFalse(routineHandle.IsDone);
                while (!m_routineRunner.IsNominalWaitFinished)
                {
                    yield return null;
                }
                Assert.IsTrue(routineHandle.IsDone);

                DateTime finished = DateTime.Now;

                Assert.Greater((finished - started).TotalSeconds, c_expectedWaitSeconds / timeScale);
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

                m_routineRunner = new ExampleRoutineRunner();
                Assert.IsFalse(m_routineRunner.IsWaitRealtimeFinished);

                m_routineRunner.WaitTimeSeconds = c_expectedWaitSeconds;
                RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.WaitRealtimeRoutine);

                DateTime started = DateTime.Now;

                Assert.IsFalse(routineHandle.IsDone);
                while (!m_routineRunner.IsWaitRealtimeFinished)
                {
                    yield return null;
                }
                Assert.IsTrue(routineHandle.IsDone);

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
            Assert.IsFalse(m_routineRunner.IsFaultyFinished);

            bool exceptionOccurred = false;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.FaultyRootRoutine, (exception) =>
            {
                exceptionOccurred = true;
            });

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_routineRunner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitFaulty()
        {
            Assert.IsFalse(m_routineRunner.IsFaultyWaitFinished);

            bool exceptionOccurred = false;
            m_routineRunner.WaitTimeSeconds = c_expectedWaitSeconds;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.FaultyWaitRootRoutine, (exception) =>
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_routineRunner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineYieldInstructionFaulty()
        {
            Assert.IsFalse(m_routineRunner.IsFaultyYieldInstructionFinished);

            bool exceptionOccurred = false;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.FaultyRootRoutine, (exception) =>
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_routineRunner.IsFaultyFinished || !exceptionOccurred)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineAsyncOperation()
        {
            Assert.IsFalse(m_routineRunner.IsLoadExampleSceneFinished);
            Assert.IsFalse(m_routineRunner.IsUnloadExampleSceneFinished);

            GameObject searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNull(searchTarget);

            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.LoadExampleSceneRoutine);

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_routineRunner.IsLoadExampleSceneFinished)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNotNull(searchTarget);

            routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.UnloadExampleSceneRoutine);

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_routineRunner.IsUnloadExampleSceneFinished)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNull(searchTarget);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineTask()
        {
            Assert.IsFalse(m_routineRunner.IsTaskDelayFinished);

            m_routineRunner.WaitTimeSeconds = c_expectedWaitSeconds;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.TaskDelayRoutine);

            DateTime started = DateTime.Now;
            while (!m_routineRunner.IsTaskDelayFinished)
            {
                yield return null;
            }
            DateTime finished = DateTime.Now;

            Assert.Greater((finished - started).TotalSeconds, c_expectedWaitSeconds);
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStopRoutine()
        {
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.InifiniteRoutine);
            RoutineHandle secondRoutineHandle = RoutineManager.I.StartRoutine(m_routineRunner.InifiniteRoutine);

            Assert.IsFalse(routineHandle.IsDone);
            Assert.IsFalse(secondRoutineHandle.IsDone);

            RoutineManager.I.StopRoutine(routineHandle);

            Assert.IsTrue(routineHandle.IsDone);
            Assert.IsFalse(secondRoutineHandle.IsDone);

            RoutineManager.I.StopRoutine(secondRoutineHandle);

            Assert.IsTrue(routineHandle.IsDone);
            Assert.IsTrue(secondRoutineHandle.IsDone);

            yield break;
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStopAllRoutines()
        {
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.InifiniteRoutine);
            RoutineHandle secondRoutineHandle = RoutineManager.I.StartRoutine(m_routineRunner.InifiniteRoutine);

            Assert.IsFalse(routineHandle.IsDone);
            Assert.IsFalse(secondRoutineHandle.IsDone);

            RoutineManager.I.StopAllRoutines();

            Assert.IsTrue(routineHandle.IsDone);
            Assert.IsTrue(secondRoutineHandle.IsDone);

            yield break;
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStoppingAlreadyStoppedRoutine()
        {
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.InifiniteRoutine);

            Assert.IsFalse(routineHandle.IsDone);

            RoutineManager.I.StopRoutine(routineHandle);

            Assert.IsTrue(routineHandle.IsDone);

            RoutineManager.I.StopRoutine(routineHandle);

            Assert.IsTrue(routineHandle.IsDone);

            yield break;
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineRoutineHandle()
        {
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_routineRunner.InifiniteRoutine);

            IEnumerator YieldOnRoutineHandle()
            {
                yield return routineHandle;
            }

            RoutineHandle secondRoutineHandle = RoutineManager.I.StartRoutine(YieldOnRoutineHandle());

            Assert.IsFalse(routineHandle.IsDone);
            Assert.IsFalse(secondRoutineHandle.IsDone);

            yield return null;

            RoutineManager.I.StopRoutine(routineHandle);

            yield return null;

            Assert.IsTrue(routineHandle.IsDone);
            Assert.IsTrue(secondRoutineHandle.IsDone);
        }
    }
}

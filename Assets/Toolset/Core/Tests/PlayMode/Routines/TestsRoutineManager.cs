using System;
using System.Collections;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine;
using Toolset.Global.Utils;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Integrtion tests to validate the RoutineRunner class.
    /// </summary>
    public class TestsRoutineManager
    {
        private const float c_expectedWaitSeconds = 0.5f;

        private ExampleRoutines m_exampleRoutines;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            m_exampleRoutines = new ExampleRoutines();
            yield break;
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineNominal()
        {
            Assert.IsFalse(m_exampleRoutines.IsNominalFinished);

            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.NominalRootRoutine);

            Assert.IsFalse(routineHandle.IsDone);
            while (!m_exampleRoutines.IsNominalFinished)
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

                m_exampleRoutines = new ExampleRoutines();
                Assert.IsFalse(m_exampleRoutines.IsNominalWaitFinished);

                m_exampleRoutines.WaitTimeSeconds = c_expectedWaitSeconds;
                RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.NominalWaitRootRoutine);

                DateTime started = DateTime.Now;

                Assert.IsFalse(routineHandle.IsDone);
                while (!m_exampleRoutines.IsNominalWaitFinished)
                {
                    yield return null;
                }
                Assert.IsTrue(routineHandle.IsDone);

                DateTime finished = DateTime.Now;

                Assert.GreaterOrEqual((finished - started).TotalSeconds, (c_expectedWaitSeconds / timeScale) - 0.01f);
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

                m_exampleRoutines = new ExampleRoutines();
                Assert.IsFalse(m_exampleRoutines.IsWaitRealtimeFinished);

                m_exampleRoutines.WaitTimeSeconds = c_expectedWaitSeconds;
                RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.WaitRealtimeRoutine);

                DateTime started = DateTime.Now;

                Assert.IsFalse(routineHandle.IsDone);
                while (!m_exampleRoutines.IsWaitRealtimeFinished)
                {
                    yield return null;
                }
                Assert.IsTrue(routineHandle.IsDone);

                DateTime finished = DateTime.Now;

                Assert.GreaterOrEqual((finished - started).TotalSeconds, c_expectedWaitSeconds * 0.99f);
            }

            yield return TestForTimeScale(1.0f);
            yield return TestForTimeScale(2.0f);
            yield return TestForTimeScale(10.0f);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineFaulty()
        {
            Assert.IsFalse(m_exampleRoutines.IsFaultyFinished);

            bool exceptionOccurred = false;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.FaultyRootRoutine, (exception) =>
            {
                exceptionOccurred = true;
            });

            while (!exceptionOccurred)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitFaulty()
        {
            Assert.IsFalse(m_exampleRoutines.IsFaultyWaitFinished);

            bool exceptionOccurred = false;
            m_exampleRoutines.WaitTimeSeconds = c_expectedWaitSeconds;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.FaultyWaitRootRoutine, (exception) =>
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            while (!exceptionOccurred)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineYieldInstructionFaulty()
        {
            Assert.IsFalse(m_exampleRoutines.IsFaultyYieldInstructionFinished);

            bool exceptionOccurred = false;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.FaultyRootRoutine, (exception) =>
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            while (!exceptionOccurred)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineAsyncOperation()
        {
            Assert.IsFalse(m_exampleRoutines.IsLoadExampleSceneFinished);
            Assert.IsFalse(m_exampleRoutines.IsUnloadExampleSceneFinished);

            GameObject searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNull(searchTarget);

            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.LoadExampleSceneRoutine);

            while (!m_exampleRoutines.IsLoadExampleSceneFinished)
            {
                yield return null;
            }
            Assert.IsTrue(routineHandle.IsDone);

            searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            Assert.IsNotNull(searchTarget);

            routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.UnloadExampleSceneRoutine);

            while (!m_exampleRoutines.IsUnloadExampleSceneFinished)
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
            Assert.IsFalse(m_exampleRoutines.IsTaskDelayFinished);

            m_exampleRoutines.WaitTimeSeconds = c_expectedWaitSeconds;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.TaskDelayRoutine);

            DateTime started = DateTime.Now;
            while (!m_exampleRoutines.IsTaskDelayFinished)
            {
                yield return null;
            }
            DateTime finished = DateTime.Now;

            Assert.GreaterOrEqual((finished - started).TotalSeconds, c_expectedWaitSeconds - 0.001f);
            Assert.IsTrue(routineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStopRoutine()
        {
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.InifiniteRoutine);
            RoutineHandle secondRoutineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.InifiniteRoutine);

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
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.InifiniteRoutine);
            RoutineHandle secondRoutineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.InifiniteRoutine);

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
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.InifiniteRoutine);

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
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.InifiniteRoutine);

            IEnumerator YieldOnRoutineHandle()
            {
                yield return routineHandle;
            }

            RoutineHandle secondRoutineHandle = RoutineManager.I.StartRoutine(YieldOnRoutineHandle());

            Assert.IsFalse(routineHandle.IsDone);
            Assert.IsFalse(secondRoutineHandle.IsDone);

            yield return new WaitForSecondsRealtime(c_expectedWaitSeconds);

            RoutineManager.I.StopRoutine(routineHandle);

            yield return new WaitForSecondsRealtime(c_expectedWaitSeconds);

            Assert.IsTrue(routineHandle.IsDone);
            Assert.IsTrue(secondRoutineHandle.IsDone);

            RoutineHandle thirdRoutineHandle = RoutineManager.I.StartRoutine(YieldOnRoutineHandle());

            yield return new WaitForSecondsRealtime(c_expectedWaitSeconds);

            Assert.IsTrue(routineHandle.IsDone);
            Assert.IsTrue(thirdRoutineHandle.IsDone);
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitForFixedUpdate()
        {
            yield return LoadExampleScene();

            ExampleRoutinesMonoBehavior exampleMonoBehavior = GetExampleMonoBehavior();
            exampleMonoBehavior.FixedDeltaTimeSeconds = 1.0f;
            
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(exampleMonoBehavior.WaitForFixedUpdateRoutine());
            while (!routineHandle.IsDone)
            {
                yield return null;
            }

            // Using 0.75f to allow a little wiggle room since deltatime can change over the duration of the test.
            Assert.GreaterOrEqual(exampleMonoBehavior.UpdateCount, 0.75f/Time.deltaTime);

            yield return UnloadExampleScene();
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineWaitForEndOfFrame()
        {
            yield return LoadExampleScene();

            ExampleRoutinesMonoBehavior exampleMonoBehavior = GetExampleMonoBehavior();

            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(exampleMonoBehavior.WaitForEndOfFrameRoutine());
            while (!routineHandle.IsDone)
            {
                yield return null;
            }

            Assert.GreaterOrEqual(exampleMonoBehavior.UpdateCount, 1);
            Assert.LessOrEqual(exampleMonoBehavior.UpdateCount, 2);

            yield return UnloadExampleScene();
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineCoroutine()
        {
            yield return LoadExampleScene();

            ExampleRoutinesMonoBehavior exampleMonoBehavior = GetExampleMonoBehavior();
            exampleMonoBehavior.CoroutineWaitSeconds = c_expectedWaitSeconds;

            bool exceptionOccurred = false;
            RoutineManager.I.StartRoutine(exampleMonoBehavior.WaitForCoroutine(), (exception) => 
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            while (!exceptionOccurred)
            {
                yield return null;
            }

            yield return UnloadExampleScene();
        }

        [UnityTest]
        [Timeout(ToolsetTestingConstants.c_mediumTimeoutMilliseconds)]
        public IEnumerator TestStartRoutineUnknownYieldInstruction()
        {
            bool exceptionOccurred = false;
            RoutineHandle routineHandle = RoutineManager.I.StartRoutine(m_exampleRoutines.UnknownYieldInstructionRoutine, (exception) => 
            {
                Assert.IsTrue(exception is InvalidOperationException);
                exceptionOccurred = true;
            });

            while (!exceptionOccurred)
            {
                yield return null;
            }
        }

        private IEnumerator LoadExampleScene()
        {
            RoutineManager.I.StartRoutine(m_exampleRoutines.LoadExampleSceneRoutine);
            while (!m_exampleRoutines.IsLoadExampleSceneFinished)
            {
                yield return null;
            }
        }

        private IEnumerator UnloadExampleScene()
        {
            RoutineManager.I.StartRoutine(m_exampleRoutines.UnloadExampleSceneRoutine);
            while (!m_exampleRoutines.IsUnloadExampleSceneFinished)
            {
                yield return null;
            }
        }

        private ExampleRoutinesMonoBehavior GetExampleMonoBehavior()
        {
            GameObject searchTarget = GameObject.Find(ToolsetTestingConstants.c_searchTargetNameRoutines);
            return searchTarget.GetComponent<ExampleRoutinesMonoBehavior>();
        }
    }
}

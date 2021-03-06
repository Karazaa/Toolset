using System.Collections;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class used to test the RoutineRunner class.
    /// </summary>
    public class ExampleRoutines
    {
        public const string c_exampleSceneNameRoutines = "ExampleSceneRoutines";
        public bool IsNominalFinished => m_stateMachine.CurrentState == States.AfterRootYield;
        public IEnumerator NominalRootRoutine => NominalRootLayer();

        public float WaitTimeSeconds { get; set; }
        public bool IsNominalWaitFinished => m_stateMachine.CurrentState == States.AfterRootYield;
        public IEnumerator NominalWaitRootRoutine => NominalWaitRootLayer();

        public bool IsFaultyFinished => (m_stateMachine.CurrentState == States.BeforeNest2Yield &&
                                            m_wasFaultyRootAfterInvoked &&
                                            m_wasFaultyNest1AfterInvoked &&
                                            !m_wasFaultyNest2AfterInvoked);
        public IEnumerator FaultyRootRoutine => FaultyRootLayer();

        public bool IsFaultyWaitFinished => (m_stateMachine.CurrentState == States.BeforeNest2Yield &&
                                    m_wasFaultyRootAfterInvoked &&
                                    m_wasFaultyNest1AfterInvoked &&
                                    !m_wasFaultyNest2AfterInvoked);
        public IEnumerator FaultyWaitRootRoutine => FaultyWaitRootLayer();

        public bool IsFaultyYieldInstructionFinished => (m_stateMachine.CurrentState == States.BeforeNest2Yield &&
                            m_wasFaultyRootAfterInvoked &&
                            m_wasFaultyNest1AfterInvoked &&
                            !m_wasFaultyNest2AfterInvoked);
        public IEnumerator FaultyYieldInstructionRootRoutine => FaultyYieldInstructionRootLayer();

        public bool IsLoadExampleSceneFinished { get; private set; }
        public IEnumerator LoadExampleSceneRoutine => LoadExampleScene();

        public bool IsUnloadExampleSceneFinished { get; private set; }
        public IEnumerator UnloadExampleSceneRoutine => UnloadExampleScene();

        public bool IsWaitRealtimeFinished { get; private set; }
        public IEnumerator WaitRealtimeRoutine => WaitRealtime();

        public bool IsTaskDelayFinished { get; private set; }
        public IEnumerator TaskDelayRoutine => TaskDelay();

        public IEnumerator InifiniteRoutine => Infinite();
        public IEnumerator UnknownYieldInstructionRoutine => UnknownYieldInstruction();

        private enum States { Created, BeforeRootYield, AfterRootYield, BeforeNest1Yield, AfterNest1Yield, BeforeNest2Yield, AfterNest2Yield }
        private enum Events { BeforeYield1, AfterYield1, BeforeYield2, AfterYield2, BeforeYield3, AfterYield3 }
        private readonly StateMachine<States, Events> m_stateMachine = new StateMachine<States, Events>(States.Created);
        private bool m_wasFaultyRootAfterInvoked = false;
        private bool m_wasFaultyNest1AfterInvoked = false;
        private bool m_wasFaultyNest2AfterInvoked = false;

        public ExampleRoutines()
        {
            m_stateMachine.OnEventGoto(States.Created, Events.BeforeYield1, States.BeforeRootYield);
            m_stateMachine.OnEventGoto(States.BeforeRootYield, Events.BeforeYield2, States.BeforeNest1Yield);
            m_stateMachine.OnEventGoto(States.BeforeNest1Yield, Events.BeforeYield3, States.BeforeNest2Yield);
            m_stateMachine.OnEventGoto(States.BeforeNest2Yield, Events.AfterYield3, States.AfterNest2Yield);
            m_stateMachine.OnEventGoto(States.AfterNest2Yield, Events.AfterYield2, States.AfterNest1Yield);
            m_stateMachine.OnEventGoto(States.AfterNest1Yield, Events.AfterYield1, States.AfterRootYield);
        }

        private IEnumerator NominalRootLayer()
        {
            m_stateMachine.Fire(Events.BeforeYield1);
            yield return NominalNestLayer1();
            m_stateMachine.Fire(Events.AfterYield1);
        }

        private IEnumerator NominalNestLayer1()
        {
            m_stateMachine.Fire(Events.BeforeYield2);
            yield return NominalNestLayer2();
            m_stateMachine.Fire(Events.AfterYield2);
        }

        private IEnumerator NominalNestLayer2()
        {
            m_stateMachine.Fire(Events.BeforeYield3);
            yield return null;
            m_stateMachine.Fire(Events.AfterYield3);
        }

        private IEnumerator NominalWaitRootLayer()
        {
            m_stateMachine.Fire(Events.BeforeYield1);
            yield return new ToolsetWaitForSeconds(WaitTimeSeconds / 6.0f);
            yield return NominalWaitNestLayer1();
            yield return new ToolsetWaitForSeconds(WaitTimeSeconds / 6.0f);
            m_stateMachine.Fire(Events.AfterYield1);
        }

        private IEnumerator NominalWaitNestLayer1()
        {
            m_stateMachine.Fire(Events.BeforeYield2);
            yield return new ToolsetWaitForSeconds(WaitTimeSeconds / 6.0f);
            yield return NominalWaitNestLayer2();
            yield return new ToolsetWaitForSeconds(WaitTimeSeconds / 6.0f);
            m_stateMachine.Fire(Events.AfterYield2);
        }

        private IEnumerator NominalWaitNestLayer2()
        {
            m_stateMachine.Fire(Events.BeforeYield3);
            yield return new ToolsetWaitForSeconds(WaitTimeSeconds / 6.0f);
            yield return null;
            yield return new ToolsetWaitForSeconds(WaitTimeSeconds / 6.0f);
            m_stateMachine.Fire(Events.AfterYield3);
        }

        private IEnumerator FaultyRootLayer()
        {
            m_stateMachine.Fire(Events.BeforeYield1);
            yield return FaultyNestLayer1();
            m_wasFaultyRootAfterInvoked = true;
        }

        private IEnumerator FaultyNestLayer1()
        {
            m_stateMachine.Fire(Events.BeforeYield2);
            yield return FaultyNestLayer2();
            m_wasFaultyNest1AfterInvoked = true;
        }

        private IEnumerator FaultyNestLayer2()
        {
            m_stateMachine.Fire(Events.BeforeYield3);
            ExceptionThrower();
            yield return null;
            m_wasFaultyNest2AfterInvoked = true;
        }

        private void ExceptionThrower()
        {
            throw new InvalidOperationException("Intentional Exception!");
        }

        private IEnumerator FaultyWaitRootLayer()
        {
            m_stateMachine.Fire(Events.BeforeYield1);
            yield return FaultyWaitNestLayer1();
            m_wasFaultyRootAfterInvoked = true;
        }

        private IEnumerator FaultyWaitNestLayer1()
        {
            m_stateMachine.Fire(Events.BeforeYield2);
            yield return FaultyWaitNestLayer2();
            m_wasFaultyNest1AfterInvoked = true;
        }

        private IEnumerator FaultyWaitNestLayer2()
        {
            m_stateMachine.Fire(Events.BeforeYield3);
            yield return new WaitForSeconds(WaitTimeSeconds);
            yield return null;
            m_wasFaultyNest2AfterInvoked = true;
        }

        private IEnumerator FaultyYieldInstructionRootLayer()
        {
            m_stateMachine.Fire(Events.BeforeYield1);
            yield return FaultyYieldInstructionNestLayer1();
            m_wasFaultyRootAfterInvoked = true;
        }

        private IEnumerator FaultyYieldInstructionNestLayer1()
        {
            m_stateMachine.Fire(Events.BeforeYield2);
            yield return FaultyYieldInstructionNestLayer2();
            m_wasFaultyNest1AfterInvoked = true;
        }

        private IEnumerator FaultyYieldInstructionNestLayer2()
        {
            m_stateMachine.Fire(Events.BeforeYield3);
            yield return new YieldInstruction();
            yield return null;
            m_wasFaultyNest2AfterInvoked = true;
        }

        private IEnumerator LoadExampleScene()
        {
            IsLoadExampleSceneFinished = false;

            yield return SceneManager.LoadSceneAsync(c_exampleSceneNameRoutines, LoadSceneMode.Additive);

            IsLoadExampleSceneFinished = true;
        }

        private IEnumerator UnloadExampleScene()
        {
            IsUnloadExampleSceneFinished = false;

            yield return SceneManager.UnloadSceneAsync(c_exampleSceneNameRoutines);

            IsUnloadExampleSceneFinished = true;
        }

        private IEnumerator WaitRealtime()
        {
            IsWaitRealtimeFinished = false;

            yield return new WaitForSecondsRealtime(WaitTimeSeconds);

            IsWaitRealtimeFinished = true;
        }

        private IEnumerator TaskDelay()
        {
            IsTaskDelayFinished = false;

            yield return Task.Delay((int) (WaitTimeSeconds * 1000f));

            IsTaskDelayFinished = true;
        }

        private IEnumerator Infinite()
        {
            while (true)
            {
                yield return null;
            }
        }

        private class WeirdInstruction : YieldInstruction { }
        private IEnumerator UnknownYieldInstruction()
        {
            yield return new WeirdInstruction();
        }
    }
}

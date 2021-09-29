using System.Collections;
using System;

namespace Toolset.Core.Tests
{
    /// <summary>
    /// Class used to test the RoutineManager class.
    /// </summary>
    public class ExampleRoutineRunner
    {
        public bool IsNominalFinished => m_stateMachine.CurrentState == States.AfterRootYield;
        public IEnumerator NominalRootRoutine => NominalRootLayer();

        public bool IsFaultyFinished => (m_stateMachine.CurrentState == States.BeforeNest2Yield && 
                                            m_wasFaultyRootAfterInvoked && 
                                            m_wasFaultyNest1AfterInvoked && 
                                            !m_wasFaultyNest2AfterInvoked);
        public IEnumerator FaultyRootRoutine => FaultyRootLayer();

        private enum States { Created, BeforeRootYield, AfterRootYield, BeforeNest1Yield, AfterNest1Yield, BeforeNest2Yield, AfterNest2Yield }
        private enum Events { BeforeYield1, AfterYield1, BeforeYield2, AfterYield2, BeforeYield3, AfterYield3 }
        private readonly StateMachine<States, Events> m_stateMachine = new StateMachine<States, Events>(States.Created);
        private bool m_wasFaultyRootAfterInvoked = false;
        private bool m_wasFaultyNest1AfterInvoked = false;
        private bool m_wasFaultyNest2AfterInvoked = false;

        public ExampleRoutineRunner()
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
    }
}

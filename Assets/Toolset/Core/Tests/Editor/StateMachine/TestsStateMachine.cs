using System;
using NUnit.Framework;
using States = ExampleStateMachineStates;
using Events = ExampleStateMachineEvents;

/// <summary>
/// Class of unit tests used to validate the StateMachine class.
/// </summary>
public class TestsStateMachine
{
    private StateMachine<States, Events> m_stateMachine;

    [SetUp]
    public void SetUp()
    {
        m_stateMachine = new StateMachine<States, Events>(States.A);
        m_stateMachine.OnEventGoto(States.A, Events.AToB, States.B);
        m_stateMachine.OnEventGoto(States.B, Events.BToC, States.C);
        m_stateMachine.OnEventGoto(States.C, Events.CToD, States.D);
        m_stateMachine.OnEventGoto(States.D, Events.DToA, States.A);
        m_stateMachine.ExecuteOnEnter(States.A, EnterA);
        m_stateMachine.ExecuteOnEnter(States.B, EnterB);
        m_stateMachine.ExecuteOnEnter(States.C, EnterC);
        m_stateMachine.ExecuteOnEnter(States.D, EnterD);
        m_stateMachine.ExecuteOnExit(States.A, ExitA);
        m_stateMachine.ExecuteOnExit(States.B, ExitB);
        m_stateMachine.ExecuteOnExit(States.C, ExitC);
        m_stateMachine.ExecuteOnExit(States.D, ExitD);
    }

    [Test]
    public void TestFullStateCircuit()
    {
        m_stateMachine.Fire(Events.AToB);
        Assert.AreEqual(m_stateMachine.CurrentState, States.B);

        m_stateMachine.Fire(Events.BToC);
        Assert.AreEqual(m_stateMachine.CurrentState, States.C);

        m_stateMachine.Fire(Events.CToD);
        Assert.AreEqual(m_stateMachine.CurrentState, States.D);

        m_stateMachine.Fire(Events.DToA);
        Assert.AreEqual(m_stateMachine.CurrentState, States.A);
    }

    [Test]
    public void TestNonExistentTransitionFromState()
    {
        m_stateMachine.Fire(Events.BToC);
        Assert.AreEqual(m_stateMachine.CurrentState, States.A);

        m_stateMachine.Fire(Events.CToD);
        Assert.AreEqual(m_stateMachine.CurrentState, States.A);

        m_stateMachine.Fire(Events.DToA);
        Assert.AreEqual(m_stateMachine.CurrentState, States.A);
    }

    [Test]
    public void TestMultipleCallbacksException()
    {
        Assert.Throws<InvalidOperationException>(() => 
        {
            m_stateMachine.ExecuteOnEnter(States.A, EnterB);
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            m_stateMachine.ExecuteOnExit(States.A, ExitB);
        });
    }

    [Test]
    public void TestMultipleTransitionsException()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            m_stateMachine.OnEventGoto(States.A, Events.AToB, States.C);
        });
    }

    [Test]
    public void TestForceState()
    {
        m_stateMachine.Fire(Events.AToB);
        Assert.AreEqual(m_stateMachine.CurrentState, States.B);

        m_stateMachine.ForceState(States.A);
        Assert.AreEqual(m_stateMachine.CurrentState, States.A);
    }

    private void EnterA(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.D, previousState);
        Assert.AreEqual(Events.DToA, transitionEvent);
        Assert.AreEqual(States.A, nextState);
    }

    private void EnterB(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.A, previousState);
        Assert.AreEqual(Events.AToB, transitionEvent);
        Assert.AreEqual(States.B, nextState);
    }

    private void EnterC(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.B, previousState);
        Assert.AreEqual(Events.BToC, transitionEvent);
        Assert.AreEqual(States.C, nextState);
    }

    private void EnterD(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.C, previousState);
        Assert.AreEqual(Events.CToD, transitionEvent);
        Assert.AreEqual(States.D, nextState);
    }

    private void ExitA(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.A, previousState);
        Assert.AreEqual(Events.AToB, transitionEvent);
        Assert.AreEqual(States.B, nextState);
    }

    private void ExitB(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.B, previousState);
        Assert.AreEqual(Events.BToC, transitionEvent);
        Assert.AreEqual(States.C, nextState);
    }

    private void ExitC(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.C, previousState);
        Assert.AreEqual(Events.CToD, transitionEvent);
        Assert.AreEqual(States.D, nextState);
    }

    private void ExitD(States previousState, Events transitionEvent, States nextState)
    {
        Assert.AreEqual(States.D, previousState);
        Assert.AreEqual(Events.DToA, transitionEvent);
        Assert.AreEqual(States.A, nextState);
    }
}

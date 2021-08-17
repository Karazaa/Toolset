namespace Toolset.Core.Tests
{
    /// <summary>
    /// An enum of example StateMachine States used to validate the StateMachine
    /// class in Unit Tests.
    /// </summary>
    public enum ExampleStateMachineStates
    {
        A, B, C, D
    }


    /// <summary>
    /// An enum of example StateMachine Events used to validate the StateMachine
    /// class in Unit Tests.
    /// </summary>
    public enum ExampleStateMachineEvents
    {
        AToB, BToC, CToD, DToA
    }
}
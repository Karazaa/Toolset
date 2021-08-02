/// <summary>
/// Example class used by integration tests for validation of the MonoBehaviourSingleton class.
/// </summary>
public class ExampleMonoBehaviorSingleton : MonoBehaviorSingleton<ExampleMonoBehaviorSingleton>
{
    /// <summary>
    /// An example value modified by integration tests for validation of the MonoBehaviourSingleton class.
    /// </summary>
    public int ExampleValue { get; set; }
}
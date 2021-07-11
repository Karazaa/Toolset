/// <summary>
/// Example class used by integration tests for validation of the MonobehaviorSingleton class.
/// </summary>
public class ExampleMonobehaviorSingleton : MonobehaviorSingleton<ExampleMonobehaviorSingleton>
{
    /// <summary>
    /// An example value modified by integration tests for validation of the MonobehaviorSingleton class.
    /// </summary>
    public int ExampleValue { get; set; }
}
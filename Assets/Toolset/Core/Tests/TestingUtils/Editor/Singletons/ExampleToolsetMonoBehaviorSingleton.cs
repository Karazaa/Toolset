namespace Toolset.Core.Tests
{
    /// <summary>
    /// Example class used by integration tests for validation of the ToolsetMonoBehaviourSingleton class.
    /// </summary>
    public class ExampleToolsetMonoBehaviorSingleton : ToolsetMonoBehaviorSingleton<ExampleToolsetMonoBehaviorSingleton>
    {
        /// <summary>
        /// An example value modified by integration tests for validation of the ToolsetMonoBehaviourSingleton class.
        /// </summary>
        public int ExampleValue { get; set; }
    }
}

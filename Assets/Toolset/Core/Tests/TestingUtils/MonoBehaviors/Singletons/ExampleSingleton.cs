namespace Toolset.Core.Tests
{
    /// <summary>
    /// Example class used by unit tests for validation of the Singleton class.
    /// </summary>
    public class ExampleSingleton : Singleton<ExampleSingleton>
    {
        /// <summary>
        /// An example value modified by unit tests for validation of the Singleton class.
        /// </summary>
        public int ExampleValue { get; set; }
    }
}
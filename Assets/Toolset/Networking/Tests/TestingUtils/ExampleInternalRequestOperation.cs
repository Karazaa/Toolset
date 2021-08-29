namespace Toolset.Networking.Tests
{
    public class ExampleInternalRequestOperation : IInternalRequestOperation
    {
        public const int c_numberOfAttemptsForSuccess = 6;
        public static int ExampleAttemptCounter { get; set; }
        public bool IsCompletedSuccessfully => ExampleAttemptCounter >= c_numberOfAttemptsForSuccess;

        public object Current => this;

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
            ExampleAttemptCounter++;
        }
    }
}

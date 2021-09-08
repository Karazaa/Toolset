namespace Toolset.Networking.Tests
{
    /// <summary>
    /// Example routine used for testing NetworkRequests
    /// </summary>
    public class ExampleInternalRequestOperation : IInternalRequestOperation
    {
        public const int c_numberOfAttemptsForSuccess = 6;
        public static int ExampleAttemptCounter { get; set; }
        public bool IsCompletedSuccessfully => ExampleAttemptCounter >= c_numberOfAttemptsForSuccess;
        public bool ShouldRetry => !IsCompletedSuccessfully;

        public object Current { get; }

        public byte[] ResponseData => null;

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

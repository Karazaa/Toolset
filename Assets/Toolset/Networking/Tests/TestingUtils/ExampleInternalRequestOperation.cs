namespace Toolset.Networking.Tests
{
    public class ExampleInternalRequestOperation : IInternalRequestOperation
    {
        public static int ExampleAttemptCounter { get; set; }
        public bool IsCompletedSuccessfully => ExampleAttemptCounter > 3;

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

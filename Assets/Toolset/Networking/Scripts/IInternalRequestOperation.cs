using System.Collections;

namespace Toolset.Networking
{
    public interface IInternalRequestOperation : IEnumerator
    {
        bool IsCompletedSuccessfully { get; }
    }
}

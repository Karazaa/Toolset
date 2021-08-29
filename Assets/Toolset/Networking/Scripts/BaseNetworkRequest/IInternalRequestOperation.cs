using System.Collections;

namespace Toolset.Networking
{
    /// <summary>
    /// Interface for a routine to send the contents of a request over the network.
    /// Sets IsCompletedSuccessfully based on the results of the response. 
    /// </summary>
    public interface IInternalRequestOperation : IEnumerator
    {
        /// <summary>
        /// Whether or not the request to the server got a valid response.
        /// </summary>
        bool IsCompletedSuccessfully { get; }

        /// <summary>
        /// Whether or not the owning NetworkRequest should attempt to
        /// re-send this internal request operation.
        /// </summary>
        bool ShouldRetry { get; }
    }
}

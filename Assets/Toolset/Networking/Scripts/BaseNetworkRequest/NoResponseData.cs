using ProtoBuf;

namespace Toolset.Networking
{
    /// <summary>
    /// Empty ProtoBuf serializeable object used as the response model
    /// for NetworkRequests that do not expect any data to be returned in the 
    /// body of the response.
    /// </summary>
    [ProtoContract]
    public class NoResponseData
    {
    }
}

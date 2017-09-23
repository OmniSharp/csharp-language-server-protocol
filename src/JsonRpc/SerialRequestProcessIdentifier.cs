using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public class SerialRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(Renor renor)
        {
            return RequestProcessType.Serial;
        }
    }
}
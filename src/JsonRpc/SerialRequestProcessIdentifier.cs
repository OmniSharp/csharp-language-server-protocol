using JsonRpc.Server;

namespace JsonRpc
{
    public class SerialRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(Renor renor)
        {
            return RequestProcessType.Serial;
        }
    }
}
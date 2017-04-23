using JsonRpc.Server;

namespace JsonRpc
{
    public interface IRequestProcessIdentifier
    {
        RequestProcessType Identify(Renor renor);
    }
}
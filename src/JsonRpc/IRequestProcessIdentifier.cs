using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestProcessIdentifier
    {
        RequestProcessType Identify(IHandlerDescriptor descriptor);
    }
}

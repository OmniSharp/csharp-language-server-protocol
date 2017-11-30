using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Messages
{
    public class ServerNotInitialized : RpcError
    {
        internal ServerNotInitialized() : base(null, new ErrorMessage(-32002, "Server Not Initialized")) { }
    }
}

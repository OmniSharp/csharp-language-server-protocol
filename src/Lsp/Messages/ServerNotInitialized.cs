using JsonRPC.Server;

namespace Lsp.Messages
{
    public class ServerNotInitialized : Error
    {
        internal ServerNotInitialized() : base(null, new ErrorMessage(-32002, "Server Not Initialized")) { }
    }
}
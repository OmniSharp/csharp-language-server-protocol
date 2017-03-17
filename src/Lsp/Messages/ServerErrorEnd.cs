using JsonRpc;
using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace Lsp.Messages
{
    public class ServerErrorEnd : Error
    {
        internal ServerErrorEnd() : base(null, new ErrorMessage(-32000, "Server Error End")) { }
    }
}
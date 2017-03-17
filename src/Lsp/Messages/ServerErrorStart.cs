using JsonRpc;
using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace Lsp.Messages
{
    public class ServerErrorStart : Error
    {
        internal ServerErrorStart(object data) : base(null, new ErrorMessage(-32099, "Server Error Start", data)) { }
    }
}

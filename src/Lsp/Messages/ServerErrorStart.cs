using JsonRPC.Server;

namespace Lsp.Messages
{
    public class ServerErrorStart : Error
    {
        internal ServerErrorStart(object data) : base(null, new ErrorMessage(-32099, "Server Error Start", data)) { }
    }
}

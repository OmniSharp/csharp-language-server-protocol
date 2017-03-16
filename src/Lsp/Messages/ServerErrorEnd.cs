using JsonRPC.Server;

namespace Lsp.Messages
{
    public class ServerErrorEnd : Error
    {
        internal ServerErrorEnd() : base(null, new ErrorMessage(-32000, "Server Error End")) { }
    }
}
using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace Lsp.Messages
{
    public class RequestCancelled : Error
    {
        internal RequestCancelled() : base(null, new ErrorMessage(-32800, "Server Error End")) { }
    }
}
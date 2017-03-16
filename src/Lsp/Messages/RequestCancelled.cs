using JsonRPC.Server;

namespace Lsp.Messages
{
    public class RequestCancelled : Error
    {
        internal RequestCancelled() : base(null, new ErrorMessage(-32800, "Server Error End")) { }
    }
}
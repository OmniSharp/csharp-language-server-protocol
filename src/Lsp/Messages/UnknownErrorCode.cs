using JsonRpc.Server;
using JsonRpc.Server.Messages;

namespace Lsp.Messages
{
    public class UnknownErrorCode : Error
    {
        internal UnknownErrorCode() : base(null, new ErrorMessage(-32602, "Unknown Error Code")) { }
    }
}
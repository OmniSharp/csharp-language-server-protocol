using JsonRPC.Server;

namespace Lsp.Messages
{
    public class UnknownErrorCode : Error
    {
        internal UnknownErrorCode() : base(null, new ErrorMessage(-32602, "Unknown Error Code")) { }
    }
}
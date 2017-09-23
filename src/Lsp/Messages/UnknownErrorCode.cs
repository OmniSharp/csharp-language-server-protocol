using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Messages
{
    public class UnknownErrorCode : Error
    {
        internal UnknownErrorCode() : base(null, new ErrorMessage(-32602, "Unknown Error Code")) { }
    }
}
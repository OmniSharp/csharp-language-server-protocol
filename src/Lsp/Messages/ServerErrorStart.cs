using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.LanguageServerProtocol.Messages
{
    public class ServerErrorStart : Error
    {
        internal ServerErrorStart(object data) : base(null, new ErrorMessage(-32099, "Server Error Start", data)) { }
    }
}

using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Exit, Direction.ClientToServer)]
    public class ExitParams : IRequest
    {
        public ExitParams() {}
        public static ExitParams Instance { get; } = new ExitParams();
    }
}

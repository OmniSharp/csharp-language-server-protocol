using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Shutdown, Direction.ClientToServer)]
    public class ShutdownParams : IRequest
    {
        public ShutdownParams() {}
        public static ShutdownParams Instance { get; } = new ShutdownParams();
    }
}

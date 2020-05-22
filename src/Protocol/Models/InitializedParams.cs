using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Initialized, Direction.ClientToServer)]
    public class InitializedParams : IRequest
    {
    }
}

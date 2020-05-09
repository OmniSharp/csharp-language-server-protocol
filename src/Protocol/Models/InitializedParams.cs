using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(GeneralNames.Initialized)]
    public class InitializedParams : IRequest
    {
    }
}

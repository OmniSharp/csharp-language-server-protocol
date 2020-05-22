using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
    public class RegistrationParams : IRequest
    {
        public RegistrationContainer Registrations { get; set; }
    }
}

using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(ClientNames.RegisterCapability)]
    public class RegistrationParams : IRequest
    {
        public RegistrationContainer Registrations { get; set; }
    }
}

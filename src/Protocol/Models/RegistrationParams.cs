using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(ClientNames.RegisterCapability)]
    public class RegistrationParams : IRequest
    {
        public RegistrationContainer Registrations { get; set; }
    }
}

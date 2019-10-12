using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class RegistrationParams : IRequest
    {
        public RegistrationContainer Registrations { get; set; }
    }
}

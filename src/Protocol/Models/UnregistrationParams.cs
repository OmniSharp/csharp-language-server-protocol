using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class UnregistrationParams : IRequest
    {
        public UnregistrationContainer Unregisterations { get; set; }

        // Placeholder for v4 support
        [JsonIgnore]
        public UnregistrationContainer Unregistrations { get => Unregisterations; set => Unregisterations = value; }
    }
}

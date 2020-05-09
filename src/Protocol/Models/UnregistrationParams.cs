using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(ClientNames.UnregisterCapability)]
    public class UnregistrationParams : IRequest
    {
        public UnregistrationContainer Unregisterations { get; set; }

        // Placeholder for v4 support
        [JsonIgnore]
        public UnregistrationContainer Unregistrations { get => Unregisterations; set => Unregisterations = value; }
    }
}

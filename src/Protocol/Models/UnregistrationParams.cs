using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(ClientNames.UnregisterCapability, Direction.ServerToClient)]
    public class UnregistrationParams : IRequest
    {
        public UnregistrationContainer Unregisterations { get; set; }

        // Placeholder for v4 support
        [JsonIgnore]
        public UnregistrationContainer Unregistrations { get => Unregisterations; set => Unregisterations = value; }
    }
}

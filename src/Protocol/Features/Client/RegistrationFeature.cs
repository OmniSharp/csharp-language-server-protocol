using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Client", Name = "RegisterCapability"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))
        ]
        public class RegistrationParams : IRequest, IJsonRpcRequest // fix this
        {
            public RegistrationContainer Registrations { get; set; } = null!;
        }

        [Parallel]
        [Method(ClientNames.UnregisterCapability, Direction.ServerToClient)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Client", Name = "UnregisterCapability"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))
        ]
        public class UnregistrationParams : IRequest, IJsonRpcRequest
        {
            public UnregistrationContainer? Unregisterations { get; set; }

            // Placeholder for v4 support
            [JsonIgnore]
            public UnregistrationContainer? Unregistrations
            {
                get => Unregisterations;
                set => Unregisterations = value;
            }
        }
    }

    namespace Client
    {
    }
}

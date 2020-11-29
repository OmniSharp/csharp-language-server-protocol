using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(GeneralNames.SetTrace, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Server"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IClientLanguageClient), typeof(ILanguageClient))]
        public class SetTraceParams : IRequest
        {
            /// <summary>
            /// The new value that should be assigned to the trace setting.
            /// </summary>
            public InitializeTrace Value { get; set; }
        }
    }

    namespace Server
    {
    }
}

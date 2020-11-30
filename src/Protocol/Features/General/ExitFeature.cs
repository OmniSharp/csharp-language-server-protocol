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
        [Serial]
        [Method(GeneralNames.Exit, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.General"), GenerateHandlerMethods, GenerateRequestMethods(typeof(ILanguageClient))]
        public partial record ExitParams : IRequest
        {
            public static ExitParams Instance { get; } = new ExitParams();
        }
    }

    namespace General
    {
        public static partial class ExitExtensions
        {
            public static void SendExit(this ILanguageClient mediator) => mediator.SendNotification(ExitParams.Instance);
        }
    }
}

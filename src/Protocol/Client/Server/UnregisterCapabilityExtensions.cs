using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class UnregisterCapabilityExtensions
    {
        public static Task UnregisterCapability(this ILanguageServerClient mediator, UnregistrationParams @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}

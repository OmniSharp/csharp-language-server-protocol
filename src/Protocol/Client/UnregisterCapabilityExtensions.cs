using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class ClientNames
    {
        public const string UnregisterCapability = "client/unregisterCapability";
    }

    public static class UnregisterCapabilityExtensions
    {
        public static async Task UnregisterCapability(this IResponseRouter mediator, UnregistrationParams @params)
        {
            try
            {
                await mediator.SendRequest(ClientNames.UnregisterCapability, @params);
            }
            catch (Exception e)
            {
                // VsCode today does not implement LSP properly.
                await mediator.SendRequest("client/unregisterFeature", @params);
            }
        }
    }
}

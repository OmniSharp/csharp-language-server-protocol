using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class UnregisterCapabilityExtensions
    {
        public static async Task UnregisterCapability(this IResponseRouter mediator, UnregistrationParams @params)
        {
            try
            {
                await mediator.SendRequest("client/unregisterCapability", @params);
            }
            catch (Exception e)
            {
                // VsCode today does not implement LSP properly.
                await mediator.SendRequest("client/unregisterFeature", @params);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class UnregisterCapabilityExtensions
    {
        public static async Task UnregisterCapability(this ILanguageServer mediator, UnregistrationParams @params)
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
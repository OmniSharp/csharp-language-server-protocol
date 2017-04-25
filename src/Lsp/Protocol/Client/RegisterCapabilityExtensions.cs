using System;
using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class RegisterCapabilityExtensions
    {
        public static async Task RegisterCapability(this ILanguageServer mediator,  RegistrationParams @params)
        {
            try
            {
                await mediator.SendRequest("client/registerCapability", @params);
            }
            catch (Exception e)
            {
                // VsCode today does not implement LSP properly.
                await mediator.SendRequest("client/registerFeature", @params);
            }
        }
    }
}
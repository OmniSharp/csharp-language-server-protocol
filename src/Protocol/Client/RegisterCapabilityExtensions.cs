using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class RegisterCapabilityExtensions
    {
        public static async Task RegisterCapability(this IResponseRouter mediator,  RegistrationParams @params)
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

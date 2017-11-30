using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class ClientNames
    {
        public const string RegisterCapability = "client/registerCapability";
    }

    public static class RegisterCapabilityExtensions
    {
        public static async Task RegisterCapability(this IResponseRouter mediator,  RegistrationParams @params)
        {
            try
            {
                await mediator.SendRequest(ClientNames.RegisterCapability, @params);
            }
            catch (Exception e)
            {
                // VsCode today does not implement LSP properly.
                await mediator.SendRequest("client/registerFeature", @params);
            }
        }
    }
}

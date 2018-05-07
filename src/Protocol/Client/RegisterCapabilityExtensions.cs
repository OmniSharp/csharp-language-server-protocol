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
                await mediator.SendRequest(Client.ClientNames.RegisterCapability, @params);
        }
    }
}

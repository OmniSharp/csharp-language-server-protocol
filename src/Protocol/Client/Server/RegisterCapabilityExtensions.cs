using System;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public static class RegisterCapabilityExtensions
    {
        public static async Task RegisterCapability(this ILanguageServerClient mediator, RegistrationParams @params)
        {
            await mediator.SendRequest(ClientNames.RegisterCapability, @params);
        }
    }
}

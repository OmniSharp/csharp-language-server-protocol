using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class SignatureHelpExtensions
    {
        public static Task<SignatureHelp> SignatureHelp(this ILanguageClientDocument mediator, SignatureHelpParams @params)
        {
            return mediator.SendRequest<SignatureHelpParams, SignatureHelp>(DocumentNames.SignatureHelp, @params);
        }
    }
}

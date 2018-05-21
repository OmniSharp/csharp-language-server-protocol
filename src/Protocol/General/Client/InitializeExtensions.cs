using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class InitializeExtensions
    {
        public static Task<InitializeResult> Initialize(this ILanguageClient mediator, InitializeParams @params)
        {
            return mediator.SendRequest<InitializeParams, InitializeResult>(GeneralNames.Initialize, @params);
        }
    }
}

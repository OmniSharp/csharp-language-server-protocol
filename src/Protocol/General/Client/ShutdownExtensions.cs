using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class ShutdownExtensions
    {
        public static Task Shutdown(this ILanguageClient mediator)
        {
            return mediator.SendRequest<object>(GeneralNames.Shutdown);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class ColorPresentationExtensions
    {
        public static Task<Container<ColorPresentation>> ColorPresentation(this ILanguageClientDocument mediator, ColorPresentationParams @params, CancellationToken cancellationToken= default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}

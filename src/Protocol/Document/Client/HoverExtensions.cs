using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class HoverExtensions
    {
        public static Task<Hover> Hover(this ILanguageClientDocument mediator, HoverParams @params)
        {
            return mediator.SendRequest<HoverParams, Hover>(DocumentNames.Hover, @params);
        }
    }
}

using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class ReferencesExtensions
    {
        public static Task<Container<Location>> References(this ILanguageClientDocument mediator, ReferenceParams @params)
        {
            return mediator.SendRequest<ReferenceParams, Container<Location>>(DocumentNames.References, @params);
        }
    }
}

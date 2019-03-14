using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class DefinitionExtensions
    {
        public static Task<LocationOrLocationLinks> Definition(this ILanguageClientDocument mediator, DefinitionParams @params)
        {
            return mediator.SendRequest<DefinitionParams, LocationOrLocationLinks>(DocumentNames.Definition, @params);
        }
    }
}

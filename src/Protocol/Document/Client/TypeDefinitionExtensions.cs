using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class TypeDefinitionExtensions
    {
        public static Task<LocationOrLocationLinks> TypeDefinition(this ILanguageClientDocument mediator, TypeDefinitionParams @params)
        {
            return mediator.SendRequest<TypeDefinitionParams, LocationOrLocationLinks>(DocumentNames.TypeDefinition, @params);
        }
    }
}

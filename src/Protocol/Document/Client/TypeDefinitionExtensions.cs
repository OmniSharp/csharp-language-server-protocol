using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class TypeDefinitionExtensions
    {
        public static Task<LocationOrLocations> TypeDefinition(this ILanguageClientDocument mediator, TypeDefinitionParams @params)
        {
            return mediator.SendRequest<TypeDefinitionParams, LocationOrLocations>(DocumentNames.TypeDefinition, @params);
        }
    }
}

using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public static class ImplementationExtensions
    {
        public static Task<LocationOrLocations> Implementation(this ILanguageClientDocument mediator, ImplementationParams @params)
        {
            return mediator.SendRequest<ImplementationParams, LocationOrLocations>(DocumentNames.Implementation, @params);
        }
    }
}

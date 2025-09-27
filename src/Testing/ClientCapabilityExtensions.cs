using Microsoft.Extensions.DependencyInjection.Extensions;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    public static class ClientCapabilityExtensions
    {
        public static LanguageClientOptions EnableAllCapabilities(this LanguageClientOptions options)
        {
            var capabilities = options.Assemblies
                                      .Union(new[] { typeof(ICapability).Assembly })
                                      .SelectMany(z => z.ExportedTypes)
                                      .Where(z => typeof(ICapability).IsAssignableFrom(z))
                                      .Where(z => z.IsClass && !z.IsAbstract);
            foreach (var item in capabilities)
            {
                options.WithCapability(( Activator.CreateInstance(item, Array.Empty<object>()) as ICapability )!);
            }

            return options;
        }

        public static LanguageClientOptions DisableAllCapabilities(this LanguageClientOptions options)
        {
            options.Services.RemoveAll(typeof(ICapability));
            return options;
        }
    }
}

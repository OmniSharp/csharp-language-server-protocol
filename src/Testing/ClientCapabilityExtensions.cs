using System;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageProtocol.Testing
{
    public static class ClientCapabilityExtensions
    {
        public static LanguageClientOptions EnableAllCapabilities( this LanguageClientOptions options)
        {
            var capabilities = typeof(ICapability).Assembly.GetExportedTypes().Where(z => typeof(ICapability).IsAssignableFrom(z));
            foreach (var item in capabilities)
            {
                options.WithCapability(Activator.CreateInstance(item, Array.Empty<object>()) as ICapability);
            }
            return options;
        }
        public static LanguageClientOptions DisableAllCapabilities( this LanguageClientOptions options)
        {
            options.SupportedCapabilities.Clear();
            return options;
        }
    }
}

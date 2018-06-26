using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    static class LspHandlerDescriptorHelpers
    {
        public static IJsonRpcHandler InitializeHandler(ILspHandlerDescriptor descriptor, SupportedCapabilities supportedCapabilities, IJsonRpcHandler handler)
        {
            supportedCapabilities.SetCapability(descriptor, handler);
            return handler;
        }

        public static IEnumerable<ISupports> GetSupportedCapabilities(object capabilities)
        {
            return capabilities
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(x => x.CanRead)
                .Select(x => x.GetValue(capabilities))
                .OfType<ISupports>();
        }
    }
}

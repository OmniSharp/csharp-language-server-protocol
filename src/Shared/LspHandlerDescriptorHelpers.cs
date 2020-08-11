using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal static class LspHandlerDescriptorHelpers
    {
        public static IJsonRpcHandler InitializeHandler(ILspHandlerDescriptor descriptor, ISupportedCapabilities supportedCapabilities, IJsonRpcHandler handler)
        {
            supportedCapabilities.SetCapability(descriptor, handler);
            return handler;
        }

        public static IEnumerable<ISupports> GetSupportedCapabilities(object capabilities) =>
            capabilities
               .GetType()
               .GetTypeInfo()
               .DeclaredProperties
               .Where(x => x.CanRead)
               .Select(x => x.GetValue(capabilities))
               .OfType<ISupports>();

        public static IEnumerable<IStaticRegistrationOptions> GetStaticRegistrationOptions(object capabilities) =>
            capabilities
               .GetType()
               .GetTypeInfo()
               .DeclaredProperties
               .Where(x => x.CanRead)
               .Select(x => x.GetValue(capabilities))
               .Select(z => z is ISupports supports ? supports.IsSupported ? supports.Value : z : z)
               .OfType<IStaticRegistrationOptions>();
    }
}

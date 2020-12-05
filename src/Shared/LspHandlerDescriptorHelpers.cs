using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal static class LspHandlerDescriptorHelpers
    {
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

using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class ReferencesOptions : WorkDoneProgressOptions, IReferencesOptions
    {
        public static ReferencesOptions Of(IReferencesOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new ReferencesOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,

            };
        }
    }
}

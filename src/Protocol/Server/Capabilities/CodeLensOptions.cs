using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Code Lens options.
    /// </summary>
    public class CodeLensOptions : WorkDoneProgressOptions, ICodeLensOptions
    {
        /// <summary>
        ///  Code lens has a resolve provider as well.
        /// </summary>
        [Optional]
        public bool ResolveProvider { get; set; }

        public static CodeLensOptions Of(ICodeLensOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new CodeLensOptions() {
                ResolveProvider = options.ResolveProvider || descriptors.Any(z => z.HandlerType == typeof(ICodeLensResolveHandler)),
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}

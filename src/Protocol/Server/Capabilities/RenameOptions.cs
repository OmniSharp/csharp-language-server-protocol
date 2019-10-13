using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class RenameOptions : WorkDoneProgressOptions, IRenameOptions
    {
        /// <summary>
        /// Renames should be checked and tested before being executed.
        /// </summary>
        [Optional]
        public bool PrepareProvider { get; set; }

        public static RenameOptions Of(IRenameOptions options)
        {
            return new RenameOptions() {
                PrepareProvider = options.PrepareProvider,
                WorkDoneProgress = options.WorkDoneProgress
            };
        }
    }
}

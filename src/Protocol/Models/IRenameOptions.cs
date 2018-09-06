using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IRenameOptions
    {
        /// <summary>
        /// Renames should be checked and tested before being executed.
        /// </summary>
        [Optional]
        bool PrepareProvider { get; set; }
    }
}

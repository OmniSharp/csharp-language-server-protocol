using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class OnTypeRenameRanges
    {
        /// <summary>
        /// A list of ranges that can be renamed together. The ranges must have
        /// identical length and contain identical text content. The ranges cannot overlap.
        /// </summary>
        public Container<Range> Ranges { get; set; } = null!;

        /// <summary>
        /// An optional word pattern (regular expression) that describes valid contents for
        /// the given ranges. If no pattern is provided, the client configuration's word
        /// pattern will be used.
        /// </summary>
        [Optional]
        public string? WordPattern { get; set; }
    }
}
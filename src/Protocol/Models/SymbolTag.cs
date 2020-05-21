using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Symbol tags are extra annotations that tweak the rendering of a symbol.
    /// @since 3.16
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(NumberEnumConverter))]
    public enum SymbolTag
    {
        /// <summary>
        /// Render a symbol as obsolete, usually using a strike-out.
        /// </summary>
        Deprecated = 1,
    }
}

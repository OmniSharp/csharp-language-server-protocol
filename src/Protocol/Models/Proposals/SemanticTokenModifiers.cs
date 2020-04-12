using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// A set of predefined token modifiers. This set is not fixed
    /// an clients can specify additional token types via the
    /// corresponding client capabilities.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SemanticTokenModifiers
    {
        Documentation,
        Declaration,
        Definition,
        Static,
        Abstract,
        Deprecated,
        Readonly,
    }
}
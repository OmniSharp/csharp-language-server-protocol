using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// A set of predefined token types. This set is not fixed
    /// an clients can specify additional token types via the
    /// corresponding client capabilities.
    ///
    /// @since 3.16.0
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [Obsolete(Constants.Proposal)]
    public enum SemanticTokenTypes
    {
        Comment,
        Keyword,
        String,
        Number,
        Regexp,
        Operator,
        Namespace,
        Type,
        Struct,
        Class,
        Interface,
        Enum,
        TypeParameter,
        Function,
        Member,
        Property,
        Macro,
        Variable,
        Parameter,
        Label,
    }
}
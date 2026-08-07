//HintName: Test0_DeclarationParams.cs
#nullable enable
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Newtonsoft.Json;
using System.ComponentModel;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace Test
{
    public partial class DeclarationParams
    {
        [Optional]
        public ProgressToken? WorkDoneToken { get; init; }

        [Optional]
        public ProgressToken? PartialResultToken { get; init; }

        [JsonProperty("$$__handler_id__$$", DefaultValueHandling = DefaultValueHandling.Ignore), System.Text.Json.Serialization.JsonPropertyName("$$__handler_id__$$"), System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault), EditorBrowsable(EditorBrowsableState.Never)]
        public string __identity { get; init; }
    }
}
#nullable restore

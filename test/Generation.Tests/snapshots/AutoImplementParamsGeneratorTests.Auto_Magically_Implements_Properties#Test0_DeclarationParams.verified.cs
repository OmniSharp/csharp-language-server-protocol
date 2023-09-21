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
    }
}
#nullable restore

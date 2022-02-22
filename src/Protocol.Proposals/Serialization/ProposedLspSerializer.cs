using System.Collections.Generic;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    internal class ProposedLspSerializer : LspSerializer
    {
        public new static LspSerializer Instance { get; } = new ProposedLspSerializer();

        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
#pragma warning disable CS0618
            AddProposedCapabilitiesConverter<ServerCapabilities, ProposedServerCapabilities>(converters);
            AddProposedCapabilitiesConverter<WorkspaceServerCapabilities, ProposedWorkspaceServerCapabilities>(converters);
            AddProposedCapabilitiesConverter<ClientCapabilities, ProposedClientCapabilities>(converters);
            AddProposedCapabilitiesConverter<GeneralClientCapabilities, ProposedGeneralClientCapabilities>(converters);
            AddProposedCapabilitiesConverter<TextDocumentClientCapabilities, ProposedTextDocumentClientCapabilities>(converters);
            AddProposedCapabilitiesConverter<WindowClientCapabilities, ProposedWindowClientCapabilities>(converters);
            AddProposedCapabilitiesConverter<WorkspaceClientCapabilities, ProposedWorkspaceClientCapabilities>(converters);
#pragma warning restore CS0618
            base.AddOrReplaceConverters(converters);
        }

        private void AddProposedCapabilitiesConverter<TFrom, TTo>(ICollection<JsonConverter> converters) where TTo : TFrom where TFrom : notnull
        {
            RemoveConverter<TFrom>(converters);
            ReplaceConverter(converters, new ProposedCapabilitiesConverter<TFrom, TTo>());
        }
    }
}

﻿using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.DocumentSymbol))]
    public class DocumentSymbolCapability : DynamicCapability, ConnectedCapability<IDocumentSymbolHandler>
    {
        /// <summary>
        /// Specific capabilities for the `SymbolKind` in the `textDocument/symbol` request.
        /// </summary>
        [Optional]
        public SymbolKindCapabilityOptions? SymbolKind { get; set; }

        /// <summary>
        /// Whether document symbol supports hierarchical `DocumentSymbol`s.
        /// </summary>
        [Optional]
        public bool HierarchicalDocumentSymbolSupport { get; set; }

        /// <summary>
        /// The client supports tags on `SymbolInformation`.Tags are supported on
        /// `DocumentSymbol` if `hierarchicalDocumentSymbolSupport` is set tot true.
        /// Clients supporting tags have to handle unknown tags gracefully.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Optional]
        public TagSupportCapabilityOptions? TagSupport { get; set; }

        /// <summary>
        /// The client supports an additional label presented in the UI when
        /// registering a document symbol provider.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public bool LabelSupport { get; set; }
    }
}

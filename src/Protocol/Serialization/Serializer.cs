using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class Serializer : JsonRpcSerializer, ISerializer
    {
        private static readonly CompletionItemKind[] DefaultCompletionItemKinds = Enum
            .GetValues(typeof(CompletionItemKind))
            .Cast<CompletionItemKind>()
            .ToArray();

        private static readonly CompletionItemTag[] DefaultCompletionItemTags = Enum
            .GetValues(typeof(CompletionItemTag))
            .Cast<CompletionItemTag>()
            .ToArray();

        private static readonly SymbolKind[] DefaultSymbolKinds = Enum.GetValues(typeof(SymbolKind))
            .Cast<SymbolKind>()
            .ToArray();

        private static readonly SymbolTag[] DefaultSymbolTags = Enum.GetValues(typeof(SymbolTag))
            .Cast<SymbolTag>()
            .ToArray();

        private static readonly DiagnosticTag[] DefaultDiagnosticTags = Enum.GetValues(typeof(DiagnosticTag))
            .Cast<DiagnosticTag>()
            .ToArray();

        private static readonly CodeActionKind[] DefaultCodeActionKinds = typeof(CodeActionKind)
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(z => z.GetValue(null))
            .Cast<CodeActionKind>()
            .ToArray();

        private CodeActionKind[] _codeActionKinds;
        private CompletionItemTag[] _completionItemTags;
        private CompletionItemKind[] _completionItemKinds;
        private SymbolKind[] _documentSymbolKinds;
        private SymbolTag[] _documentSymbolTags;
        private SymbolKind[] _workspaceSymbolKinds;
        private SymbolTag[] _workspaceSymbolTags;
        private DiagnosticTag[] _diagnosticTags;

        public ClientVersion ClientVersion { get; private set; }

        public static Serializer Instance { get; } = new Serializer();

        public Serializer() : this(ClientVersion.Lsp3)
        {
        }

        public Serializer(ClientVersion clientVersion)
        {
            ClientVersion = clientVersion;
            _completionItemKinds = DefaultCompletionItemKinds;
            _completionItemTags = DefaultCompletionItemTags;
            _documentSymbolKinds = DefaultSymbolKinds;
            _documentSymbolTags = DefaultSymbolTags;
            _workspaceSymbolKinds = DefaultSymbolKinds;
            _workspaceSymbolTags = DefaultSymbolTags;
            _diagnosticTags = DefaultDiagnosticTags;
            _codeActionKinds = DefaultCodeActionKinds;
        }

        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new ValueTupleContractResolver<long, long>());
            ReplaceConverter(converters, new EnumLikeStringConverter());
            ReplaceConverter(Options.Converters, new CodeActionConverter(_codeActionKinds));
            ReplaceConverter(Options.Converters,
                new CompletionItemConverter(_completionItemKinds, _completionItemTags));
            ReplaceConverter(Options.Converters, new DiagnosticConverter(_diagnosticTags));
            ReplaceConverter(Options.Converters,
                new DocumentSymbolConverter(_documentSymbolKinds, _documentSymbolTags));
            ReplaceConverter(Options.Converters,
                new SymbolInformationConverter(_workspaceSymbolKinds, _workspaceSymbolTags));
            base.AddOrReplaceConverters(converters);
        }

        public void SetClientCapabilities(ClientVersion clientVersion, ClientCapabilities clientCapabilities)
        {
            ;

            if (clientCapabilities?.TextDocument?.Completion.IsSupported == true)
            {
                var completion = clientCapabilities.TextDocument.Completion.Value;
                var valueSet = completion?.CompletionItemKind?.ValueSet;
                if (valueSet != null)
                {
                    _completionItemKinds = valueSet.ToArray();
                }

                var tagSupportSet = completion?.CompletionItem?.TagSupport.Value?.ValueSet;
                if (tagSupportSet != null)
                {
                    _completionItemTags = tagSupportSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.DocumentSymbol.IsSupported == true)
            {
                var symbol = clientCapabilities.TextDocument.DocumentSymbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet != null)
                {
                    _documentSymbolKinds = symbolKindSet.ToArray();
                }

                var valueSet = symbol?.TagSupport.Value?.ValueSet;
                if (valueSet != null)
                {
                    _documentSymbolTags = valueSet.ToArray();
                }
            }

            if (clientCapabilities?.Workspace?.Symbol.IsSupported == true)
            {
                var symbol = clientCapabilities.Workspace.Symbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet != null)
                {
                    _workspaceSymbolKinds = symbolKindSet.ToArray();
                }

                var tagSupportSet = symbol?.TagSupport.Value?.ValueSet;
                if (tagSupportSet != null)
                {
                    _workspaceSymbolTags = tagSupportSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.PublishDiagnostics.IsSupported == true)
            {
                var publishDiagnostics = clientCapabilities?.TextDocument?.PublishDiagnostics.Value;
                var tagValueSet = publishDiagnostics.TagSupport.Value?.ValueSet;
                if (tagValueSet != null)
                {
                    _diagnosticTags = tagValueSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.CodeAction.IsSupported == true)
            {
                var codeActions = clientCapabilities?.TextDocument?.CodeAction.Value;
                var kindValueSet = codeActions.CodeActionLiteralSupport?.CodeActionKind?.ValueSet;
                if (kindValueSet != null)
                {
                    _codeActionKinds = kindValueSet.ToArray();
                }
            }

            AddOrReplaceConverters(Options.Converters);
        }
    }
}

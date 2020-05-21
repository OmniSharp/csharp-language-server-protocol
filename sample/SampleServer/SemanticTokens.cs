using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace SampleServer
{
#pragma warning disable 618
    public class SemanticTokens : SemanticTokensHandler
    {
        private readonly ILogger _logger;

        public SemanticTokens(ILogger<SemanticTokens> logger) : base(new SemanticTokensRegistrationOptions() {
            DocumentSelector = DocumentSelector.ForLanguage("csharp"),
            Legend = new SemanticTokensLegend(),
            DocumentProvider = new Supports<SemanticTokensDocumentProviderOptions>(true,
                new SemanticTokensDocumentProviderOptions() {
                    Edits = true
                }),
            RangeProvider = true
        })
        {
            _logger = logger;
        }

        public override async Task<OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals.SemanticTokens> Handle(
            SemanticTokensParams request, CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);
            _logger.LogInformation(JsonConvert.SerializeObject(result));
            return result;
        }

        public override async Task<OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals.SemanticTokens> Handle(
            SemanticTokensRangeParams request, CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);
            _logger.LogInformation(JsonConvert.SerializeObject(result));
            return result;
        }

        public override async Task<SemanticTokensOrSemanticTokensEdits> Handle(SemanticTokensEditsParams request,
            CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);
            _logger.LogInformation(JsonConvert.SerializeObject(result));
            return result;
        }

        protected override async Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier,
            CancellationToken cancellationToken)
        {
            using var typesEnumerator = RotateEnum(SemanticTokenType.Defaults).GetEnumerator();
            using var modifiersEnumerator = RotateEnum(SemanticTokenModifier.Defaults).GetEnumerator();
            // you would normally get this from a common source that is managed by current open editor, current active editor, etc.
            var content = await File.ReadAllTextAsync(DocumentUri.GetFileSystemPath(identifier), cancellationToken);
            await Task.Yield();

            foreach (var (line, text) in content.Split('\n').Select((text, line) => (line, text)))
            {
                var parts = text.TrimEnd().Split(';', ' ', '.', '"', '(', ')');
                var index = 0;
                foreach (var part in parts)
                {
                    typesEnumerator.MoveNext();
                    modifiersEnumerator.MoveNext();
                    if (string.IsNullOrWhiteSpace(part)) continue;
                    index = text.IndexOf(part, index, StringComparison.Ordinal);
                    builder.Push(line, index, part.Length, typesEnumerator.Current, modifiersEnumerator.Current);
                }
            }
        }

        protected override Task<SemanticTokensDocument>
            GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SemanticTokensDocument(GetRegistrationOptions().Legend));
        }


        private IEnumerable<T> RotateEnum<T>(IEnumerable<T> values)
        {
            while (true)
            {
                foreach (var item in values)
                    yield return item;
            }
        }
    }
#pragma warning restore 618
}

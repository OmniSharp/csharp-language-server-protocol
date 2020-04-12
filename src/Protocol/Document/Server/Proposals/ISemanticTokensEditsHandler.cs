using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.SemanticTokens)]
    public interface ISemanticTokensHandler : IJsonRpcRequestHandler<SemanticTokensParams, SemanticTokens>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.SemanticTokensEdits)]
    public interface ISemanticTokensEditsHandler :
        IJsonRpcRequestHandler<SemanticTokensEditsParams, SemanticTokensOrSemanticTokensEdits>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.SemanticTokensRange)]
    public interface ISemanticTokensRangeHandler : IJsonRpcRequestHandler<SemanticTokensRangeParams, SemanticTokens>,
        IRegistration<SemanticTokensRegistrationOptions>, ICapability<SemanticTokensCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class SemanticTokensHandler : ISemanticTokensHandler, ISemanticTokensEditsHandler,
        ISemanticTokensRangeHandler
    {
        private readonly SemanticTokensRegistrationOptions _options;

        public SemanticTokensHandler(SemanticTokensRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public SemanticTokensRegistrationOptions GetRegistrationOptions() => _options;

        public virtual async Task<SemanticTokens> Handle(SemanticTokensParams request,
            CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request);
            var builder = document.Create();
            await Tokenize(builder, request);
            return builder.Commit().GetSemanticTokens();
        }

        public virtual async Task<SemanticTokensOrSemanticTokensEdits> Handle(SemanticTokensEditsParams request,
            CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request);
            var builder = document.Edit(request);
            await Tokenize(builder, request);
            return builder.Commit().GetSemanticTokensEdits();
        }

        // TODO: Is this correct?
        public virtual async Task<SemanticTokens> Handle(SemanticTokensRangeParams request,
            CancellationToken cancellationToken)
        {
            var document = await GetSemanticTokensDocument(request);
            var builder = document.Create();
            await Tokenize(builder, request);

            return builder.Commit().GetSemanticTokens(request.Range);
        }

        public virtual void SetCapability(SemanticTokensCapability capability) => Capability = capability;
        protected SemanticTokensCapability Capability { get; private set; }

        protected abstract Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier);

        protected abstract Task<SemanticTokensDocument>
            GetSemanticTokensDocument(ITextDocumentIdentifierParams @params);
    }

    [Obsolete(Constants.Proposal)]
    public class SemanticTokensBuilder
    {
        /// <summary>
        /// The window size of the data array
        /// </summary>
        private const int Parition = 5;

        private int _prevLine;
        private int _prevChar;
        private readonly SemanticTokensDocument _document;
        private readonly SemanticTokensLegend _legend;
        private readonly ImmutableArray<int>.Builder _data;
        private int _dataLen;

        public SemanticTokensBuilder(
            SemanticTokensDocument document,
            SemanticTokensLegend legend)
        {
            _document = document;
            _legend = legend;
            _data = ImmutableArray<int>.Empty.ToBuilder();
        }

        public void Push(int line, int @char, int length, SemanticTokenTypes? tokenType,
            params SemanticTokenModifiers[] tokenModifiers)
        {
            Push(line, @char, length, _legend.GetTokenTypeIdentity(tokenType),
                _legend.GetTokenModifiersIdentity(tokenModifiers));
        }

        public void Push(int line, int @char, int length, SemanticTokenTypes? tokenType,
            IEnumerable<SemanticTokenModifiers> tokenModifiers)
        {
            Push(line, @char, length, _legend.GetTokenTypeIdentity(tokenType),
                _legend.GetTokenModifiersIdentity(tokenModifiers));
        }

        public void Push(int line, int @char, int length, string tokenType, params string[] tokenModifiers)
        {
            Push(line, @char, length, _legend.GetTokenTypeIdentity(tokenType),
                _legend.GetTokenModifiersIdentity(tokenModifiers));
        }

        public void Push(int line, int @char, int length, string tokenType, IEnumerable<string> tokenModifiers)
        {
            Push(line, @char, length, _legend.GetTokenTypeIdentity(tokenType),
                _legend.GetTokenModifiersIdentity(tokenModifiers));
        }

        public void Push(int line, int @char, int length, int tokenType, int tokenModifiers)
        {
            var pushLine = line;
            var pushChar = @char;
            if (_dataLen > 0)
            {
                pushLine -= _prevLine;
                if (pushLine == 0)
                {
                    pushChar -= _prevChar;
                }
            }

            _dataLen += Parition;
            _data.AddRange(pushLine, pushChar, length, tokenType, tokenModifiers);

            _prevLine = line;
            _prevChar = @char;
        }

        /// <summary>
        /// Apply the changes to the source document
        /// </summary>
        public SemanticTokensDocument Commit()
        {
            _document._dataLen = _dataLen;
            _document._data = _data.ToImmutable();
            return _document;
        }
    }

    [Obsolete(Constants.Proposal)]
    public class SemanticTokensDocument
    {
        private readonly SemanticTokensLegend _legend;

        private Guid _id;

        internal ImmutableArray<int> _data;
        internal int _dataLen;

        private ImmutableArray<int>? _prevData;

        public SemanticTokensDocument(SemanticTokensRegistrationOptions options) : this(options.Legend)
        {
        }

        public SemanticTokensDocument(SemanticTokensLegend legend)
        {
            _legend = legend;
            _prevData = null;
            Initialize();
        }

        private void Initialize()
        {
            _id = Guid.NewGuid();
            _data = new ImmutableArray<int>();
            _dataLen = 0;
        }

        public string Id => _id.ToString();

        public SemanticTokensBuilder Create()
        {
            _prevData = null;
            return new SemanticTokensBuilder(this, _legend);
        }

        public SemanticTokensBuilder Edit(SemanticTokensEditsParams @params)
        {
            if (@params.PreviousResultId == Id)
            {
                _prevData = _data;
            }

            return new SemanticTokensBuilder(this, _legend);
        }

        public SemanticTokens GetSemanticTokens()
        {
            _prevData = null;
            return new SemanticTokens() {
                ResultId = Id,
                Data = _data
            };
        }

        public SemanticTokens GetSemanticTokens(Range range)
        {
            _prevData = null;

            var data = ImmutableArray<int>.Empty.ToBuilder();
            var currentLine = 0;
            var currentCharOffset = 0;
            var capturing = false;
            var innerOffset = 0;
            for (var i = 0; i < _data.Length; i += 5)
            {
                var lineOffset = _data[i];
                currentLine += lineOffset;
                if (lineOffset > 0) currentCharOffset = 0;
                if (!capturing)
                {
                    if (range.Start.Line == currentLine)
                    {
                        var charOffset = _data[i + 1];
                        var length = _data[i + 2];
                        // TODO: Do we want to capture partial tokens?
                        // using Sys|tem.Collections.Generic|
                        //           ^^^ do we want a token for 'tem`?
                        if (currentCharOffset + charOffset >= range.Start.Character)
                        {
                            capturing = true;
                            // var overlap = ((currentCharOffset + charOffset) - range.Start.Character);
                            data.AddRange(0, charOffset, length, _data[i + 3], _data[i + 4]);
                            continue;
                        }

                        if (currentCharOffset + charOffset + length >= range.Start.Character)
                        {
                            capturing = true;
                            var overlap = ((currentCharOffset + charOffset + length) - range.Start.Character);
                            data.AddRange(0, 0, overlap, _data[i + 3], _data[i + 4]);
                            innerOffset = charOffset - overlap;
                            continue;
                        }

                        currentCharOffset += charOffset;
                    }
                }
                else
                {
                    if (range.End.Line == currentLine)
                    {
                        var charOffset = _data[i + 1];
                        var length = _data[i + 2];
                        if (currentCharOffset + charOffset >= range.End.Character)
                        {
                            capturing = false;
                            break;
                        }

                        if (currentCharOffset + charOffset + length >= range.End.Character)
                        {
                            capturing = false;
                            var overlap = ((currentCharOffset + charOffset + length) - range.End.Character);
                            data.AddRange(lineOffset, charOffset, length -  overlap, _data[i + 3], _data[i + 4]);
                            break;
                        }

                        currentCharOffset += charOffset;

                        data.AddRange(_data[i], _data[i + 1], _data[i + 2], _data[i + 3], _data[i + 4]);
                    }
                    else
                    {
                        if (innerOffset > 0)
                        {
                            data.AddRange(_data[i], _data[i + 1] - innerOffset, _data[i + 2], _data[i + 3],
                                _data[i + 4]);
                            innerOffset = 0;
                        }
                        else
                        {
                            data.AddRange(_data[i], _data[i + 1], _data[i + 2], _data[i + 3], _data[i + 4]);
                        }
                    }
                }
            }

            return new SemanticTokens() {
                ResultId = Id,
                Data = data.ToImmutable()
            };
        }

        public SemanticTokensOrSemanticTokensEdits GetSemanticTokensEdits()
        {
            if (!_prevData.HasValue) return GetSemanticTokens();

            var prevData = _prevData.Value;
            var prevDataLength = prevData.Length;
            var dataLength = _data.Length;
            var startIndex = 0;
            while (startIndex < dataLength && startIndex < prevDataLength && prevData[startIndex] ==
                _data[startIndex])
            {
                startIndex++;
            }

            if (startIndex < dataLength && startIndex < prevDataLength)
            {
                // Find end index
                var endIndex = 0;
                while (endIndex < dataLength && endIndex < prevDataLength &&
                       prevData[prevDataLength - 1 - endIndex] == _data[dataLength - 1 - endIndex])
                {
                    endIndex++;
                }

                var newData = ImmutableArray.Create(_data, startIndex, dataLength - endIndex - startIndex);
                var result = new SemanticTokensEdits {
                    ResultId = Id,
                    Edits = new[] {
                        new SemanticTokensEdit {
                            Start = startIndex,
                            DeleteCount = prevDataLength - endIndex - startIndex,
                            Data = newData
                        }
                    }
                };
                return result;
            }

            if (startIndex < dataLength)
            {
                return new SemanticTokensEdits {
                    ResultId = Id, Edits = new[] {
                        new SemanticTokensEdit {
                            Start = startIndex,
                            DeleteCount = 0,
                            Data = ImmutableArray.Create(_data, startIndex, _dataLen - startIndex)
                        }
                    }
                };
            }

            if (startIndex < prevDataLength)
            {
                return new SemanticTokensEdits {
                    ResultId = Id,
                    Edits = new[] {
                        new SemanticTokensEdit {
                            Start = startIndex,
                            DeleteCount = prevDataLength - startIndex
                        }
                    }
                };
            }

            return new SemanticTokensEdits {
                ResultId = Id, Edits = Array.Empty<SemanticTokensEdit>()
            };
        }
    }

    [Obsolete(Constants.Proposal)]
    public static class SemanticTokensEditsHandlerExtensions
    {
        public static IDisposable OnSemanticTokensEdits(
            this ILanguageServerRegistry registry,
            Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> tokenize,
            Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>> getSemanticTokensDocument,
            SemanticTokensRegistrationOptions registrationOptions = null,
            Action<SemanticTokensCapability> setCapability = null)
        {
            registrationOptions ??= new SemanticTokensRegistrationOptions() {
                DocumentProvider =
                    new Supports<SemanticTokensDocumentProviderOptions>(true,
                        new SemanticTokensDocumentProviderOptions() { })
            };
            registrationOptions.RangeProvider = true;
            if (registrationOptions.DocumentProvider.IsSupported && registrationOptions.DocumentProvider.Value != null)
            {
                registrationOptions.DocumentProvider.Value.Edits = true;
            }

            return registry.AddHandlers(
                new DelegatingHandler(tokenize, getSemanticTokensDocument, setCapability, registrationOptions));
        }

        class DelegatingHandler : SemanticTokensHandler
        {
            private readonly Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> _tokenize;

            private readonly Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>>
                _getSemanticTokensDocument;

            private readonly Action<SemanticTokensCapability> _setCapability;

            public DelegatingHandler(
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> tokenize,
                Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                Action<SemanticTokensCapability> setCapability,
                SemanticTokensRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _tokenize = tokenize;
                _getSemanticTokensDocument = getSemanticTokensDocument;
                _setCapability = setCapability;
            }

            protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier) =>
                _tokenize(builder, identifier);

            protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(
                ITextDocumentIdentifierParams @params) => _getSemanticTokensDocument(@params);

            public override void SetCapability(SemanticTokensCapability capability) =>
                _setCapability?.Invoke(capability);
        }
    }
}

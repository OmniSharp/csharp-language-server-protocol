using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals
{
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

        public void Push(int line, int @char, int length, SemanticTokenType? tokenType,
            params SemanticTokenModifier[] tokenModifiers)
        {
            Push(line, @char, length, _legend.GetTokenTypeIdentity(tokenType),
                _legend.GetTokenModifiersIdentity(tokenModifiers));
        }

        public void Push(int line, int @char, int length, SemanticTokenType? tokenType,
            IEnumerable<SemanticTokenModifier> tokenModifiers)
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
}

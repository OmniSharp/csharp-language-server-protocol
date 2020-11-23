using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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
            SemanticTokensLegend legend
        )
        {
            _document = document;
            _legend = legend;
            _data = ImmutableArray<int>.Empty.ToBuilder();
        }

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <param name="line"></param>
        /// <param name="char"></param>
        /// <param name="length"></param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(int line, int @char, int length, SemanticTokenType? tokenType, params SemanticTokenModifier[] tokenModifiers) => Push(
            line, @char, length, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <remarks>Avoid creating the range just to call this method</remarks>
        /// <param name="range">The range, cannot span multiple lines</param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(Range range, SemanticTokenType? tokenType, params SemanticTokenModifier[] tokenModifiers) => Push(
            range, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <param name="line"></param>
        /// <param name="char"></param>
        /// <param name="length"></param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(int line, int @char, int length, SemanticTokenType? tokenType, IEnumerable<SemanticTokenModifier> tokenModifiers) => Push(
            line, @char, length, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <remarks>Avoid creating the range just to call this method</remarks>
        /// <param name="range">The range, cannot span multiple lines</param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(Range range, SemanticTokenType? tokenType, IEnumerable<SemanticTokenModifier> tokenModifiers) => Push(
            range, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <param name="line"></param>
        /// <param name="char"></param>
        /// <param name="length"></param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(int line, int @char, int length, string tokenType, params string[] tokenModifiers) => Push(
            line, @char, length, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <remarks>Avoid creating the range just to call this method</remarks>
        /// <param name="range">The range, cannot span multiple lines</param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(Range range, string tokenType, params string[] tokenModifiers) => Push(
            range, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <param name="line"></param>
        /// <param name="char"></param>
        /// <param name="length"></param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(int line, int @char, int length, string tokenType, IEnumerable<string> tokenModifiers) =>
            Push(
                line, @char, length, _legend.GetTokenTypeIdentity(tokenType),
                _legend.GetTokenModifiersIdentity(tokenModifiers)
            );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <remarks>Avoid creating the range just to call this method</remarks>
        /// <param name="range">The range, cannot span multiple lines</param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(Range range, string tokenType, IEnumerable<string> tokenModifiers) => Push(
            range, _legend.GetTokenTypeIdentity(tokenType), _legend.GetTokenModifiersIdentity(tokenModifiers)
        );

        /// <summary>
        /// Push a token onto the builder
        /// </summary>
        /// <param name="line"></param>
        /// <param name="char"></param>
        /// <param name="length"></param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
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
        /// Push a token onto the builder
        /// </summary>
        /// <remarks>Avoid creating the range just to call this method</remarks>
        /// <param name="range">The range, cannot span multiple lines</param>
        /// <param name="tokenType"></param>
        /// <param name="tokenModifiers"></param>
        public void Push(Range range, int tokenType, int tokenModifiers)
        {
            if (range.Start.Line != range.End.Line) throw new ArgumentOutOfRangeException(nameof(range), "Range must not span multiple lines");
            Push(range.Start.Line, range.Start.Character, range.End.Character - range.Start.Character, tokenType, tokenModifiers);
        }

        /// <summary>
        /// Apply the changes to the source document
        /// </summary>
        public SemanticTokensDocument Commit()
        {
            _document.DataLen = _dataLen;
            _document.Data = _data.ToImmutable();
            return _document;
        }
    }
}

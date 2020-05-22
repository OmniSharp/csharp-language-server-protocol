using System;
using System.Collections.Immutable;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals
{
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
                            data.AddRange(lineOffset, charOffset, length - overlap, _data[i + 3], _data[i + 4]);
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
}

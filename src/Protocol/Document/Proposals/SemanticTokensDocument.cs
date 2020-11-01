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

        internal ImmutableArray<int> Data;
        internal int DataLen;

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
            Data = new ImmutableArray<int>();
            DataLen = 0;
        }

        public string Id => _id.ToString();

        public SemanticTokensBuilder Create()
        {
            _prevData = null;
            return new SemanticTokensBuilder(this, _legend);
        }

        public SemanticTokensBuilder Edit(SemanticTokensDeltaParams @params)
        {
            if (@params.PreviousResultId == Id)
            {
                _prevData = Data;
            }

            return new SemanticTokensBuilder(this, _legend);
        }

        public SemanticTokens GetSemanticTokens()
        {
            _prevData = null;
            return new SemanticTokens {
                ResultId = Id,
                Data = Data
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
            for (var i = 0; i < Data.Length; i += 5)
            {
                var lineOffset = Data[i];
                currentLine += lineOffset;
                if (lineOffset > 0) currentCharOffset = 0;
                if (!capturing)
                {
                    if (range.Start.Line == currentLine)
                    {
                        var charOffset = Data[i + 1];
                        var length = Data[i + 2];
                        // TODO: Do we want to capture partial tokens?
                        // using Sys|tem.Collections.Generic|
                        //           ^^^ do we want a token for 'tem`?
                        if (currentCharOffset + charOffset >= range.Start.Character)
                        {
                            capturing = true;
                            // var overlap = ((currentCharOffset + charOffset) - range.Start.Character);
                            data.AddRange(0, charOffset, length, Data[i + 3], Data[i + 4]);
                            continue;
                        }

                        if (currentCharOffset + charOffset + length >= range.Start.Character)
                        {
                            capturing = true;
                            var overlap = currentCharOffset + charOffset + length - range.Start.Character;
                            data.AddRange(0, 0, overlap, Data[i + 3], Data[i + 4]);
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
                        var charOffset = Data[i + 1];
                        var length = Data[i + 2];
                        if (currentCharOffset + charOffset >= range.End.Character)
                        {
                            break;
                        }

                        if (currentCharOffset + charOffset + length >= range.End.Character)
                        {
                            var overlap = currentCharOffset + charOffset + length - range.End.Character;
                            data.AddRange(lineOffset, charOffset, length - overlap, Data[i + 3], Data[i + 4]);
                            break;
                        }

                        currentCharOffset += charOffset;

                        data.AddRange(Data[i], Data[i + 1], Data[i + 2], Data[i + 3], Data[i + 4]);
                    }
                    else
                    {
                        if (innerOffset > 0)
                        {
                            data.AddRange(
                                Data[i], Data[i + 1] - innerOffset, Data[i + 2], Data[i + 3],
                                Data[i + 4]
                            );
                            innerOffset = 0;
                        }
                        else
                        {
                            data.AddRange(Data[i], Data[i + 1], Data[i + 2], Data[i + 3], Data[i + 4]);
                        }
                    }
                }
            }

            return new SemanticTokens {
                ResultId = Id,
                Data = data.ToImmutable()
            };
        }

        public SemanticTokensFullOrDelta GetSemanticTokensEdits()
        {
            if (!_prevData.HasValue) return GetSemanticTokens();

            var prevData = _prevData.Value;
            var prevDataLength = prevData.Length;
            var dataLength = Data.Length;
            var startIndex = 0;
            while (startIndex < dataLength && startIndex < prevDataLength && prevData[startIndex] ==
                Data[startIndex])
            {
                startIndex++;
            }

            if (startIndex < dataLength && startIndex < prevDataLength)
            {
                // Find end index
                var endIndex = 0;
                while (endIndex < dataLength && endIndex < prevDataLength &&
                       prevData[prevDataLength - 1 - endIndex] == Data[dataLength - 1 - endIndex])
                {
                    endIndex++;
                }

                var newData = ImmutableArray.Create(Data, startIndex, dataLength - endIndex - startIndex);
                var result = new SemanticTokensDelta {
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
                return new SemanticTokensDelta {
                    ResultId = Id, Edits = new[] {
                        new SemanticTokensEdit {
                            Start = startIndex,
                            DeleteCount = 0,
                            Data = ImmutableArray.Create(Data, startIndex, DataLen - startIndex)
                        }
                    }
                };
            }

            if (startIndex < prevDataLength)
            {
                return new SemanticTokensDelta {
                    ResultId = Id,
                    Edits = new[] {
                        new SemanticTokensEdit {
                            Start = startIndex,
                            DeleteCount = prevDataLength - startIndex
                        }
                    }
                };
            }

            return new SemanticTokensDelta {
                ResultId = Id, Edits = Array.Empty<SemanticTokensEdit>()
            };
        }
    }
}

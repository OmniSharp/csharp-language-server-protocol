using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    public class PipelinesBased
    {
        public PipelinesBased(PipeReader pipeReader)
        {
            _pipeReader = pipeReader;
            _contentLengthData = "Content-Length".Select(x => (byte) x).ToArray();
            _headersBuffer = new Memory<byte>(new byte[HeadersFinishedLength]);
            _contentLengthBuffer = new Memory<byte>(new byte[ContentLengthLength]);
            _contentLengthValueBuffer = new byte[20]; // Max string length of the long value
            _contentLengthValueMemory =
                new Memory<byte>(_contentLengthValueBuffer); // Max string length of the long value
        }


        public static readonly byte[] HeadersFinished =
            new byte[] {(byte) '\r', (byte) '\n', (byte) '\r', (byte) '\n'}.ToArray();

        public const int HeadersFinishedLength = 4;
        public static readonly char[] HeaderKeys = {'\r', '\n', ':'};
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"
        public static readonly byte[] ContentLength = "Content-Length".Select(x => (byte) x).ToArray();
        public static readonly int ContentLengthLength = 14;

        private readonly PipeReader _pipeReader;
        private readonly Memory<byte> _headersBuffer;
        private readonly Memory<byte> _contentLengthBuffer;
        private readonly byte[] _contentLengthValueBuffer;
        private readonly Memory<byte> _contentLengthValueMemory;
        private byte[] _contentLengthData;

        private bool TryParseHeaders(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            // TODO: This might be simplified with SequenceReader...
            // Not sure we can move to purely .netstandard 2.1
            if (buffer.Length < MinBuffer || buffer.Length < HeadersFinishedLength)
            {
                line = default;
                return false;
            }

            var rentedSpan = _headersBuffer.Span;

            var start = buffer.PositionOf((byte) '\r');
            do
            {
                if (!start.HasValue)
                {
                    line = default;
                    return false;
                }

                var next = buffer.Slice(start.Value, buffer.GetPosition(4, start.Value));
                next.CopyTo(rentedSpan);
                if (IsEqual(rentedSpan, HeadersFinished))
                {
                    line = buffer.Slice(0, next.End);
                    buffer = buffer.Slice(next.End);
                    return true;
                }

                start = buffer.Slice(buffer.GetPosition(HeadersFinishedLength, start.Value)).PositionOf((byte) '\r');
            } while (start.HasValue && buffer.Length > MinBuffer);

            line = default;
            return false;
        }

        static bool IsEqual(in Span<byte> headers, in byte[] bytes)
        {
            var isEqual = true;
            var len = bytes.Length;
            for (var i = 0; i < len; i++)
            {
                if (bytes[i] == headers[i]) continue;
                isEqual = false;
                break;
            }

            return isEqual;
        }

        private bool TryParseBodyString(in long length, ref ReadOnlySequence<byte> buffer,
            out ReadOnlySequence<byte> line)
        {
            if (buffer.Length < length)
            {
                line = default;
                return false;
            }


            line = buffer.Slice(0, length);
            buffer = buffer.Slice(length);
            return true;
        }

        bool TryParseContentLength(ref ReadOnlySequence<byte> buffer, out long length)
        {
            do
            {
                var colon = buffer.PositionOf((byte) ':');
                if (!colon.HasValue)
                {
                    length = -1;
                    return false;
                }

                var slice = buffer.Slice(0, colon.Value);
                slice.CopyTo(_contentLengthBuffer.Span);

                if (IsEqual(_contentLengthBuffer.Span, ContentLength))
                {
                    var position = buffer.GetPosition(1, colon.Value);
                    var offset = 1;

                    while (buffer.TryGet(ref position, out var memory, true) && !memory.Span.IsEmpty)
                    {
                        foreach (var t in memory.Span)
                        {
                            if (t == (byte) ' ')
                            {
                                offset++;
                                continue;
                            }

                            break;
                        }
                    }

                    var lengthSlice = buffer.Slice(
                        buffer.GetPosition(offset, colon.Value),
                        buffer.PositionOf((byte) '\r') ?? buffer.End
                    );

                    var whitespacePosition = lengthSlice.PositionOf((byte) ' ');
                    if (whitespacePosition.HasValue)
                    {
                        lengthSlice = lengthSlice.Slice(0, whitespacePosition.Value);
                    }

                    lengthSlice.CopyTo(_contentLengthValueMemory.Span);
                    if (long.TryParse(Encoding.ASCII.GetString(_contentLengthValueBuffer), out length))
                    {
                        // Reset the array otherwise smaller numbers will be inflated;
                        for (var i = 0; i < lengthSlice.Length; i++) _contentLengthValueMemory.Span[i] = 0;
                        return true;
                    }
                    // Reset the array otherwise smaller numbers will be inflated;
                    for (var i = 0; i < lengthSlice.Length; i++) _contentLengthValueMemory.Span[i] = 0;

                    // _logger.LogError("Unable to get length from content length header...");
                    return false;
                }
                else
                {
                    buffer = buffer.Slice(buffer.GetPosition(1, buffer.PositionOf((byte) '\n') ?? buffer.End));
                }
            } while (true);
        }

        internal async Task ProcessInputStream(CancellationToken cancellationToken)
        {
            // some time to attach a debugger
            // System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

            var headersParsed = false;
            long length = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _pipeReader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;

                if (!headersParsed)
                {
                    if (TryParseHeaders(ref buffer, out var line))
                    {
                        if (TryParseContentLength(ref line, out length))
                        {
                            headersParsed = true;
                        }
                    }
                }

                if (headersParsed && length == 0)
                {
                    HandleRequest(new ReadOnlySequence<byte>(Array.Empty<byte>()));
                    headersParsed = false;
                }

                if (headersParsed)
                {
                    if (TryParseBodyString(length, ref buffer, out var line))
                    {
                        headersParsed = false;
                        length = 0;
                        HandleRequest(line);
                    }
                }

                _pipeReader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming.
                if (result.IsCompleted && buffer.GetPosition(0, buffer.Start).Equals(buffer.GetPosition(0, buffer.End)))
                {
                    break;
                }
            }
        }

        private void HandleRequest(in ReadOnlySequence<byte> request)
        {
        }
    }
}

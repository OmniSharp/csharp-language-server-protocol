using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Pipeline
{
    public class ClassicHandler
    {
        private readonly Stream _input;
        public const char CR = '\r';
        public const char LF = '\n';
        public static char[] CRLF = {CR, LF};
        public static char[] HeaderKeys = {CR, LF, ':'};
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"

        public ClassicHandler(Stream input)
        {
            _input = input;
        }

        // don't be async: We already allocated a seperate thread for this.
        public Task ProcessInputStream()
        {
            // some time to attach a debugger
            // System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

            // header is encoded in ASCII
            // "Content-Length: 0" counts bytes for the following content
            // content is encoded in UTF-8
            while (_input.CanRead)
            {
                var buffer = new byte[300];
                var current = _input.Read(buffer, 0, MinBuffer);
                if (current == 0) return Task.CompletedTask; // no more _input
                while (current < MinBuffer ||
                       buffer[current - 4] != CR || buffer[current - 3] != LF ||
                       buffer[current - 2] != CR || buffer[current - 1] != LF)
                {
                    var n = _input.Read(buffer, current, 1);
                    if (n == 0) return Task.CompletedTask; // no more _input, mitigates endless loop here.
                    current += n;
                }

                var headersContent = System.Text.Encoding.ASCII.GetString(buffer, 0, current);
                var headers = headersContent.Split(HeaderKeys, StringSplitOptions.RemoveEmptyEntries);
                long length = 0;
                for (var i = 1; i < headers.Length; i += 2)
                {
                    // starting at i = 1 instead of 0 won't throw, if we have uneven headers' length
                    var header = headers[i - 1];
                    var value = headers[i].Trim();
                    if (header.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        length = 0;
                        long.TryParse(value, out length);
                    }
                }

                if (length == 0 || length >= int.MaxValue)
                {
                    HandleRequest(string.Empty, CancellationToken.None);
                }
                else
                {
                    var requestBuffer = new byte[length];
                    var received = 0;
                    while (received < length)
                    {
                        var n = _input.Read(requestBuffer, received, requestBuffer.Length - received);
                        if (n == 0) return Task.CompletedTask; // no more _input
                        received += n;
                    }

                    // TODO sometimes: encoding should be based on the respective header (including the wrong "utf8" value)
                    var payload = System.Text.Encoding.ASCII.GetString(requestBuffer);
                    HandleRequest(payload, CancellationToken.None);
                }
            }

            return Task.CompletedTask;
        }

        private void HandleRequest(string request, CancellationToken cancellationToken)
        {
        }
    }
}

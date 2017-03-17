using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRPC
{
    public class InputHandler : IDisposable
    {
        public const char CR = '\r';
        public const char LF = '\n';
        public static char[] CRLF = { CR, LF };
        public static char[] HeaderKeys = { CR, LF, ':' };
        public const short MinBuffer = 21; // Minimum size of the buffer "Content-Length: X\r\n\r\n"

        private readonly TextReader _input;
        private readonly Action<string> _inputHandler;
        private readonly Thread _thread;

        public InputHandler(TextReader input, Action<string> inputHandler)
        {
            _input = input;
            _inputHandler = inputHandler;

            _thread = new Thread(async () => await Loop()) {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            _thread.Start();
        }

        private async Task Loop()
        {
            while (true)
            {
                var buffer = new char[200];
                var current = await _input.ReadBlockAsync(buffer, 0, MinBuffer);
                while (current < MinBuffer || buffer[current - 4] != CR || buffer[current - 3] != LF ||
                       buffer[current - 2] != CR || buffer[current - 1] != LF)
                {
                    current += await _input.ReadBlockAsync(buffer, 0, 1);
                }

                var headersContent = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(buffer, 0, current));
                var headers = headersContent.Split(HeaderKeys, StringSplitOptions.RemoveEmptyEntries);
                long length = 0;
                for (var i = 0; i < headers.Length; i += 2)
                {
                    var header = headers[0];
                    var value = headers[i + 1].Trim();
                    if (header.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        length = long.Parse(value);
                    }
                }

                var requestBuffer = new char[length];

                await _input.ReadBlockAsync(requestBuffer, 0, requestBuffer.Length);

                var payload = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(requestBuffer));
                
                _inputHandler(payload);
            }
        }

        public void Dispose()
        {
            _thread.Abort();
        }
    }
}
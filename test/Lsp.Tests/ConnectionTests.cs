using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JsonRPC;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Lsp.Tests
{
    public class InputHandlerTests
    {
        [Fact]
        public async Task Test1()
        {
            var inputStream = new MemoryStream(Encoding.ASCII.GetBytes( "Content-Length: 2\r\n\r\n{}"));
            var inputWriter = new StreamWriter(inputStream);

            var cts = new CancellationTokenSource();
            using (var inputHandler = new InputHandler(new StreamReader(inputStream), (token) => {
                token.Should().BeEquivalentTo(JObject.Parse("{}"));
                cts.Cancel();
            }))
            {
                cts.Wait();
            }
        }
    }

    public static class TestExtensions {
        public static void Wait(this CancellationTokenSource cancellationTokenSource)
        {
            cancellationTokenSource.Token.WaitHandle.WaitOne();
        }
    }
}
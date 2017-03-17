using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace JsonRpc.Tests
{
    public class InputHandlerTests
    {
        [Fact]
        public void ShouldReturnSimpleRequest()
        {
            var inputStream = new MemoryStream(Encoding.ASCII.GetBytes("Content-Length: 2\r\n\r\n{}"));
            var inputWriter = new StreamWriter(inputStream);

            var cts = new CancellationTokenSource();
            using (var inputHandler = new InputHandler(new StreamReader(inputStream), (token) => {
                token.Should().BeEquivalentTo("{}");
                cts.Cancel();
            }))
            {
                cts.Wait();
            }
        }
    }
}
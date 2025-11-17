using System.IO.Pipelines;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit.Abstractions;

namespace Lsp.Tests.Testing
{
    public class LanguageServerTestBaseTests : LanguageServerTestBase
    {
        public LanguageServerTestBaseTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        protected override (Stream clientOutput, Stream serverInput) SetupServer()
        {
            var clientPipe = new Pipe(TestOptions.DefaultPipeOptions);
            var serverPipe = new Pipe(TestOptions.DefaultPipeOptions);

            var server = LanguageServer.PreInit(options => { options.WithInput(serverPipe.Reader).WithOutput(clientPipe.Writer); });

            server.Initialize(CancellationToken);

            return ( clientPipe.Reader.AsStream(), serverPipe.Writer.AsStream() );
        }

        [Fact]
        public async Task Should_Connect_To_Server()
        {
            var client = await InitializeClient(x => { });

            client.ClientSettings.Should().NotBeNull();
            client.ServerSettings.Should().NotBeNull();
        }
    }
}

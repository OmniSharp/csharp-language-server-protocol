using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class LanguageServerTests : AutoTestBase
    {
        public LanguageServerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task Works_With_IWorkspaceSymbolsHandler()
        {
            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var client = new LanguageClient(LoggerFactory, process);

            var handler = Substitute.For<IWorkspaceSymbolsHandler>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000 * 60 * 5);

            var serverStart = LanguageServer.From(x => x
                //.WithHandler(handler)
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .WithLoggerFactory(LoggerFactory)
                .AddDefaultLoggingProvider()
                .WithMinimumLogLevel(LogLevel.Trace),
                cts.Token
            );

            await Task.WhenAll(
                client.Initialize(
                    Directory.GetCurrentDirectory(),
                    new object(),
                    cts.Token),
                serverStart
            );
            var server = await serverStart;
            server.AddHandlers(handler);
        }
    }
}

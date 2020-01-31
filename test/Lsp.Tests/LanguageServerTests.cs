using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Client.Processes;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

namespace Lsp.Tests
{
    public class LanguageServerTests : AutoTestBase
    {
        public LanguageServerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact(Skip = "Doesn't work in CI :(")]
        public async Task Works_With_IWorkspaceSymbolsHandler()
        {
            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var client = new LanguageClient(LoggerFactory, process);

            var handler = Substitute.For<IWorkspaceSymbolsHandler>();
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000 * 60 * 5);

            var serverStart = LanguageServer.From(x => x
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .ConfigureLogging(z => z.Services.AddSingleton(LoggerFactory)),
                cts.Token
            );

            await Task.WhenAll(
                client.Initialize(
                    Directory.GetCurrentDirectory(),
                    new object(),
                    cts.Token),
                serverStart
            );
            using var server = await serverStart;
            server.AddHandlers(handler);
        }

        [Fact]
        public async Task GH141_CrashesWithEmptyInitializeParams()
        {
            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var server = LanguageServer.PreInit(x => x
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .ConfigureLogging(z => z.Services.AddSingleton(LoggerFactory))
                .AddHandlers(TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp"))
            ) as IRequestHandler<InitializeParams, InitializeResult>;

            var handler = server as IRequestHandler<InitializeParams, InitializeResult>;

            Func<Task> a = async () => await handler.Handle(new InitializeParams() { }, CancellationToken.None);
            a.Should().NotThrow();
        }

        [Fact]
        public async Task TriggersStartedTask()
        {
            var startupInterface = Substitute.For(new [] {typeof(IOnStarted), typeof(IDidChangeConfigurationHandler) }, Array.Empty<object>()) as IOnStarted;
            var startedDelegate = Substitute.For<StartedDelegate>();
            startedDelegate(Arg.Any<ILanguageServer>(), Arg.Any<InitializeResult>()).Returns(Task.CompletedTask);
            var process = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), LoggerFactory);
            await process.Start();
            var client = new LanguageClient(LoggerFactory, process);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(15));
            var serverStart = LanguageServer.From(x => x
                .OnStarted(startedDelegate)
                .OnStarted(startedDelegate)
                .OnStarted(startedDelegate)
                .OnStarted(startedDelegate)
                .WithHandler((IDidChangeConfigurationHandler) startupInterface)
                .WithInput(process.ClientOutputStream)
                .WithOutput(process.ClientInputStream)
                .ConfigureLogging(z => z.Services.AddSingleton(LoggerFactory))
                .AddHandlers(TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp"))
            , cts.Token);

            await Task.WhenAll(
                client.Initialize(
                    Directory.GetCurrentDirectory(),
                    new object(),
                    cts.Token),
                serverStart
            );
            using var server = await serverStart;

            _ = startedDelegate.Received(4)(Arg.Any<ILanguageServer>(), Arg.Any<InitializeResult>());
            _ = startupInterface.Received(1).OnStarted(Arg.Any<ILanguageServer>(), Arg.Any<InitializeResult>());
        }
    }
}

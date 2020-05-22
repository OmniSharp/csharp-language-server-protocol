using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class RequestCancellationTests : LanguageProtocolTestBase
    {
        public RequestCancellationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions()
            .ConfigureForXUnit(outputHelper)
            .WithSettleTimeSpan(TimeSpan.FromMilliseconds(200))
        )
        {
        }

        [Fact]
        public async Task Should_Cancel_Pending_Requests()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(10));
            CancellationToken.Register(cts.Cancel);
            Func<Task<CompletionList>> action = () => client.TextDocument.RequestCompletion(new CompletionParams() {
                TextDocument = "/a/file.cs"
            }, cts.Token).AsTask();
            action.Should().Throw<TaskCanceledException>();
        }

        [Fact]
        public async Task Should_Abandon_Pending_Requests_For_Text_Changes()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            var request1 = client.TextDocument.RequestCompletion(new CompletionParams() {
                TextDocument = "/a/file.cs"
            }, CancellationToken).AsTask();

            client.TextDocument.DidChangeTextDocument(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier() {
                    Uri = "/a/file.cs",
                    Version = 123,
                },
                ContentChanges = new Container<TextDocumentContentChangeEvent>()
            });

            await SettleNext();

            Func<Task> action = () => request1;
            action.Should().Throw<ContentModifiedException>();
        }

        [Fact]
        public async Task Should_Cancel_Requests_After_Timeout()
        {
            var (client, server) = await Initialize(ConfigureClient, x => {
                ConfigureServer(x);
                x.WithMaximumRequestTimeout(TimeSpan.FromMilliseconds(500));
            });

            Func<Task> action = () => client.TextDocument.RequestCompletion(new CompletionParams() {
                TextDocument = "/a/file.cs"
            }, CancellationToken).AsTask();
            action.Should().Throw<RequestCancelledException>();
        }

        [Fact]
        public async Task Should_Cancel_Requests_After_Timeout_without_Content_Modified()
        {
            var (client, server) = await Initialize(ConfigureClient, x => {
                ConfigureServer(x);
                x.WithContentModifiedSupport(false).WithMaximumRequestTimeout(TimeSpan.FromMilliseconds(500));
            });

            Func<Task> action = () => client.TextDocument.RequestCompletion(new CompletionParams() {
                TextDocument = "/a/file.cs"
            }, CancellationToken).AsTask();
            action.Should().Throw<RequestCancelledException>();
        }

        [Fact]
        public async Task Can_Publish_Diagnostics_Delayed()
        {
            var (client, server) = await Initialize(ConfigureClient, x => {
                ConfigureServer(x);
                x.WithMaximumRequestTimeout(TimeSpan.FromMilliseconds(10000));
            });

            server.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams() {
                Diagnostics = new Container<Diagnostic>(new Diagnostic() {
                    Message = "asdf",
                }),
                Uri = DocumentUri.File("/from/file"),
                Version = 1
            });

            await SettleNext();

            await Task.Delay(1000);

            _diagnostics.Should().HaveCount(1);
        }

        private readonly ConcurrentDictionary<DocumentUri, IEnumerable<Diagnostic>> _diagnostics = new ConcurrentDictionary<DocumentUri, IEnumerable<Diagnostic>>();

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.OnPublishDiagnostics(async (request, ct) => {
                try
                {
                    TestOptions.ClientLoggerFactory.CreateLogger("test").LogCritical("start");
                    await Task.Delay(500, ct);
                    _diagnostics.AddOrUpdate(request.Uri, (a) => request.Diagnostics, (a, b) => request.Diagnostics);
                }
                catch (Exception e)
                {
                    TestOptions.ClientLoggerFactory.CreateLogger("test").LogCritical(e, "error");
                }
            });
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.WithContentModifiedSupport(true);
            options.OnCompletion(async (x, ct) => {
                await Task.Delay(50000, ct);
                return new CompletionList();
            }, new CompletionRegistrationOptions());
            options.OnDidChangeTextDocument(async x => { await Task.Delay(20); }, new TextDocumentChangeRegistrationOptions());
        }
    }
}

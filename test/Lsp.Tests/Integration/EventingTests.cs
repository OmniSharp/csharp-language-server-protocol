using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using Arg = NSubstitute.Arg;
// ReSharper disable SuspiciousTypeConversion.Global

namespace Lsp.Tests.Integration
{
    public class EventingTests : LanguageProtocolTestBase
    {
        public EventingTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported()
        {
            var onLanguageClientInitialize = Substitute.For<IOnLanguageClientInitialize>();
            var onLanguageServerInitialize = Substitute.For<IOnLanguageServerInitialize>();
            var (client, server) = await Initialize(
                options => options.Services.AddSingleton(onLanguageClientInitialize),
                options => options.Services.AddSingleton(onLanguageServerInitialize)
            );

            await onLanguageClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Delegate_Is_Supported()
        {
            var onLanguageClientInitialize = Substitute.For<OnLanguageClientInitializeDelegate>();
            var onLanguageServerInitialize = Substitute.For<OnLanguageServerInitializeDelegate>();
            var (client, server) = await Initialize(
                options => options.OnInitialize(onLanguageClientInitialize),
                options => options.OnInitialize(onLanguageServerInitialize)
            );

            await onLanguageClientInitialize.Received(1).Invoke(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(1).Invoke(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported_On_Handlers()
        {
            var onLanguageClientInitialize =
                (IOnLanguageClientInitialize) Substitute.For(new[] { typeof(IOnLanguageClientInitialize), typeof(PublishDiagnosticsHandlerBase) }, Array.Empty<object>());
            var onLanguageServerInitialize =
                (IOnLanguageServerInitialize) Substitute.For(new[] { typeof(IOnLanguageServerInitialize), typeof(CompletionHandlerBase) }, new object[] {  });
            var (client, server) = await Initialize(
                options => options.AddHandler((IJsonRpcHandler) onLanguageClientInitialize!),
                options => options.AddHandler((IJsonRpcHandler) onLanguageServerInitialize!)
            );

            await onLanguageClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onLanguageClientInitialize =
                (IOnLanguageClientInitialize) Substitute.For(new[] { typeof(IOnLanguageClientInitialize), typeof(PublishDiagnosticsHandlerBase) }, Array.Empty<object>());
            var onLanguageServerInitialize =
                (IOnLanguageServerInitialize) Substitute.For(new[] { typeof(IOnLanguageServerInitialize), typeof(CompletionHandlerBase) }, new object[] {  });
            var (client, server) = await Initialize(o => { }, o => { });

            await onLanguageClientInitialize.Received(0).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(0).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler((IJsonRpcHandler) onLanguageClientInitialize!));
            server.Register(r => r.AddHandler((IJsonRpcHandler) onLanguageServerInitialize!));

            await onLanguageClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported()
        {
            var onLanguageClientInitialized = Substitute.For<IOnLanguageClientInitialized>();
            var onLanguageServerInitialized = Substitute.For<IOnLanguageServerInitialized>();
            var (client, server) = await Initialize(
                options => options.Services.AddSingleton(onLanguageClientInitialized),
                options => options.Services.AddSingleton(onLanguageServerInitialized)
            );

            await onLanguageClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Delegate_Is_Supported()
        {
            var onLanguageClientInitialized = Substitute.For<OnLanguageClientInitializedDelegate>();
            var onLanguageServerInitialized = Substitute.For<OnLanguageServerInitializedDelegate>();
            var (client, server) = await Initialize(
                options => options.OnInitialized(onLanguageClientInitialized),
                options => options.OnInitialized(onLanguageServerInitialized)
            );

            await onLanguageClientInitialized.Received(1).Invoke(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(1).Invoke(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported_On_Handlers()
        {
            var onLanguageClientInitialized =
                (IOnLanguageClientInitialized) Substitute.For(new[] { typeof(IOnLanguageClientInitialized), typeof(PublishDiagnosticsHandlerBase) }, Array.Empty<object>());
            var onLanguageServerInitialized =
                (IOnLanguageServerInitialized) Substitute.For(new[] { typeof(IOnLanguageServerInitialized), typeof(CompletionHandlerBase) }, new object[] { });
            var (client, server) = await Initialize(
                options => options.AddHandler((IJsonRpcHandler) onLanguageClientInitialized!),
                options => options.AddHandler((IJsonRpcHandler) onLanguageServerInitialized!)
            );

            await onLanguageClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onLanguageClientInitialized =
                (IOnLanguageClientInitialized) Substitute.For(new[] { typeof(IOnLanguageClientInitialized), typeof(PublishDiagnosticsHandlerBase) }, Array.Empty<object>());
            var onLanguageServerInitialized =
                (IOnLanguageServerInitialized) Substitute.For(new[] { typeof(IOnLanguageServerInitialized), typeof(CompletionHandlerBase) }, new object[] { });
            var (client, server) = await Initialize(o => { }, o => { });

            await onLanguageClientInitialized.Received(0).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(0).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler((IJsonRpcHandler) onLanguageClientInitialized!));
            server.Register(r => r.AddHandler((IJsonRpcHandler) onLanguageServerInitialized!));

            await onLanguageClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported()
        {
            var onLanguageClientStarted = Substitute.For<IOnLanguageClientStarted>();
            var onLanguageServerStarted = Substitute.For<IOnLanguageServerStarted>();
            var (client, server) = await Initialize(
                options => options.Services.AddSingleton(onLanguageClientStarted),
                options => options.Services.AddSingleton(onLanguageServerStarted)
            );

            await onLanguageClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Delegate_Is_Supported()
        {
            var onLanguageClientStarted = Substitute.For<OnLanguageClientStartedDelegate>();
            var onLanguageServerStarted = Substitute.For<OnLanguageServerStartedDelegate>();
            var (client, server) = await Initialize(
                options => options.OnStarted(onLanguageClientStarted),
                options => options.OnStarted(onLanguageServerStarted)
            );

            await onLanguageClientStarted.Received(1).Invoke(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(1).Invoke(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported_On_Handlers()
        {
            var onLanguageClientStarted =
                (IOnLanguageClientStarted) Substitute.For(new[] { typeof(IOnLanguageClientStarted), typeof(PublishDiagnosticsHandlerBase) }, Array.Empty<object>());
            var onLanguageServerStarted =
                (IOnLanguageServerStarted) Substitute.For(new[] { typeof(IOnLanguageServerStarted), typeof(CompletionHandlerBase) }, new object[] { });
            var (client, server) = await Initialize(
                options => options.AddHandler((IJsonRpcHandler) onLanguageClientStarted!),
                options => options.AddHandler((IJsonRpcHandler) onLanguageServerStarted!)
            );

            await onLanguageClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onLanguageClientStarted =
                (IOnLanguageClientStarted) Substitute.For(new[] { typeof(IOnLanguageClientStarted), typeof(PublishDiagnosticsHandlerBase) }, Array.Empty<object>());
            var onLanguageServerStarted =
                (IOnLanguageServerStarted) Substitute.For(new[] { typeof(IOnLanguageServerStarted), typeof(CompletionHandlerBase) }, new object[] { });
            var (client, server) = await Initialize(o => { }, o => { });

            await onLanguageClientStarted.Received(0).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(0).OnStarted(server, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler((IJsonRpcHandler) onLanguageClientStarted!));
            server.Register(r => r.AddHandler((IJsonRpcHandler) onLanguageServerStarted!));

            await onLanguageClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }
    }
}

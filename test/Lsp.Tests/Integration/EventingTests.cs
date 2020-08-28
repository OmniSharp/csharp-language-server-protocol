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
                Substitute.For(new[] { typeof(IOnLanguageClientInitialize), typeof(PublishDiagnosticsHandler) }, Array.Empty<object>()) as IOnLanguageClientInitialize;
            var onLanguageServerInitialize =
                Substitute.For(new[] { typeof(IOnLanguageServerInitialize), typeof(CompletionHandler) }, new object[] { new CompletionRegistrationOptions() }) as
                    IOnLanguageServerInitialize;
            var (client, server) = await Initialize(
                options => options.AddHandler(onLanguageClientInitialize as IJsonRpcHandler),
                options => options.AddHandler(onLanguageServerInitialize as IJsonRpcHandler)
            );

            await onLanguageClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onLanguageClientInitialize =
                Substitute.For(new[] { typeof(IOnLanguageClientInitialize), typeof(PublishDiagnosticsHandler) }, Array.Empty<object>()) as IOnLanguageClientInitialize;
            var onLanguageServerInitialize =
                Substitute.For(new[] { typeof(IOnLanguageServerInitialize), typeof(CompletionHandler) }, new object[] { new CompletionRegistrationOptions() }) as
                    IOnLanguageServerInitialize;
            var (client, server) = await Initialize(o => { }, o => { });

            await onLanguageClientInitialize.Received(0).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialize.Received(0).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler(onLanguageClientInitialize as IJsonRpcHandler));
            server.Register(r => r.AddHandler(onLanguageServerInitialize as IJsonRpcHandler));

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
                Substitute.For(new[] { typeof(IOnLanguageClientInitialized), typeof(PublishDiagnosticsHandler) }, Array.Empty<object>()) as IOnLanguageClientInitialized;
            var onLanguageServerInitialized =
                Substitute.For(new[] { typeof(IOnLanguageServerInitialized), typeof(CompletionHandler) }, new object[] { new CompletionRegistrationOptions() }) as
                    IOnLanguageServerInitialized;
            var (client, server) = await Initialize(
                options => options.AddHandler(onLanguageClientInitialized as IJsonRpcHandler),
                options => options.AddHandler(onLanguageServerInitialized as IJsonRpcHandler)
            );

            await onLanguageClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onLanguageClientInitialized =
                Substitute.For(new[] { typeof(IOnLanguageClientInitialized), typeof(PublishDiagnosticsHandler) }, Array.Empty<object>()) as IOnLanguageClientInitialized;
            var onLanguageServerInitialized =
                Substitute.For(new[] { typeof(IOnLanguageServerInitialized), typeof(CompletionHandler) }, new object[] { new CompletionRegistrationOptions() }) as
                    IOnLanguageServerInitialized;
            var (client, server) = await Initialize(o => { }, o => { });

            await onLanguageClientInitialized.Received(0).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onLanguageServerInitialized.Received(0).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler(onLanguageClientInitialized as IJsonRpcHandler));
            server.Register(r => r.AddHandler(onLanguageServerInitialized as IJsonRpcHandler));

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
                Substitute.For(new[] { typeof(IOnLanguageClientStarted), typeof(PublishDiagnosticsHandler) }, Array.Empty<object>()) as IOnLanguageClientStarted;
            var onLanguageServerStarted =
                Substitute.For(new[] { typeof(IOnLanguageServerStarted), typeof(CompletionHandler) }, new object[] { new CompletionRegistrationOptions() }) as
                    IOnLanguageServerStarted;
            var (client, server) = await Initialize(
                options => options.AddHandler(onLanguageClientStarted as IJsonRpcHandler),
                options => options.AddHandler(onLanguageServerStarted as IJsonRpcHandler)
            );

            await onLanguageClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onLanguageClientStarted =
                Substitute.For(new[] { typeof(IOnLanguageClientStarted), typeof(PublishDiagnosticsHandler) }, Array.Empty<object>()) as IOnLanguageClientStarted;
            var onLanguageServerStarted =
                Substitute.For(new[] { typeof(IOnLanguageServerStarted), typeof(CompletionHandler) }, new object[] { new CompletionRegistrationOptions() }) as
                    IOnLanguageServerStarted;
            var (client, server) = await Initialize(o => { }, o => { });

            await onLanguageClientStarted.Received(0).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(0).OnStarted(server, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler(onLanguageClientStarted as IJsonRpcHandler));
            server.Register(r => r.AddHandler(onLanguageServerStarted as IJsonRpcHandler));

            await onLanguageClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onLanguageServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }
    }
}

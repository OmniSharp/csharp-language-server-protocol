using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.DebugAdapter.Server;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using OnDebugAdapterServerInitializedDelegate = OmniSharp.Extensions.DebugAdapter.Protocol.Server.OnDebugAdapterServerInitializedDelegate;

namespace Dap.Tests
{
    public class EventingTests : DebugAdapterProtocolTestBase
    {
        public EventingTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported()
        {
            var onDebugAdapterClientInitialize = Substitute.For<IOnDebugAdapterClientInitialize>();
            var onDebugAdapterServerInitialize = Substitute.For<IOnDebugAdapterServerInitialize>();
            var (client, server) = await Initialize(
                options => options.Services.AddSingleton(onDebugAdapterClientInitialize),
                options => options.Services.AddSingleton(onDebugAdapterServerInitialize)
            );

            await onDebugAdapterClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Delegate_Is_Supported()
        {
            var onDebugAdapterClientInitialize = Substitute.For<OnDebugAdapterClientInitializeDelegate>();
            var onDebugAdapterServerInitialize = Substitute.For<OnDebugAdapterServerInitializeDelegate>();
            var (client, server) = await Initialize(
                options => options.OnInitialize(onDebugAdapterClientInitialize),
                options => options.OnInitialize(onDebugAdapterServerInitialize)
            );

            await onDebugAdapterClientInitialize.Received(1).Invoke(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialize.Received(1).Invoke(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported_On_Handlers()
        {
            var onDebugAdapterClientInitialize =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterClientInitialize), typeof(RunInTerminalHandler)}, Array.Empty<object>()) as IOnDebugAdapterClientInitialize;
            var onDebugAdapterServerInitialize =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterServerInitialize), typeof(CompletionsHandler)}, Array.Empty<object>()) as
                    IOnDebugAdapterServerInitialize;
            var (client, server) = await Initialize(
                options => options.AddHandler(onDebugAdapterClientInitialize as IJsonRpcHandler),
                options => options.AddHandler(onDebugAdapterServerInitialize as IJsonRpcHandler));

            await onDebugAdapterClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialize_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onDebugAdapterClientInitialize =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterClientInitialize), typeof(RunInTerminalHandler)}, Array.Empty<object>()) as IOnDebugAdapterClientInitialize;
            var onDebugAdapterServerInitialize =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterServerInitialize), typeof(CompletionsHandler)}, Array.Empty<object>()) as
                    IOnDebugAdapterServerInitialize;
            var (client, server) = await Initialize(o => { }, o => { });

            await onDebugAdapterClientInitialize.Received(0).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialize.Received(0).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler(onDebugAdapterClientInitialize as IJsonRpcHandler));
            server.Register(r => r.AddHandler(onDebugAdapterServerInitialize as IJsonRpcHandler));

            await onDebugAdapterClientInitialize.Received(1).OnInitialize(client, client.ClientSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialize.Received(1).OnInitialize(server, server.ClientSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported()
        {
            var onDebugAdapterClientInitialized = Substitute.For<IOnDebugAdapterClientInitialized>();
            var onDebugAdapterServerInitialized = Substitute.For<IOnDebugAdapterServerInitialized>();
            var (client, server) = await Initialize(
                options => options.Services.AddSingleton(onDebugAdapterClientInitialized),
                options => options.Services.AddSingleton(onDebugAdapterServerInitialized)
            );

            await onDebugAdapterClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Delegate_Is_Supported()
        {
            var onDebugAdapterClientInitialized = Substitute.For<OnDebugAdapterClientInitializedDelegate>();
            var onDebugAdapterServerInitialized = Substitute.For<OnDebugAdapterServerInitializedDelegate>();
            var (client, server) = await Initialize(
                options => options.OnInitialized(onDebugAdapterClientInitialized),
                options => options.OnInitialized(onDebugAdapterServerInitialized)
            );

            await onDebugAdapterClientInitialized.Received(1).Invoke(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialized.Received(1).Invoke(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported_On_Handlers()
        {
            var onDebugAdapterClientInitialized =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterClientInitialized), typeof(RunInTerminalHandler)}, Array.Empty<object>()) as IOnDebugAdapterClientInitialized;
            var onDebugAdapterServerInitialized =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterServerInitialized), typeof(CompletionsHandler)}, Array.Empty<object>()) as
                    IOnDebugAdapterServerInitialized;
            var (client, server) = await Initialize(
                options => options.AddHandler(onDebugAdapterClientInitialized as IJsonRpcHandler),
                options => options.AddHandler(onDebugAdapterServerInitialized as IJsonRpcHandler));

            await onDebugAdapterClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Initialized_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onDebugAdapterClientInitialized =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterClientInitialized), typeof(RunInTerminalHandler)}, Array.Empty<object>()) as IOnDebugAdapterClientInitialized;
            var onDebugAdapterServerInitialized =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterServerInitialized), typeof(CompletionsHandler)}, Array.Empty<object>()) as
                    IOnDebugAdapterServerInitialized;
            var (client, server) = await Initialize(o => { }, o => { });

            await onDebugAdapterClientInitialized.Received(0).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialized.Received(0).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler(onDebugAdapterClientInitialized as IJsonRpcHandler));
            server.Register(r => r.AddHandler(onDebugAdapterServerInitialized as IJsonRpcHandler));

            await onDebugAdapterClientInitialized.Received(1).OnInitialized(client, client.ClientSettings, client.ServerSettings, Arg.Any<CancellationToken>());
            await onDebugAdapterServerInitialized.Received(1).OnInitialized(server, server.ClientSettings, server.ServerSettings, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported()
        {
            var onDebugAdapterClientStarted = Substitute.For<IOnDebugAdapterClientStarted>();
            var onDebugAdapterServerStarted = Substitute.For<IOnDebugAdapterServerStarted>();
            var (client, server) = await Initialize(
                options => options.Services.AddSingleton(onDebugAdapterClientStarted),
                options => options.Services.AddSingleton(onDebugAdapterServerStarted)
            );

            await onDebugAdapterClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onDebugAdapterServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Delegate_Is_Supported()
        {
            var onDebugAdapterClientStarted = Substitute.For<OnDebugAdapterClientStartedDelegate>();
            var onDebugAdapterServerStarted = Substitute.For<OnDebugAdapterServerStartedDelegate>();
            var (client, server) = await Initialize(
                options => options.OnStarted(onDebugAdapterClientStarted),
                options => options.OnStarted(onDebugAdapterServerStarted)
            );

            await onDebugAdapterClientStarted.Received(1).Invoke(client, Arg.Any<CancellationToken>());
            await onDebugAdapterServerStarted.Received(1).Invoke(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported_On_Handlers()
        {
            var onDebugAdapterClientStarted =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterClientStarted), typeof(RunInTerminalHandler)}, Array.Empty<object>()) as IOnDebugAdapterClientStarted;
            var onDebugAdapterServerStarted =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterServerStarted), typeof(CompletionsHandler)}, Array.Empty<object>()) as
                    IOnDebugAdapterServerStarted;
            var (client, server) = await Initialize(o => { }, o => { });

            await onDebugAdapterClientStarted.Received(0).OnStarted(client, Arg.Any<CancellationToken>());
            await onDebugAdapterServerStarted.Received(0).OnStarted(server, Arg.Any<CancellationToken>());

            client.Register(r => r.AddHandler(onDebugAdapterClientStarted as IJsonRpcHandler));
            server.Register(r => r.AddHandler(onDebugAdapterServerStarted as IJsonRpcHandler));

            await onDebugAdapterClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onDebugAdapterServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Started_Interface_Is_Supported_On_Handlers_After_Startup()
        {
            var onDebugAdapterClientStarted =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterClientStarted), typeof(RunInTerminalHandler)}, Array.Empty<object>()) as IOnDebugAdapterClientStarted;
            var onDebugAdapterServerStarted =
                Substitute.For(new Type[] {typeof(IOnDebugAdapterServerStarted), typeof(CompletionsHandler)}, Array.Empty<object>()) as
                    IOnDebugAdapterServerStarted;
            var (client, server) = await Initialize(
                options => options.AddHandler(onDebugAdapterClientStarted as IJsonRpcHandler),
                options => options.AddHandler(onDebugAdapterServerStarted as IJsonRpcHandler));

            await onDebugAdapterClientStarted.Received(1).OnStarted(client, Arg.Any<CancellationToken>());
            await onDebugAdapterServerStarted.Received(1).OnStarted(server, Arg.Any<CancellationToken>());
        }
    }
}

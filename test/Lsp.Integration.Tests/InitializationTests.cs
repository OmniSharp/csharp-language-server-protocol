using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

#nullable disable

namespace Lsp.Integration.Tests
{
    public class InitializationTests : LanguageProtocolTestBase
    {
        public InitializationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Logs_should_be_allowed_during_startup()
        {
            await Initialize(ConfigureClient, ConfigureServer);

            _logs.Should().HaveCount(2);
            _logs.Should().ContainInOrder("OnInitialize", "OnInitialized");
        }

        [Fact]
        public async Task Facades_should_be_resolvable()
        {
            var (client, server) = await Initialize(ConfigureClient, ConfigureServer);

            client.Services.GetService<ILanguageClientFacade>().Should().NotBeNull();
            server.Services.GetService<ILanguageServerFacade>().Should().NotBeNull();
            // This ensures that the facade made it.
            var response = await client.RequestCodeLens(new CodeLensParams(), CancellationToken);
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_Not_Be_Able_To_Send_Messages_Unit_Initialization()
        {
            if (!( TestOptions.ClientLoggerFactory is TestLoggerFactory loggerFactory )) throw new Exception("wtf");
            var logs = new List<LogEvent>();
            var onInitializeNotify = Substitute.For<Action>();
            var onInitializedNotify = Substitute.For<Action>();
            using var _ = loggerFactory.Where(z => z.Level == LogEventLevel.Warning).Subscribe(z => logs.Add(z));

            var (client, server) = await Initialize(
                options =>
                {
                    ConfigureClient(options);
                    options
                       .OnNotification("OnInitializeNotify", onInitializeNotify)
                       .OnNotification("OnInitializedNotify", onInitializedNotify);
                }, options =>
                {
                    ConfigureServer(options);
                    options
                       .OnInitialize(
                            (languageServer, request, token) =>
                            {
                                languageServer.SendNotification("OnInitializeNotify");
                                return Task.CompletedTask;
                            }
                        )
                       .OnInitialized(
                            (languageServer, request, response, token) =>
                            {
                                languageServer.SendNotification("OnInitializedNotify");
                                return Task.CompletedTask;
                            }
                        );
                }
            );

            await SettleNext();
//
//            logs.Should().HaveCount(2);
//            logs[0].RenderMessage().Should().Contain("OnInitializeNotify");
//            logs[1].RenderMessage().Should().Contain("OnInitializedNotify");

            await SettleNext();

            onInitializeNotify.Received(1).Invoke();
            onInitializedNotify.Received(1).Invoke();
        }

        [Fact]
        public async Task Should_Be_Able_To_Register_Before_Initialize()
        {
            var (client, server) = Create(options => options.EnableDynamicRegistration().EnableAllCapabilities(), options => { });

            server.Register(
                r => { r.AddHandler<CodeLensHandlerA>(); }
            );

            await Observable.FromAsync(client.Initialize)
                            .ForkJoin(Observable.FromAsync(server.Initialize), (a, b) => ( client, server ))
                            .ToTask(CancellationToken);

            client.Services.GetService<ILanguageClientFacade>().Should().NotBeNull();
            server.Services.GetService<ILanguageServerFacade>().Should().NotBeNull();
            // This ensures that the facade made it.
            var response = await client.RequestCodeLens(new CodeLensParams(), CancellationToken);
            response.Should().NotBeNull();
        }

        private class CodeLensHandlerA : CodeLensHandlerBase
        {
            public CodeLensHandlerA(ILanguageServerFacade languageServerFacade)
            {
                languageServerFacade.Should().NotBeNull();
            }

            public override Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new CodeLensContainer());
            }

            public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken)
            {
                return Task.FromResult(request);
            }

            protected internal override CodeLensRegistrationOptions CreateRegistrationOptions(
                CodeLensCapability capability, ClientCapabilities clientCapabilities
            )
            {
                return new();
            }
        }

        private readonly List<string> _logs = new List<string>();

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.OnLogMessage(log => { _logs.Add(log.Message); });
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnInitialize(
                (server, request, token) =>
                {
                    server.Window.LogInfo("OnInitialize");
                    return Task.CompletedTask;
                }
            );
            options.AddHandler<CodeLensHandlerA>();
            options.OnInitialized(
                (server, request, response, token) =>
                {
                    server.Window.LogInfo("OnInitialized");
                    return Task.CompletedTask;
                }
            );
        }
    }
}

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
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
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
        public async Task Should_Be_Able_To_Register_Before_Initialize()
        {
            var (client, server) = Create(options => options.EnableDynamicRegistration().EnableAllCapabilities(), options => {});

            server.Register(
                r => {
                    r.AddHandler<CodeLensHandlerA>();
                }
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

        class CodeLensHandlerA : CodeLensHandler
        {
            public CodeLensHandlerA(ILanguageServerFacade languageServerFacade) : base(new CodeLensRegistrationOptions())
            {
                languageServerFacade.Should().NotBeNull();
            }

            public override Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken) => Task.FromResult(new CodeLensContainer());

            public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken) => Task.FromResult(request);
        }
        private readonly List<string> _logs = new List<string>();

        private void ConfigureClient(LanguageClientOptions options) =>
            options.OnLogMessage(log => { _logs.Add(log.Message); });

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.OnInitialize(
                (server, request, token) => {
                    server.Window.LogInfo("OnInitialize");
                    return Task.CompletedTask;
                }
            );
            options.AddHandler<CodeLensHandlerA>();
            options.OnInitialized(
                (server, request, response, token) => {
                    server.Window.LogInfo("OnInitialized");
                    return Task.CompletedTask;
                }
            );
        }
    }
}

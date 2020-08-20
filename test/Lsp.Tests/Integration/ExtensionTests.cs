using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using Lsp.Tests.Integration.Fixtures;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace Lsp.Tests.Integration
{
    public class ExtensionTests : LanguageProtocolTestBase
    {
        public ExtensionTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Support_Custom_Capabilities()
        {
            var onDiscoverHandler = Substitute.For<Func<DiscoverUnitTestsParams, UnitTestCapability, CancellationToken, Task<Container<UnitTest>>>>();
            var onRunUnitHandler = Substitute.For<Func<UnitTest, UnitTestCapability, CancellationToken, Task>>();
            var (client, server) = await Initialize(
                options =>
                    options.WithCapability(
                        new UnitTestCapability() {
                            DynamicRegistration = true,
                            Property = "Abcd"
                        }
                    ), options => {
                    options.OnDiscoverUnitTests(onDiscoverHandler, new UnitTestRegistrationOptions());
                    options.OnRunUnitTest(onRunUnitHandler, new UnitTestRegistrationOptions());
                }
            );

            {
                var capability = client.ClientSettings.Capabilities.Workspace.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                var capability = server.ClientSettings.Capabilities.Workspace.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                var capability = server.GetRequiredService<ICapabilitiesProvider>().GetCapability<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                client.RegistrationManager.CurrentRegistrations.Should().Contain(z => z.Method == "tests/discover");
                client.RegistrationManager.CurrentRegistrations.Should().Contain(z => z.Method == "tests/run");
            }

            var unitTests = await client.RequestDiscoverUnitTests(new DiscoverUnitTestsParams(), CancellationToken);
            await client.RunUnitTest(new UnitTest(), CancellationToken);

            onDiscoverHandler.Received(1).Invoke(Arg.Any<DiscoverUnitTestsParams>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
            onRunUnitHandler.Received(1).Invoke(Arg.Any<UnitTest>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Support_Custom_Capabilities_Using_Json()
        {
            var onDiscoverHandler = Substitute.For<Func<DiscoverUnitTestsParams, UnitTestCapability, CancellationToken, Task<Container<UnitTest>>>>();
            var onRunUnitHandler = Substitute.For<Func<UnitTest, UnitTestCapability, CancellationToken, Task>>();
            var (client, server) = await Initialize(
                options => { options.ClientCapabilities.Workspace.ExtensionData["unitTests"] = JToken.FromObject(new { property = "Abcd", dynamicRegistration = true }); },
                options => {
                    options.OnDiscoverUnitTests(onDiscoverHandler, new UnitTestRegistrationOptions());
                    options.OnRunUnitTest(onRunUnitHandler, new UnitTestRegistrationOptions());
                }
            );

            {
                var capability = server.ClientSettings.Capabilities.Workspace.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                var capability = server.GetRequiredService<ICapabilitiesProvider>().GetCapability<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                client.RegistrationManager.CurrentRegistrations.Should().Contain(z => z.Method == "tests/discover");
                client.RegistrationManager.CurrentRegistrations.Should().Contain(z => z.Method == "tests/run");
            }

            var unitTests = await client.RequestDiscoverUnitTests(new DiscoverUnitTestsParams(), CancellationToken);
            await client.RunUnitTest(new UnitTest(), CancellationToken);

            onDiscoverHandler.Received(1).Invoke(Arg.Any<DiscoverUnitTestsParams>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
            onRunUnitHandler.Received(1).Invoke(Arg.Any<UnitTest>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
            options.WithCapability(
                new UnitTestCapability() {
                    DynamicRegistration = true,
                    Property = "Abcd"
                }
            );
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
        }
    }
}

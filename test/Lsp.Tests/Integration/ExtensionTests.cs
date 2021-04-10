using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using Lsp.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using TestingUtils;

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
            onDiscoverHandler
               .Invoke(Arg.Any<DiscoverUnitTestsParams>(), Arg.Any<UnitTestCapability>(), Arg.Any<CancellationToken>())
               .Returns(new Container<UnitTest>());
            var onRunUnitHandler = Substitute.For<Func<UnitTest, UnitTestCapability, CancellationToken, Task>>();
            onRunUnitHandler
               .Invoke(Arg.Any<UnitTest>(), Arg.Any<UnitTestCapability>(), Arg.Any<CancellationToken>())
               .Returns(Task.CompletedTask);
            var (client, server) = await Initialize(
                options => {
                    options.UseAssemblyAttributeScanning = false;
                    options
                       .WithAssemblies(typeof(UnitTestCapability).Assembly)
                       .WithCapability(
                            new UnitTestCapability() {
                                DynamicRegistration = true,
                                Property = "Abcd"
                            }
                        );
                }, options => {
                    options.UseAssemblyAttributeScanning = false;
                    options
                       .WithAssemblies(typeof(UnitTestCapability).Assembly)
                       .OnDiscoverUnitTests(onDiscoverHandler, (_, _) => new UnitTestRegistrationOptions())
                       .OnRunUnitTest(
                            onRunUnitHandler, (_, _) => new UnitTestRegistrationOptions() {
                                SupportsDebugging = true,
                                WorkDoneProgress = true
                            }
                        );
                }
            );

            {
                var capability = client.ClientSettings.Capabilities!.Workspace!.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                var capability = server.ClientSettings.Capabilities!.Workspace!.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                var capability = server.GetRequiredService<ICapabilitiesProvider>().GetCapability<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                await TestHelper.DelayUntil(() => client.RegistrationManager.CurrentRegistrations.Any(z => z.Method == "tests"), CancellationToken);
                client.RegistrationManager.CurrentRegistrations.Should().Contain(z => z.Method == "tests").And.HaveCount(1);
            }

            await client.RequestDiscoverUnitTests(
                new DiscoverUnitTestsParams() {
                    PartialResultToken = new ProgressToken(1),
                    WorkDoneToken = new ProgressToken(1),
                }, CancellationToken
            );
            await client.RunUnitTest(new UnitTest(), CancellationToken);

            onDiscoverHandler.Received(1).Invoke(Arg.Any<DiscoverUnitTestsParams>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
            onRunUnitHandler.Received(1).Invoke(Arg.Any<UnitTest>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Support_Custom_Capabilities_Using_Json()
        {
            var onDiscoverHandler = Substitute.For<Func<DiscoverUnitTestsParams, UnitTestCapability, CancellationToken, Task<Container<UnitTest>>>>();
            onDiscoverHandler
               .Invoke(Arg.Any<DiscoverUnitTestsParams>(), Arg.Any<UnitTestCapability>(), Arg.Any<CancellationToken>())
               .Returns(new Container<UnitTest>());
            var onRunUnitHandler = Substitute.For<Func<UnitTest, UnitTestCapability, CancellationToken, Task>>();
            onRunUnitHandler
               .Invoke(Arg.Any<UnitTest>(), Arg.Any<UnitTestCapability>(), Arg.Any<CancellationToken>())
               .Returns(Task.CompletedTask);
            var (client, server) = await Initialize(
                options => {
                    options.UseAssemblyAttributeScanning = false;
                    options.ClientCapabilities.Workspace!.ExtensionData["unitTests"] = JToken.FromObject(new { property = "Abcd", dynamicRegistration = true }); },
                options => {

                    options.UseAssemblyAttributeScanning = false;
                    options.OnDiscoverUnitTests(onDiscoverHandler, (_, _) => new UnitTestRegistrationOptions());
                    options.OnRunUnitTest(onRunUnitHandler, (_, _) => new UnitTestRegistrationOptions());
                }
            );

            {
                var capability = server.ClientSettings.Capabilities!.Workspace!.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                var capability = server.GetRequiredService<ICapabilitiesProvider>().GetCapability<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                await client.RegistrationManager.Registrations.Throttle(TimeSpan.FromMilliseconds(300)).Take(1).ToTask(CancellationToken);
                client.RegistrationManager.CurrentRegistrations.Should().Contain(z => z.Method == "tests").And.HaveCount(1);
            }

            await client.RequestDiscoverUnitTests(new DiscoverUnitTestsParams(), CancellationToken);
            await client.RunUnitTest(new UnitTest(), CancellationToken);

            onDiscoverHandler.Received(1).Invoke(Arg.Any<DiscoverUnitTestsParams>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
            onRunUnitHandler.Received(1).Invoke(Arg.Any<UnitTest>(), Arg.Is<UnitTestCapability>(x => x.Property == "Abcd"), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Should_Support_Custom_Static_Options()
        {
            var onDiscoverHandler = Substitute.For<Func<DiscoverUnitTestsParams, UnitTestCapability, CancellationToken, Task<Container<UnitTest>>>>();
            var onRunUnitHandler = Substitute.For<Func<UnitTest, UnitTestCapability, CancellationToken, Task>>();
            var (_, server) = await Initialize(
                options => {
                    options.UseAssemblyAttributeScanning = false;
                    options.WithCapability(
                        new UnitTestCapability() {
                            DynamicRegistration = false,
                            Property = "Abcd"
                        }
                    );
                }, options => {
                    options.UseAssemblyAttributeScanning = false;
                    options.OnDiscoverUnitTests(onDiscoverHandler, (_, _) => new UnitTestRegistrationOptions() { SupportsDebugging = true });
                    options.OnRunUnitTest(onRunUnitHandler, (_, _) => new UnitTestRegistrationOptions() { SupportsDebugging = true });
                }
            );

            {
                var capability = server.ClientSettings.Capabilities!.Workspace!.ExtensionData["unitTests"].ToObject<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }

            {
                server.ServerSettings.Capabilities.ExtensionData["unitTestDiscovery"].Should().NotBeNull();
                server.ServerSettings.Capabilities.ExtensionData["unitTestDiscovery"]
                      .ToObject<UnitTestRegistrationOptions.StaticOptions>().SupportsDebugging.Should().BeTrue();
            }

            {
                var capability = server.GetRequiredService<ICapabilitiesProvider>().GetCapability<UnitTestCapability>();
                capability.Property.Should().Be("Abcd");
            }
        }

        [Fact]
        public async Task Should_Convert_Registration_Options_Into_Static_Options_As_Required()
        {
            var (client, _) = await Initialize(
                options => {
                    options.UseAssemblyAttributeScanning = false;
                    options.DisableDynamicRegistration();
                    options.WithCapability(
                        new CodeActionCapability() {
                            DynamicRegistration = false,
                            CodeActionLiteralSupport = new CodeActionLiteralSupportOptions() {
                                CodeActionKind = new CodeActionKindCapabilityOptions() {
                                    ValueSet = new Container<CodeActionKind>(
                                        CodeActionKind.Empty,
                                        CodeActionKind.Refactor,
                                        CodeActionKind.Source,
                                        CodeActionKind.QuickFix,
                                        CodeActionKind.RefactorExtract,
                                        CodeActionKind.RefactorInline,
                                        CodeActionKind.RefactorRewrite,
                                        CodeActionKind.SourceOrganizeImports
                                    )
                                }
                            }
                        }
                    );
                },
                options => {
                    options.UseAssemblyAttributeScanning = false;
                    options.OnCodeAction(
                        (@params, capability, token) => Task.FromResult(new CommandOrCodeActionContainer()),
                        (_, _) => new CodeActionRegistrationOptions() {
                            CodeActionKinds = new Container<CodeActionKind>(
                                CodeActionKind.RefactorExtract,
                                CodeActionKind.RefactorInline,
                                CodeActionKind.RefactorRewrite,
                                CodeActionKind.SourceOrganizeImports
                            )
                        }
                    );
                }
            );

            client.ServerSettings.Capabilities.CodeActionProvider.Should().NotBeNull();
            client.ServerSettings.Capabilities.CodeActionProvider!.IsValue.Should().Be(true);
            client.ServerSettings.Capabilities.CodeActionProvider.Value.CodeActionKinds.Should().ContainInOrder(
                CodeActionKind.RefactorExtract,
                CodeActionKind.RefactorInline,
                CodeActionKind.RefactorRewrite,
                CodeActionKind.SourceOrganizeImports
            );
        }
    }
}

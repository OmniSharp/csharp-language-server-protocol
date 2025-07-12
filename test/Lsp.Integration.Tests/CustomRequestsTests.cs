using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using TestingUtils;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public class CustomRequestsTests : LanguageProtocolTestBase
    {
        public CustomRequestsTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Should_Support_Custom_Telemetry_Using_Base_Class()
        {
            var fake = Substitute.For<TelemetryEventHandlerBase<CustomTelemetryEventParams>>();
            var (_, server) = await Initialize(options => { options.AddHandler(fake); }, options => { });

            var @event = new CustomTelemetryEventParams
            {
                CodeFolding = true,
                ProfileLoading = false,
                ScriptAnalysis = true,
                Pester5CodeLens = true,
                PromptToUpdatePackageManagement = false
            };
            server.SendTelemetryEvent(@event);
            await TestHelper.DelayUntil(() => fake.ReceivedCalls().Any(), CancellationToken);

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();

            args[0].Should().BeOfType<CustomTelemetryEventParams>()
                   .And.Subject
                   .Should().BeEquivalentTo(@event, z => z.UsingStructuralRecordEquality().Excluding(x => x.ExtensionData));
        }

        [Fact]
        public async Task Should_Support_Custom_Telemetry_Receiving_Regular_Telemetry_Using_Base_Class()
        {
            var fake = Substitute.For<TelemetryEventHandlerBase>();
            var (_, server) = await Initialize(options => { options.AddHandler(fake); }, options => { });

            var @event = new CustomTelemetryEventParams
            {
                CodeFolding = true,
                ProfileLoading = false,
                ScriptAnalysis = true,
                Pester5CodeLens = true,
                PromptToUpdatePackageManagement = false
            };
            server.SendTelemetryEvent(@event);
            await TestHelper.DelayUntil(() => fake.ReceivedCalls().Any(), CancellationToken);

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<TelemetryEventParams>().Which;
            request.ExtensionData.Should().ContainKey("codeFolding").And.Subject["codeFolding"].Should().Be(true);
            request.ExtensionData.Should().ContainKey("profileLoading").And.Subject["profileLoading"].Should().Be(false);
            request.ExtensionData.Should().ContainKey("scriptAnalysis").And.Subject["scriptAnalysis"].Should().Be(true);
            request.ExtensionData.Should().ContainKey("pester5CodeLens").And.Subject["pester5CodeLens"].Should().Be(true);
            request.ExtensionData.Should().ContainKey("promptToUpdatePackageManagement").And.Subject["promptToUpdatePackageManagement"].Should().Be(false);
        }

        [Fact]
        public async Task Should_Support_Custom_Telemetry_Using_Extension_Data_Using_Base_Class()
        {
            var fake = Substitute.For<TelemetryEventHandlerBase<CustomTelemetryEventParams>>();
            var (_, server) = await Initialize(options => { options.AddHandler(fake); }, options => { });

            server.SendTelemetryEvent(
                new TelemetryEventParams
                {
                    ExtensionData = new Dictionary<string, object>
                    {
                        ["CodeFolding"] = true,
                        ["ProfileLoading"] = false,
                        ["ScriptAnalysis"] = true,
                        ["Pester5CodeLens"] = true,
                        ["PromptToUpdatePackageManagement"] = false
                    }
                }
            );
            await TestHelper.DelayUntil(() => fake.ReceivedCalls().Any(), CancellationToken);

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomTelemetryEventParams>().Which;
            request.CodeFolding.Should().Be(true);
            request.ProfileLoading.Should().Be(false);
            request.ScriptAnalysis.Should().Be(true);
            request.Pester5CodeLens.Should().Be(true);
            request.PromptToUpdatePackageManagement.Should().Be(false);
        }

        [Fact]
        public async Task Should_Support_Custom_Telemetry_Using_Delegate()
        {
            var fake = Substitute.For<Func<CustomTelemetryEventParams, CancellationToken, Task>>();
            var (_, server) = await Initialize(options => { options.OnTelemetryEvent(fake); }, options => { });

            var @event = new CustomTelemetryEventParams
            {
                CodeFolding = true,
                ProfileLoading = false,
                ScriptAnalysis = true,
                Pester5CodeLens = true,
                PromptToUpdatePackageManagement = false
            };
            server.SendTelemetryEvent(@event);
            await TestHelper.DelayUntil(() => fake.ReceivedCalls().Any(), CancellationToken);

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            args[0]
               .Should().BeOfType<CustomTelemetryEventParams>()
               .And.Subject
               .Should().BeEquivalentTo(@event, z => z.UsingStructuralRecordEquality().Excluding(x => x.ExtensionData));
        }

        [Fact]
        public async Task Should_Support_Custom_Telemetry_Receiving_Regular_Telemetry_Using_Delegate()
        {
            var fake = Substitute.For<Func<TelemetryEventParams, CancellationToken, Task>>();
            var (_, server) = await Initialize(options => { options.OnTelemetryEvent(fake); }, options => { });

            var @event = new CustomTelemetryEventParams
            {
                CodeFolding = true,
                ProfileLoading = false,
                ScriptAnalysis = true,
                Pester5CodeLens = true,
                PromptToUpdatePackageManagement = false
            };
            server.SendTelemetryEvent(@event);
            await TestHelper.DelayUntil(() => fake.ReceivedCalls().Any(), CancellationToken);

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<TelemetryEventParams>().Which;
            request.ExtensionData.Should().ContainKey("codeFolding").And.Subject["codeFolding"].Should().Be(true);
            request.ExtensionData.Should().ContainKey("profileLoading").And.Subject["profileLoading"].Should().Be(false);
            request.ExtensionData.Should().ContainKey("scriptAnalysis").And.Subject["scriptAnalysis"].Should().Be(true);
            request.ExtensionData.Should().ContainKey("pester5CodeLens").And.Subject["pester5CodeLens"].Should().Be(true);
            request.ExtensionData.Should().ContainKey("promptToUpdatePackageManagement").And.Subject["promptToUpdatePackageManagement"].Should().Be(false);
        }

        [Fact]
        public async Task Should_Support_Custom_Telemetry_Using_Extension_Data_Using_Delegate()
        {
            var fake = Substitute.For<Func<CustomTelemetryEventParams, CancellationToken, Task>>();
            var (_, server) = await Initialize(options => { options.OnTelemetryEvent(fake); }, options => { });

            server.SendTelemetryEvent(
                new TelemetryEventParams
                {
                    ExtensionData = new Dictionary<string, object>
                    {
                        ["CodeFolding"] = true,
                        ["ProfileLoading"] = false,
                        ["ScriptAnalysis"] = true,
                        ["Pester5CodeLens"] = true,
                        ["PromptToUpdatePackageManagement"] = false
                    }
                }
            );
            await TestHelper.DelayUntil(() => fake.ReceivedCalls().Any(), CancellationToken);

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomTelemetryEventParams>().Which;
            request.CodeFolding.Should().Be(true);
            request.ProfileLoading.Should().Be(false);
            request.ScriptAnalysis.Should().Be(true);
            request.Pester5CodeLens.Should().Be(true);
            request.PromptToUpdatePackageManagement.Should().Be(false);
        }

        public record CustomTelemetryEventParams : TelemetryEventParams
        {
            public bool ScriptAnalysis { get; init; }
            public bool CodeFolding { get; init; }
            public bool PromptToUpdatePackageManagement { get; init; }
            public bool ProfileLoading { get; init; }
            public bool Pester5CodeLens { get; init; }
        }
    }
}

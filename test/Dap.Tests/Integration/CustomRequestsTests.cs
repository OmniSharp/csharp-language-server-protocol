using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class CustomRequestsTests : DebugAdapterProtocolTestBase
    {
        public CustomRequestsTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Fact]
        public async Task Should_Support_Custom_Attach_Request_Using_Base_Class()
        {
            var fake = Substitute.For<AttachHandlerBase<CustomAttachRequestArguments>>();
            var (client, _) = await Initialize(options => { }, options => { options.AddHandler(fake); });

            await client.Attach(
                new CustomAttachRequestArguments {
                    ComputerName = "computer",
                    RunspaceId = "1234",
                    ProcessId = "4321"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomAttachRequestArguments>().Which;
            request.ComputerName.Should().Be("computer");
            request.RunspaceId.Should().Be("1234");
            request.ProcessId.Should().Be("4321");
        }

        [Fact]
        public async Task Should_Support_Custom_Attach_Request_Receiving_Regular_Request_Using_Base_Class()
        {
            var fake = Substitute.For<AttachHandlerBase>();
            var (client, _) = await Initialize(options => { }, options => { options.AddHandler(fake); });

            await client.Attach(
                new CustomAttachRequestArguments {
                    ComputerName = "computer",
                    RunspaceId = "1234",
                    ProcessId = "4321"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<AttachRequestArguments>().Which;
            request.ExtensionData.Should().ContainKey("computerName").And.Subject["computerName"].Should().Be("computer");
            request.ExtensionData.Should().ContainKey("runspaceId").And.Subject["runspaceId"].Should().Be("1234");
            request.ExtensionData.Should().ContainKey("processId").And.Subject["processId"].Should().Be("4321");
        }

        [Fact]
        public async Task Should_Support_Custom_Attach_Request_Using_Extension_Data_Using_Base_Class()
        {
            var fake = Substitute.For<AttachHandlerBase<CustomAttachRequestArguments>>();
            var (client, _) = await Initialize(options => { }, options => { options.AddHandler(fake); });

            await client.Attach(
                new AttachRequestArguments {
                    ExtensionData = new Dictionary<string, object> {
                        ["ComputerName"] = "computer",
                        ["RunspaceId"] = "1234",
                        ["ProcessId"] = "4321"
                    }
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomAttachRequestArguments>().Which;
            request.ComputerName.Should().Be("computer");
            request.RunspaceId.Should().Be("1234");
            request.ProcessId.Should().Be("4321");
        }

        [Fact]
        public async Task Should_Support_Custom_Launch_Request_Using_Base_Class()
        {
            var fake = Substitute.For<LaunchHandlerBase<CustomLaunchRequestArguments>>();
            var (client, _) = await Initialize(options => { }, options => { options.AddHandler(fake); });

            await client.RequestLaunch(
                new CustomLaunchRequestArguments {
                    Script = "build.ps1"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomLaunchRequestArguments>().Which;
            request.Script.Should().Be("build.ps1");
        }

        [Fact]
        public async Task Should_Support_Custom_Launch_Request_Receiving_Regular_Request_Using_Base_Class()
        {
            var fake = Substitute.For<LaunchHandlerBase>();
            var (client, _) = await Initialize(options => { }, options => { options.AddHandler(fake); });

            await client.RequestLaunch(
                new CustomLaunchRequestArguments {
                    Script = "build.ps1"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<LaunchRequestArguments>().Which;
            request.ExtensionData.Should().ContainKey("script").And.Subject["script"].Should().Be("build.ps1");
        }

        [Fact]
        public async Task Should_Support_Custom_Launch_Request_Using_Extension_Data_Base_Class()
        {
            var fake = Substitute.For<LaunchHandlerBase<CustomLaunchRequestArguments>>();
            var (client, _) = await Initialize(options => { }, options => { options.AddHandler(fake); });

            await client.RequestLaunch(
                new CustomLaunchRequestArguments {
                    ExtensionData = new Dictionary<string, object> {
                        ["Script"] = "build.ps1"
                    }
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomLaunchRequestArguments>().Which;
            request.Script.Should().Be("build.ps1");
        }

        [Fact]
        public async Task Should_Support_Custom_Attach_Request_Using_Delegate()
        {
            var fake = Substitute.For<Func<CustomAttachRequestArguments, CancellationToken, Task<AttachResponse>>>();
            var (client, _) = await Initialize(options => { }, options => { options.OnAttach(fake); });

            await client.Attach(
                new CustomAttachRequestArguments {
                    ComputerName = "computer",
                    RunspaceId = "1234",
                    ProcessId = "4321"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomAttachRequestArguments>().Which;
            request.ComputerName.Should().Be("computer");
            request.RunspaceId.Should().Be("1234");
            request.ProcessId.Should().Be("4321");
        }

        [Fact]
        public async Task Should_Support_Custom_Attach_Request_Receiving_Regular_Request_Using_Delegate()
        {
            var fake = Substitute.For<Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>>>();
            var (client, _) = await Initialize(options => { }, options => { options.OnAttach(fake); });

            await client.Attach(
                new CustomAttachRequestArguments {
                    ComputerName = "computer",
                    RunspaceId = "1234",
                    ProcessId = "4321"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<AttachRequestArguments>().Which;
            request.ExtensionData.Should().ContainKey("computerName").And.Subject["computerName"].Should().Be("computer");
            request.ExtensionData.Should().ContainKey("runspaceId").And.Subject["runspaceId"].Should().Be("1234");
            request.ExtensionData.Should().ContainKey("processId").And.Subject["processId"].Should().Be("4321");
        }

        [Fact]
        public async Task Should_Support_Custom_Attach_Request_Using_Extension_Data_Using_Delegate()
        {
            var fake = Substitute.For<Func<CustomAttachRequestArguments, CancellationToken, Task<AttachResponse>>>();
            var (client, _) = await Initialize(options => { }, options => { options.OnAttach(fake); });

            await client.Attach(
                new AttachRequestArguments {
                    ExtensionData = new Dictionary<string, object> {
                        ["ComputerName"] = "computer",
                        ["RunspaceId"] = "1234",
                        ["ProcessId"] = "4321"
                    }
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomAttachRequestArguments>().Which;
            request.ComputerName.Should().Be("computer");
            request.RunspaceId.Should().Be("1234");
            request.ProcessId.Should().Be("4321");
        }

        [Fact]
        public async Task Should_Support_Custom_Launch_Request_Using_Delegate()
        {
            var fake = Substitute.For<Func<CustomLaunchRequestArguments, CancellationToken, Task<LaunchResponse>>>();
            var (client, _) = await Initialize(options => { }, options => { options.OnLaunch(fake); });

            await client.RequestLaunch(
                new CustomLaunchRequestArguments {
                    Script = "build.ps1"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomLaunchRequestArguments>().Which;
            request.Script.Should().Be("build.ps1");
        }

        [Fact]
        public async Task Should_Support_Custom_Launch_Request_Receiving_Regular_Request_Using_Delegate()
        {
            var fake = Substitute.For<Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse>>>();
            var (client, _) = await Initialize(options => { }, options => { options.OnLaunch(fake); });

            await client.RequestLaunch(
                new CustomLaunchRequestArguments {
                    Script = "build.ps1"
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<LaunchRequestArguments>().Which;
            request.ExtensionData.Should().ContainKey("script").And.Subject["script"].Should().Be("build.ps1");
        }

        [Fact]
        public async Task Should_Support_Custom_Launch_Request_Using_Extension_Data_Using_Delegate()
        {
            var fake = Substitute.For<Func<CustomLaunchRequestArguments, CancellationToken, Task<LaunchResponse>>>();
            var (client, _) = await Initialize(options => { }, options => { options.OnLaunch(fake); });

            await client.RequestLaunch(
                new CustomLaunchRequestArguments {
                    ExtensionData = new Dictionary<string, object> {
                        ["Script"] = "build.ps1"
                    }
                }
            );

            var call = fake.ReceivedCalls().Single();
            var args = call.GetArguments();
            var request = args[0].Should().BeOfType<CustomLaunchRequestArguments>().Which;
            request.Script.Should().Be("build.ps1");
        }

        public record CustomAttachRequestArguments : AttachRequestArguments
        {
            public string ComputerName { get; init; }

            public string ProcessId { get; init; }

            public string RunspaceId { get; init; }
        }

        public record CustomLaunchRequestArguments : LaunchRequestArguments
        {
            /// <summary>
            /// Gets or sets the absolute path to the script to debug.
            /// </summary>
            public string Script { get; init; }
        }
    }
}

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Testing;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests.Integration
{
    public class HandlersManagerIntegrationTests : DebugAdapterProtocolTestBase
    {
        public HandlersManagerIntegrationTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [Fact]
        public async Task Should_Return_Default_Handlers()
        {
            var (client, server) = await Initialize(options => {}, options => {});

            var handlersManager = server.GetRequiredService<IHandlersManager>();
            handlersManager.Descriptors.Should().HaveCount(2);
            handlersManager.GetHandlers().Should().HaveCount(2);
        }

        [Fact]
        public async Task Link_Should_Fail_If_No_Handler_Is_Defined()
        {
            var (client, server) = await Initialize(options => {}, options => {});

            var handlersManager = server.GetRequiredService<IHandlersManager>();

            Action a  = () => handlersManager.AddLink(RequestNames.Completions, "my/completions");
            a.Should().Throw<ArgumentException>().Which.Message.Should().Contain("Descriptors must be registered before links can be created");
        }

        [Fact]
        public async Task Link_Should_Fail_If_Link_Is_On_The_Wrong_Side()
        {
            var (client, server) = await Initialize(options => {}, options => {});

            var handlersManager = server.GetRequiredService<IHandlersManager>();
            handlersManager.Add(Substitute.For(new Type[] { typeof(ICompletionsHandler) }, Array.Empty<object>()) as IJsonRpcHandler, new JsonRpcHandlerOptions());

            Action a  = () => handlersManager.AddLink("my/completions", RequestNames.Completions);
            a.Should().Throw<ArgumentException>().Which.Message.Should().Contain($"Did you mean to link '{RequestNames.Completions}' to 'my/completions' instead");
        }
    }
}

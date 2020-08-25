using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class HandlersManagerIntegrationTests : LanguageProtocolTestBase
    {
        public HandlersManagerIntegrationTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [Fact]
        public async Task Should_Return_Default_Handlers()
        {
            var (client, server) = await Initialize(options => {}, options => {});

            var handlersManager = server.GetRequiredService<IHandlersManager>();
            handlersManager.Descriptors.Should().HaveCount(8);
            handlersManager.GetHandlers().Should().HaveCount(5);
        }

        [Fact]
        public async Task Should_Return_Additional_Handlers()
        {
            var (client, server) = await Initialize(options => {}, options => {});

            server.Register(o => o.AddHandler(Substitute.For(new Type[] { typeof (ICompletionHandler), typeof(ICompletionResolveHandler) }, Array.Empty<object>()) as IJsonRpcHandler));
            var handlersManager = server.GetRequiredService<IHandlersManager>();
            handlersManager.Descriptors.Should().HaveCount(10);
            handlersManager.GetHandlers().Should().HaveCount(6);
        }
    }
}

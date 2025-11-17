using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public class HandlersManagerIntegrationTests : LanguageProtocolTestBase
    {
        public HandlersManagerIntegrationTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [Fact]
        public async Task Should_Return_Default_Handlers()
        {
            var (_, server) = await Initialize(options => { }, options => { });

            var handlersManager = server.GetRequiredService<IHandlersManager>();
            handlersManager.Descriptors.Should().HaveCount(9);
            handlersManager.GetHandlers().Should().HaveCount(6);
        }

        [Fact]
        public async Task Should_Return_Additional_Handlers()
        {
            var (_, server) = await Initialize(options => { }, options => { });

            server.Register(
                o => o.AddHandler(
                    (IJsonRpcHandler)Substitute.For(new[] { typeof(ICompletionHandler), typeof(ICompletionResolveHandler) }, Array.Empty<object>())
                )
            );
            var handlersManager = server.GetRequiredService<IHandlersManager>();
            handlersManager.Descriptors.Should().HaveCount(11);
            handlersManager.GetHandlers().Should().HaveCount(7);
        }

        [Fact]
        public async Task Link_Should_Fail_If_No_Handler_Is_Defined()
        {
            var (_, server) = await Initialize(options => { }, options => { });

            var handlersManager = server.GetRequiredService<IHandlersManager>();

            Action a = () => handlersManager.AddLink(TextDocumentNames.Completion, "my/completion");
            a.Should().Throw<ArgumentException>().Which.Message.Should().Contain("Descriptors must be registered before links can be created");
        }

        [Fact]
        public async Task Link_Should_Fail_If_Link_Is_On_The_Wrong_Side()
        {
            var (_, server) = await Initialize(options => { }, options => { });

            server.Register(
                o => o.AddHandler(
                    (IJsonRpcHandler)Substitute.For(new[] { typeof(ICompletionHandler), typeof(ICompletionResolveHandler) }, Array.Empty<object>())
                )
            );
            var handlersManager = server.GetRequiredService<IHandlersManager>();

            Action a = () => handlersManager.AddLink("my/completion", TextDocumentNames.Completion);
            a.Should().Throw<ArgumentException>().Which.Message.Should()
             .Contain($"Did you mean to link '{TextDocumentNames.Completion}' to 'my/completion' instead");
        }
    }
}

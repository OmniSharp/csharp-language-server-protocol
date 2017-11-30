using System;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class RequestProcessIdentifierTests
    {
        private readonly TestLoggerFactory _testLoggerFactory;

        public RequestProcessIdentifierTests(ITestOutputHelper testOutputHelper)
        {
            _testLoggerFactory = new TestLoggerFactory(testOutputHelper);
        }

        [Fact]
        public void ShouldIdentifyAs_Default()
        {
            var identifier = new RequestProcessIdentifier(RequestProcessType.Parallel);
            var handler = Substitute.For<IHandlerDescriptor>();
            handler.Handler.Returns(Substitute.For<IJsonRpcHandler>());
            handler.HandlerType.Returns(typeof(IJsonRpcHandler));

            identifier.Identify(handler).Should().Be(RequestProcessType.Parallel);
        }

        [Theory]
        [InlineData(typeof(ICodeActionHandler))]
        [InlineData(typeof(ICodeLensHandler))]
        [InlineData(typeof(ICodeLensResolveHandler))]
        [InlineData(typeof(IDefinitionHandler))]
        [InlineData(typeof(IDidCloseTextDocumentHandler))]
        [InlineData(typeof(IDocumentHighlightHandler))]
        [InlineData(typeof(IDocumentLinkHandler))]
        [InlineData(typeof(IDocumentLinkResolveHandler))]
        [InlineData(typeof(IDocumentSymbolHandler))]
        [InlineData(typeof(IWorkspaceSymbolsHandler))]
        [InlineData(typeof(IWillSaveTextDocumentHandler))]
        [InlineData(typeof(IHoverHandler))]
        [InlineData(typeof(IReferencesHandler))]
        [InlineData(typeof(ISignatureHelpHandler))]
        [InlineData(typeof(ICancelRequestHandler))]
        public void ShouldIdentifyAs_Parallel(Type type)
        {
            var identifier = new RequestProcessIdentifier(RequestProcessType.Serial);
            var handler = Substitute.For<IHandlerDescriptor>();
            handler.Handler.Returns((IJsonRpcHandler)Substitute.For(new Type[] { type }, new object[0]));
            handler.HandlerType.Returns(type);

            identifier.Identify(handler).Should().Be(RequestProcessType.Parallel);
        }

        [Theory]
        [InlineData(typeof(IDidChangeTextDocumentHandler))]
        [InlineData(typeof(IDidOpenTextDocumentHandler))]
        [InlineData(typeof(IDidSaveTextDocumentHandler))]
        [InlineData(typeof(IDocumentFormattingHandler))]
        [InlineData(typeof(IDocumentOnTypeFormatHandler))]
        [InlineData(typeof(IDocumentRangeFormattingHandler))]
        [InlineData(typeof(IWillSaveWaitUntilTextDocumentHandler))]
        [InlineData(typeof(IExitHandler))]
        [InlineData(typeof(IShutdownHandler))]
        [InlineData(typeof(IInitializeHandler))]
        [InlineData(typeof(IInitializedHandler))]
        [InlineData(typeof(IDidChangeConfigurationHandler))]
        [InlineData(typeof(IDidChangeWatchedFilesHandler))]
        [InlineData(typeof(IExecuteCommandHandler))]
        public void ShouldIdentifyAs_Serial(Type type)
        {
            var identifier = new RequestProcessIdentifier(RequestProcessType.Parallel);
            var handler = Substitute.For<IHandlerDescriptor>();
            handler.Handler.Returns((IJsonRpcHandler)Substitute.For(new Type[] { type }, new object[0]));
            handler.HandlerType.Returns(type);

            identifier.Identify(handler).Should().Be(RequestProcessType.Serial);
        }
    }
}

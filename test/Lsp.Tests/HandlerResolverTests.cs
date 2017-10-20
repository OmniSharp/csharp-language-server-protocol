using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Xunit;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.HandlerCollection;
using OmniSharp.Extensions.LanguageServer.Models;
using OmniSharp.Extensions.LanguageServer.Abstractions;

namespace Lsp.Tests
{
    public class HandlerResolverTests
    {
        [Theory]
        [InlineData(typeof(IInitializeHandler), "initialize", 1)]
        [InlineData(typeof(IInitializedHandler), "initialized", 1)]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]);
            if (sub is IRegistration<TextDocumentRegistrationOptions> reg)
                reg.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions());

            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData("textDocument/didOpen", 4)]
        [InlineData("textDocument/didChange", 4)]
        [InlineData("textDocument/didClose", 4)]
        [InlineData("textDocument/didSave", 4)]
        public void Should_Contain_AllDefinedTextDocumentSyncMethods(string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.something"));

            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData("exit", 4)]
        [InlineData("shutdown", 4)]
        [InlineData("initialized", 4)]
        [InlineData("initialize", 4)]
        public void Should_Contain_AllDefinedLanguageServerMethods(string key, int count)
        {
            var handler = new HandlerCollection();
            handler.Add(
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>()
            );
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData("exit", 4)]
        [InlineData("shutdown", 4)]
        [InlineData("initialized", 4)]
        [InlineData("initialize", 4)]
        public void Should_Contain_AllDefinedLanguageServerMethods_GivenDuplicates(string key, int count)
        {
            var handler = new HandlerCollection();
            handler.Add(
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>(),
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>(),
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IExitHandler>(),
                Substitute.For<IShutdownHandler>()
            );
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData("textDocument/didOpen", 8)]
        [InlineData("textDocument/didChange", 8)]
        [InlineData("textDocument/didClose", 8)]
        [InlineData("textDocument/didSave", 8)]
        public void Should_Contain_AllDefinedMethods_ForDifferentKeys(string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var sub2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"));

            handler.Add(sub);
            handler.Add(sub2);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 2)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub is IRegistration<TextDocumentRegistrationOptions> reg)
                reg.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector()
                    });
            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 2)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer_WithDifferentKeys(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub is IRegistration<TextDocumentRegistrationOptions> reg)
                reg.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector()
                    });
            var sub2 = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub2 is IRegistration<TextDocumentRegistrationOptions> reg2)
                reg2.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector()
                    });
            handler.Add(sub);
            handler.Add(sub2);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }

        [Fact]
        public void Should_BeAwesome()
        {
            var handler = new HandlerCollection();

            handler.Add(
                Substitute.For<IExitHandler>(),
                Substitute.For<IInitializeHandler>(),
                Substitute.For<IInitializedHandler>(),
                Substitute.For<IShutdownHandler>()
            );

            handler._handlers.Should().Contain(x => x.Method == "exit");
            handler._handlers.Should().Contain(x => x.Method == "shutdown");
            handler._handlers.Count.Should().Be(4);
        }
    }
}

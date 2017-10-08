using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Xunit;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.HandlerCollection;

namespace Lsp.Tests
{
    public class HandlerResolverTests
    {

        [Theory]
        [InlineData(typeof(IInitializeHandler), "initialize", 1)]
        [InlineData(typeof(IInitializedHandler), "initialized", 1)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didOpen", 4)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didChange", 4)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didClose", 4)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didSave", 4)]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]);
            sub.Key.Returns("abcd");
            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), "initialize", 2)]
        [InlineData(typeof(IInitializedHandler), "initialized", 2)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didOpen", 8)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didChange", 8)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didClose", 8)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didSave", 8)]
        public void Should_Contain_AllDefinedMethods_ForDifferentKeys(Type requestHandler, string key, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]);
            sub.Key.Returns("abcd");
            var sub2 = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]);
            sub2.Key.Returns("efgh");
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
            sub.Key.Returns("abd3");
            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 4)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer_WithDifferentKeys(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new HandlerCollection();
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            sub.Key.Returns("abd3");
            var sub2 = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            sub2.Key.Returns("efgh");
            handler.Add(sub);
            handler.Add(sub2);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }
    }
}

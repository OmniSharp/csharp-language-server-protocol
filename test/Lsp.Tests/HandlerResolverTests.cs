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
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didOpen", 5)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didChange", 5)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didClose", 5)]
        [InlineData(typeof(ITextDocumentSyncHandler), "textDocument/didSave", 5)]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key, int count)
        {
            var handler = new HandlerCollection();
            handler.Add((IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]));
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 2)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new HandlerCollection();
            handler.Add((IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]));
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Should().Contain(x => x.Method == key2);
            handler._handlers.Count.Should().Be(count);
        }
    }
}

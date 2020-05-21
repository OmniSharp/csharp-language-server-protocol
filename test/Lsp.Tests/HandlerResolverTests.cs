using System;
using System.Linq;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Xunit;
using System.Collections.Generic;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace Lsp.Tests
{
    public class HandlerResolverTests
    {
        [Theory]
        [InlineData(typeof(IInitializeHandler), "initialize", 1)]
        [InlineData(typeof(IInitializedHandler), "initialized", 1)]
        [InlineData(typeof(IShutdownHandler), "shutdown", 1)]
        [InlineData(typeof(IExitHandler), "exit", 1)]
        public void Should_Contain_AllDefinedMethods(Type requestHandler, string key, int count)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler }, new object[0]);

            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Fact]
        public void Should_Contain_AllConcreteDefinedMethods()
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());

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

        [Theory]
        [InlineData(TextDocumentNames.DidOpen, 4)]
        [InlineData(TextDocumentNames.DidChange, 4)]
        [InlineData(TextDocumentNames.DidClose, 4)]
        [InlineData(TextDocumentNames.DidSave, 4)]
        public void Should_Contain_AllDefinedTextDocumentSyncMethods(string key, int count)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            var sub = (IJsonRpcHandler)TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.something"), "csharp");

            handler.Add(sub);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(GeneralNames.Exit, 4)]
        [InlineData(GeneralNames.Shutdown, 4)]
        [InlineData(GeneralNames.Initialized, 4)]
        [InlineData(GeneralNames.Initialize, 4)]
        public void Should_Contain_AllDefinedLanguageServerMethods(string key, int count)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
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
        [InlineData(GeneralNames.Exit, 4)]
        [InlineData(GeneralNames.Shutdown, 4)]
        [InlineData(GeneralNames.Initialized, 4)]
        [InlineData(GeneralNames.Initialize, 4)]
        public void Should_Contain_AllDefinedLanguageServerMethods_GivenDuplicates(string key, int count)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
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
        [InlineData(TextDocumentNames.DidOpen, 8)]
        [InlineData(TextDocumentNames.DidChange, 8)]
        [InlineData(TextDocumentNames.DidClose, 8)]
        [InlineData(TextDocumentNames.DidSave, 8)]
        public void Should_Contain_AllDefinedMethods_ForDifferentKeys(string key, int count)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            var sub = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");

            var sub2 = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cake"), "csharp");

            handler.Add(sub);
            handler.Add(sub2);
            handler._handlers.Should().Contain(x => x.Method == key);
            handler._handlers.Count.Should().Be(count);
        }

        [Theory]
        [InlineData(typeof(IInitializeHandler), typeof(IInitializedHandler), "initialize", "initialized", 2)]
        public void Should_Contain_AllDefinedMethods_OnLanguageServer(Type requestHandler, Type type2, string key, string key2, int count)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
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
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { requestHandler, type2 }, new object[0]);
            if (sub is IRegistration<TextDocumentRegistrationOptions> reg)
                reg.GetRegistrationOptions()
                    .Returns(new TextDocumentRegistrationOptions() {
                        DocumentSelector = new DocumentSelector() { }
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

        [Theory]
        [InlineData("somemethod", typeof(IJsonRpcRequestHandler<IRequest<object>, object>))]
        public void Should_AllowSpecificHandlers_ToBeAdded(string method, Type handlerType)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            var sub = (IJsonRpcHandler)Substitute.For(new Type[] { handlerType }, new object[0]);
            var sub2 = (IJsonRpcHandler)Substitute.For(new Type[] { handlerType }, new object[0]);
            handler.Add(method, sub);
            handler.Add(method, sub2);
            handler._handlers.Should().Contain(x => x.Method == method);
            handler._handlers.Should().Contain(x => x.Method == method);
            handler._handlers.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(Should_DealWithClassesThatImplementMultipleHandlers_WithoutConflictingRegistrations_Data))]
        public void Should_DealWithClassesThatImplementMultipleHandlers_WithoutConflictingRegistrations(string method, IJsonRpcHandler sub)
        {
            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            handler.Add(sub);

            var descriptor = handler._handlers.First(x => x.Method == method);
            descriptor.Key.Should().Be("default");
        }

        [Fact]
        public void Should_DealWithClassesThatImplementMultipleHandlers_BySettingKeyAccordingly()
        {
            var codeLensHandler = Substitute.For(new Type[] { typeof(ICodeLensHandler), typeof(ICodeLensResolveHandler) }, new object[0]);
            ((ICodeLensHandler)codeLensHandler).GetRegistrationOptions()
                .Returns(new CodeLensRegistrationOptions() {
                    DocumentSelector = new DocumentSelector(DocumentFilter.ForLanguage("foo"))
                });

            var handler = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, new TextDocumentIdentifiers());
            handler.Add(codeLensHandler as IJsonRpcHandler);

            var descriptor = handler._handlers.Select(x => x.Key);
            descriptor.Should().BeEquivalentTo(new [] { "[foo]", "[foo]" });
        }

        public static IEnumerable<object[]> Should_DealWithClassesThatImplementMultipleHandlers_WithoutConflictingRegistrations_Data()
        {
            var codeLensHandler = Substitute.For(new Type[] { typeof(ICodeLensHandler), typeof(ICodeLensResolveHandler) }, new object[0]);
            ((ICodeLensHandler)codeLensHandler).GetRegistrationOptions()
                    .Returns(new CodeLensRegistrationOptions() {
                        DocumentSelector = new DocumentSelector() { }
                    });

            yield return new object[] { TextDocumentNames.CodeLensResolve, codeLensHandler };

            var documentLinkHandler = Substitute.For(new Type[] { typeof(IDocumentLinkHandler), typeof(IDocumentLinkResolveHandler) }, new object[0]);
            ((IDocumentLinkHandler)documentLinkHandler).GetRegistrationOptions()
                .Returns(new DocumentLinkRegistrationOptions() {
                    DocumentSelector = new DocumentSelector() { }
                });

            yield return new object[] { TextDocumentNames.DocumentLinkResolve, documentLinkHandler };

            var completionHandler = Substitute.For(new Type[] { typeof(ICompletionHandler), typeof(ICompletionResolveHandler) }, new object[0]);
            ((ICompletionHandler)completionHandler).GetRegistrationOptions()
                .Returns(new CompletionRegistrationOptions() {
                    DocumentSelector = new DocumentSelector() { }
                });

            yield return new object[] { TextDocumentNames.CompletionResolve, completionHandler };
        }
    }
}

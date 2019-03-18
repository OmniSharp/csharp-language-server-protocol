using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Matchers
{
    public class TextDocumentMatcherTests : AutoTestBase
    {
        public TextDocumentMatcherTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().NotBeNull();
        }

        [Fact]
        public void Should_Return_Empty_Descriptor()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();

            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_Did_Open_Text_Document_Handler_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(new DidOpenTextDocumentParams() {
                TextDocument = new TextDocumentItem {
                    Uri = new Uri("file:///abc/123/d.cs")
                }
            },
                collection.Where(x => x.Method == DocumentNames.DidOpen));

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == DocumentNames.DidOpen);
        }

        [Fact]
        public void Should_Return_Did_Change_Text_Document_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                collection.Where(x => x.Method == DocumentNames.DidChange));

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == DocumentNames.DidChange);
        }

        [Fact]
        public void Should_Return_Did_Save_Text_Document_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                collection.Where(x => x.Method == DocumentNames.DidSave));

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == DocumentNames.DidSave);
        }

        [Fact]
        public void Should_Return_Did_Close_Text_Document_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(new DidCloseTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                collection.Where(x => x.Method == DocumentNames.DidClose));

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == DocumentNames.DidClose);
        }

        [Fact]
        public void Should_Return_Code_Lens_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));
            var collection = new HandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue) { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            var codeLensHandler = Substitute.For(new Type[] { typeof(ICodeLensHandler), typeof(ICodeLensResolveHandler) }, new object[0]) as ICodeLensHandler;
            codeLensHandler.GetRegistrationOptions()
                .Returns(new CodeLensRegistrationOptions() {
                    DocumentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.cs" })
                });

            var codeLensHandler2 = Substitute.For(new Type[] { typeof(ICodeLensHandler), typeof(ICodeLensResolveHandler) }, new object[0]) as ICodeLensHandler;
            codeLensHandler2.GetRegistrationOptions()
                .Returns(new CodeLensRegistrationOptions() {
                    DocumentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.cake" })
                });
            collection.Add(codeLensHandler, codeLensHandler2);

            // When
            var result = handlerMatcher.FindHandler(new CodeLensParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                collection.Where(x => x.Method == DocumentNames.CodeLens));

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == DocumentNames.CodeLens);
            result.Should().Contain(x => ((HandlerDescriptor)x).Key == "[**/*.cs]");
        }
    }
}

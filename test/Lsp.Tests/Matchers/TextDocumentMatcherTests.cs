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
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using Xunit;

namespace Lsp.Tests.Matchers
{
    public class TextDocumentMatcherTests
    {
        private readonly ILogger<TextDocumentMatcher> _logger;

        public TextDocumentMatcherTests()
        {
            _logger = Substitute.For<ILogger<TextDocumentMatcher>>();
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection());

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

            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection());

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
            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection() { textDocumentSyncHandler });

            // When
            var result = handlerMatcher.FindHandler(new DidOpenTextDocumentParams() {
                TextDocument = new TextDocumentItem {
                    Uri = new Uri("file:///abc/123/d.cs")
                }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor(DocumentNames.DidOpen,
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

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
            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection() { textDocumentSyncHandler });

            // When
            var result = handlerMatcher.FindHandler(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor(DocumentNames.DidChange,
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

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
            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection() { textDocumentSyncHandler });

            // When
            var result = handlerMatcher.FindHandler(new DidChangeTextDocumentParams() {
                TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
            },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor(DocumentNames.DidSave,
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

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
            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection() { textDocumentSyncHandler });

            // When
            var result = handlerMatcher.FindHandler(new DidCloseTextDocumentParams() {
                    TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
                },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor(DocumentNames.DidClose,
                        "Key",
                        textDocumentSyncHandler,
                        textDocumentSyncHandler.GetType(),
                        typeof(DidOpenTextDocumentParams),
                        typeof(TextDocumentRegistrationOptions),
                        typeof(TextDocumentClientCapabilities),
                        () => { })
                });

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
            var handlerMatcher = new TextDocumentMatcher(_logger, new HandlerCollection() { textDocumentSyncHandler });

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

            // When
            var result = handlerMatcher.FindHandler(new DidCloseTextDocumentParams() {
                    TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
                },
                new List<HandlerDescriptor>() {
                    new HandlerDescriptor(DocumentNames.CodeLens,
                        "Key2",
                        codeLensHandler2,
                        codeLensHandler2.GetType(),
                        typeof(CodeLensParams),
                        typeof(CodeLensRegistrationOptions),
                        typeof(CodeLensCapability),
                        () => { }),
                    new HandlerDescriptor(DocumentNames.CodeLens,
                        "Key",
                        codeLensHandler,
                        codeLensHandler.GetType(),
                        typeof(CodeLensParams),
                        typeof(CodeLensRegistrationOptions),
                        typeof(CodeLensCapability),
                        () => { }),
                });

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == DocumentNames.CodeLens);
            result.Should().Contain(x => ((HandlerDescriptor)x).Key == "Key");
        }
    }
}

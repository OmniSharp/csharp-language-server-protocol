using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc;
using FluentAssertions;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Shared;
using Xunit;
using Xunit.Abstractions;
using Arg = NSubstitute.Arg;

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
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(
                new DidOpenTextDocumentParams {
                    TextDocument = new TextDocumentItem {
                        Uri = new Uri("file:///abc/123/d.cs")
                    }
                },
                collection.Where(x => x.Method == TextDocumentNames.DidOpen)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.DidOpen);
        }

        [Fact]
        public void Should_Return_Did_Open_Text_Document_Handler_Descriptor_With_Sepcial_Character()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cshtml"), "csharp");
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(
                new DidOpenTextDocumentParams {
                    TextDocument = new TextDocumentItem {
                        Uri = new Uri("file://c:/users/myøasdf/d.cshtml")
                    }
                },
                collection.Where(x => x.Method == TextDocumentNames.DidOpen)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.DidOpen);
        }

        [Fact]
        public void Should_Return_Did_Change_Text_Document_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(
                new DidChangeTextDocumentParams {
                    TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
                },
                collection.Where(x => x.Method == TextDocumentNames.DidChange)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.DidChange);
        }

        [Fact]
        public void Should_Return_Did_Save_Text_Document_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(
                new DidChangeTextDocumentParams {
                    TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
                },
                collection.Where(x => x.Method == TextDocumentNames.DidSave)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.DidSave);
        }

        [Fact]
        public void Should_Return_Did_Close_Text_Document_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(
                new DidCloseTextDocumentParams {
                    TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
                },
                collection.Where(x => x.Method == TextDocumentNames.DidClose)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.DidClose);
        }

        [Fact]
        public void Should_Return_Code_Lens_Descriptor()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"), "csharp");
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            var codeLensHandler = (ICodeLensHandler) Substitute.For(new[] { typeof(ICodeLensHandler), typeof(ICodeLensResolveHandler) }, new object[0]);
            codeLensHandler.GetRegistrationOptions(Arg.Any<CodeLensCapability>())
                           .Returns(
                                new CodeLensRegistrationOptions {
                                    DocumentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.cs" })
                                }
                            );

            var codeLensHandler2 = (ICodeLensHandler) Substitute.For(new[] { typeof(ICodeLensHandler), typeof(ICodeLensResolveHandler) }, new object[0]);
            codeLensHandler2.GetRegistrationOptions(Arg.Any<CodeLensCapability>())
                            .Returns(
                                 new CodeLensRegistrationOptions {
                                     DocumentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.cake" })
                                 }
                             );
            collection.Add(codeLensHandler, codeLensHandler2);
            collection.Initialize();

            // When
            var result = handlerMatcher.FindHandler(
                new CodeLensParams {
                    TextDocument = new VersionedTextDocumentIdentifier { Uri = new Uri("file:///abc/123/d.cs"), Version = 1 }
                },
                collection.Where(x => x.Method == TextDocumentNames.CodeLens)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.CodeLens);
            lspHandlerDescriptors.Should().Contain(x => ( (LspHandlerDescriptor) x ).Key == "[**/*.cs]");
        }
    }

    public class FoldingRangeMatcherTests : AutoTestBase
    {
        public FoldingRangeMatcherTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Should_Return_Did_Folding_Range_handler()
        {
            // Given
            var textDocumentSyncHandler =
                TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.ps*1"), "powershell");
            var handler = Substitute.For<IFoldingRangeHandler>();
            handler.GetRegistrationOptions(Arg.Any<FoldingRangeCapability>()).Returns(
                new FoldingRangeRegistrationOptions {
                    DocumentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.ps*1" })
                }
            );
            var textDocumentIdentifiers = new TextDocumentIdentifiers();
            AutoSubstitute.Provide(textDocumentIdentifiers);
            var collection = new SharedHandlerCollection(SupportedCapabilitiesFixture.AlwaysTrue, textDocumentIdentifiers, Substitute.For<IResolverContext>(),
                                                         new LspHandlerTypeDescriptorProvider(new [] { typeof(FoundationTests).Assembly, typeof(LanguageServer).Assembly, typeof(LanguageClient).Assembly, typeof(IRegistrationManager).Assembly, typeof(LspRequestRouter).Assembly }))
                { textDocumentSyncHandler, handler };
            AutoSubstitute.Provide<IHandlerCollection>(collection);
            AutoSubstitute.Provide<IEnumerable<ILspHandlerDescriptor>>(collection);
            var handlerMatcher = AutoSubstitute.Resolve<TextDocumentMatcher>();

            // When
            var result = handlerMatcher.FindHandler(
                new FoldingRangeRequestParam {
                    TextDocument = new TextDocumentItem {
                        Uri = new Uri("file:///abc/123/d.ps1")
                    }
                },
                collection.Where(x => x.Method == TextDocumentNames.DidOpen)
            );

            // Then
            var lspHandlerDescriptors = result as ILspHandlerDescriptor[] ?? result.ToArray();
            lspHandlerDescriptors.Should().NotBeNullOrEmpty();
            lspHandlerDescriptors.Should().Contain(x => x.Method == TextDocumentNames.DidOpen);
        }
    }
}

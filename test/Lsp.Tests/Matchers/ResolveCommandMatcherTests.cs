using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using Xunit;

namespace Lsp.Tests.Matchers
{
    public class ResolveCommandMatcherTests
    {
        private readonly ILogger _logger;

        public ResolveCommandMatcherTests()
        {
            _logger = Substitute.For<ILogger>();
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = new ResolveCommandMatcher(_logger);

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
            var handlerMatcher = new ResolveCommandMatcher(_logger);

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_Not_Throw_Given_Another_Descriptor()
        {
            // Given
            var resolveHandler = Substitute.For<ICodeLensHandler>();
            var handlerDescriptor = new HandlerDescriptor(
                        DocumentNames.CodeLens,
                        "Key",
                        resolveHandler,
                        resolveHandler.GetType(),
                        typeof(CodeLensParams),
                        null,
                        null,
                        () => { });
            var handlerMatcher = new ResolveCommandMatcher(_logger);

            // When
            Action a = () => handlerMatcher.Process(handlerDescriptor, new CodeLensParams(), new CodeLensContainer());
            a.ShouldNotThrow();
        }

        [Fact]
        public void Should_Return_CodeLensResolve_Descriptor()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICodeLensResolveHandler>();
            var resolveHandler2 = Substitute.For<ICodeLensResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CodeLens>()).Returns(false);
            resolveHandler2.CanResolve(Arg.Any<CodeLens>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CodeLens() {
                Data = JToken.FromObject(new { handlerType = typeof(ICodeLensResolveHandler).FullName, data = new { a = 1 } })
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(DocumentNames.CodeLensResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLens),
                            null,
                            null,
                            () => { }),
                        new HandlerDescriptor(DocumentNames.CodeLensResolve,
                            "Key2",
                            resolveHandler2,
                            typeof(ICodeLensResolveHandler),
                            typeof(CodeLens),
                            null,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Handle_Null_Data()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem() { },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Handler == resolveHandler);
        }

        [Fact]
        public void Should_Return_CompletionResolve_Descriptor()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            var resolveHandler2 = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(false);
            resolveHandler2.CanResolve(Arg.Any<CompletionItem>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem() {
                Data = JToken.FromObject(new { handlerType = typeof(ICompletionResolveHandler).FullName, data = new { a = 1 } })
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2,
                            typeof(ICompletionResolveHandler),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Deal_WithHandlers_That_Not_Also_Resolvers()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(false);
            var resolveHandler2 = Substitute.For(new Type[] {
                typeof(ICompletionHandler),
                typeof(ICompletionResolveHandler)
            }, new object[0]);
            (resolveHandler2 as ICompletionResolveHandler).CanResolve(Arg.Any<CompletionItem>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem() {
                Data = new JObject()
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2 as IJsonRpcHandler,
                            typeof(ICompletionResolveHandler),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Deal_WithHandlers_That_Not_Also_Resolvers2()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(true);
            var resolveHandler2 = Substitute.For(new Type[] {
                typeof(ICompletionHandler),
                typeof(ICompletionResolveHandler)
            }, new object[0]);
            (resolveHandler2 as ICompletionResolveHandler).CanResolve(Arg.Any<CompletionItem>()).Returns(false);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem() {
                Data = new JObject()
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                        new HandlerDescriptor(DocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2 as IJsonRpcHandler,
                            typeof(ICompletionResolveHandler),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Handler == resolveHandler);
        }

        [Fact]
        public void Should_FindPostProcessor_AsSelf_For_CodeLens()
        {
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICodeLensHandler>();
            var descriptor = new HandlerDescriptor(
                            DocumentNames.CodeLens,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLensParams),
                            null,
                            null,
                            () => { });

            // When
            var processors = handlerMatcher
                .FindPostProcessor(descriptor, new CodeLensParams(), new CodeLensContainer())
                .ToArray();

            // Then
            processors.Should().Contain(x => x == handlerMatcher);
        }

        [Fact]
        public void Should_FindPostProcessor_AsSelf_For_Completion()
        {
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICompletionHandler>();
            var descriptor = new HandlerDescriptor(
                            DocumentNames.Completion,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLensParams),
                            null,
                            null,
                            () => { });

            // When
            var processors = handlerMatcher
                .FindPostProcessor(descriptor, new CompletionParams(), new CompletionList(Enumerable.Empty<CompletionItem>()))
                .ToArray();

            // Then
            processors.Should().Contain(x => x == handlerMatcher);
        }

        [Fact]
        public void Should_Update_CompletionItems_With_HandlerType()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICompletionHandler),
                typeof(ICompletionResolveHandler)
            }, new object[0]);
            (resolveHandler as ICompletionResolveHandler).CanResolve(Arg.Any<CompletionItem>()).Returns(true);
            var descriptor = new HandlerDescriptor(
                            DocumentNames.Completion,
                            "Key",
                            resolveHandler as IJsonRpcHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            () => { });

            var item = new CompletionItem() {
                Data = JObject.FromObject(new { hello = "world" })
            };
            var list = new CompletionList(new[] { item });

            (list is IEnumerable<ICanBeResolved>).Should().BeTrue();

            // When
            var response = handlerMatcher.Process(descriptor, new CompletionParams(), list);

            // Then
            response.Should().Be(list);
            (response as CompletionList).Items.Should().Contain(item);
            var responseItem = (response as CompletionList).Items.First();
            responseItem.Data[ResolveCommandMatcher.PrivateHandlerTypeName].Value<string>().Should().NotBeNullOrEmpty();
            responseItem.Data["data"]["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public void Should_Update_CodeLensContainer_With_HandlerType()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICodeLensHandler),
                typeof(ICodeLensResolveHandler)
            }, new object[0]);
            (resolveHandler as ICodeLensResolveHandler).CanResolve(Arg.Any<CodeLens>()).Returns(true);
            var descriptor = new HandlerDescriptor(
                            DocumentNames.CodeLens,
                            "Key",
                            resolveHandler as IJsonRpcHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLens),
                            null,
                            null,
                            () => { });

            var item = new CodeLens() {
                Data = JObject.FromObject(new { hello = "world" })
            };
            var list = new CodeLensContainer(new[] { item });

            (list is IEnumerable<ICanBeResolved>).Should().BeTrue();

            // When
            var response = handlerMatcher.Process(descriptor, new CodeLensParams(), list);

            // Then
            response.Should().Be(list);
            (response as CodeLensContainer).Should().Contain(item);
            var responseItem = (response as CodeLensContainer).First();
            responseItem.Data[ResolveCommandMatcher.PrivateHandlerTypeName].Value<string>().Should().NotBeNullOrEmpty();
            responseItem.Data["data"]["hello"].Value<string>().Should().Be("world");
        }
    }
}

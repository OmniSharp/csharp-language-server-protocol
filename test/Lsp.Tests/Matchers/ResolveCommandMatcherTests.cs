using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Matchers
{
    public class ResolveCommandMatcherTests : AutoTestBase
    {
        public ResolveCommandMatcherTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();

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
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();

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
                        TextDocumentNames.CodeLens,
                        "Key",
                        resolveHandler,
                        resolveHandler.GetType(),
                        typeof(CodeLensParams),
                        null,
                        null,
                        false,
                        null,
                        () => { });
            var handlerMatcher = new ResolveCommandPipeline<CodeLensParams, CodeLensContainer>(
                new RequestContext() { Descriptor = handlerDescriptor },
                LoggerFactory.CreateLogger<ResolveCommandPipeline<CodeLensParams, CodeLensContainer>>());

            // When
            Func<Task> a = async () => await handlerMatcher.Handle(new CodeLensParams(), CancellationToken.None, () => Task.FromResult(new CodeLensContainer()));
            a.Should().NotThrow();
        }

        [Fact]
        public void Should_Return_CodeLensResolve_Descriptor()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICodeLensResolveHandler>();
            var resolveHandler2 = Substitute.For<ICodeLensResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CodeLens>()).Returns(false);
            resolveHandler2.CanResolve(Arg.Any<CodeLens>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CodeLens()
            {
                Data = JToken.FromObject(new { handlerType = typeof(ICodeLensResolveHandler).FullName, data = new { a = 1 } })
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(TextDocumentNames.CodeLensResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLens),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                        new HandlerDescriptor(TextDocumentNames.CodeLensResolve,
                            "Key2",
                            resolveHandler2,
                            typeof(ICodeLensResolveHandler),
                            typeof(CodeLens),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Handle_Null_Data()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem() { },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler);
        }

        [Fact]
        public void Should_Handle_Simple_Json_Data()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem()
            {
                Data = new Uri("file:///c%3A/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler);
        }

        [Fact]
        public void Should_Return_CompletionResolve_Descriptor()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            var resolveHandler2 = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(false);
            resolveHandler2.CanResolve(Arg.Any<CompletionItem>()).Returns(true);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem()
            {
                Data = JToken.FromObject(new { handlerType = typeof(ICompletionResolveHandler).FullName, data = new { a = 1 } })
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2,
                            typeof(ICompletionResolveHandler),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Deal_WithHandlers_That_Not_Also_Resolvers()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(false);
            var resolveHandler2 = Substitute.For(new Type[] {
                typeof(ICompletionHandler),
                typeof(ICompletionResolveHandler)
            }, new object[0]) as IJsonRpcHandler;
            (resolveHandler2 as ICompletionResolveHandler).CanResolve(Arg.Any<CompletionItem>()).Returns(true);
            (resolveHandler2 as ICompletionHandler).GetRegistrationOptions().Returns(
                new CompletionRegistrationOptions()
                {
                    DocumentSelector = DocumentSelector.ForLanguage("csharp")
                });

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem()
            {
                Data = new JObject()
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2 as IJsonRpcHandler,
                            typeof(ICompletionResolveHandler),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public void Should_Deal_WithHandlers_That_Not_Also_Resolvers2()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            resolveHandler.CanResolve(Arg.Any<CompletionItem>()).Returns(true);
            var resolveHandler2 = Substitute.For(new Type[] {
                typeof(ICompletionHandler),
                typeof(ICompletionResolveHandler)
            }, new object[0]);
            (resolveHandler2 as ICompletionResolveHandler).CanResolve(Arg.Any<CompletionItem>()).Returns(false);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem()
            {
                Data = new JObject()
            },
                    new List<HandlerDescriptor> {
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                        new HandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2 as IJsonRpcHandler,
                            typeof(ICompletionResolveHandler),
                            typeof(CompletionItem),
                            null,
                            null,
                            false,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler);
        }

        [Fact]
        public async Task Should_Update_CompletionItems_With_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICompletionHandler),
                typeof(ICompletionResolveHandler)
            }, new object[0]);
            (resolveHandler as ICompletionResolveHandler).CanResolve(Arg.Any<CompletionItem>()).Returns(true);
            var descriptor = new HandlerDescriptor(
                            TextDocumentNames.Completion,
                            "Key",
                            resolveHandler as IJsonRpcHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionParams),
                            null,
                            null,
                            false,
                            null,
                            () => { });
            var handlerMatcher = new ResolveCommandPipeline<CompletionParams, CompletionList>(
                new RequestContext() { Descriptor = descriptor },
                Substitute.For<ILogger<ResolveCommandPipeline<CompletionParams, CompletionList>>>());

            var item = new CompletionItem()
            {
                Data = JObject.FromObject(new { hello = "world" })
            };
            var list = new CompletionList(new[] { item });

            (list is IEnumerable<ICanBeResolved>).Should().BeTrue();

            // When
            var response = await handlerMatcher.Handle(new CompletionParams(), CancellationToken.None, () => Task.FromResult(list));

            // Then
            response.Should().BeEquivalentTo(list);
            (response as CompletionList).Items.Should().Contain(item);
            var responseItem = (response as CompletionList).Items.First();
            responseItem.Data[ResolveCommandMatcher.PrivateHandlerTypeName].Value<string>().Should().NotBeNullOrEmpty();
            responseItem.Data[ResolveCommandMatcher.PrivateHandlerKey].Value<string>().Should().NotBeNullOrEmpty();
            responseItem.Data["data"]["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public async Task Should_Update_CodeLensContainer_With_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICodeLensHandler),
                typeof(ICodeLensResolveHandler)
            }, new object[0]);
            (resolveHandler as ICodeLensResolveHandler).CanResolve(Arg.Any<CodeLens>()).Returns(true);
            var descriptor = new HandlerDescriptor(
                            TextDocumentNames.CodeLens,
                            "Key",
                            resolveHandler as IJsonRpcHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLensParams),
                            null,
                            null,
                            false,
                            null,
                            () => { });
            var handlerMatcher = new ResolveCommandPipeline<CodeLensParams, CodeLensContainer>(
                new RequestContext() { Descriptor = descriptor },
                Substitute.For<ILogger<ResolveCommandPipeline<CodeLensParams, CodeLensContainer>>>());

            var item = new CodeLens()
            {
                Data = JObject.FromObject(new { hello = "world" })
            };
            var list = new CodeLensContainer(new[] { item });

            (list is IEnumerable<ICanBeResolved>).Should().BeTrue();

            // When
            var response = await handlerMatcher.Handle(new CodeLensParams(), CancellationToken.None, () => Task.FromResult(list));

            // Then
            response.Should().BeEquivalentTo(list);
            (response as CodeLensContainer).Should().Contain(item);
            var responseItem = (response as CodeLensContainer).First();
            responseItem.Data[ResolveCommandMatcher.PrivateHandlerTypeName].Value<string>().Should().NotBeNullOrEmpty();
            responseItem.Data[ResolveCommandMatcher.PrivateHandlerKey].Value<string>().Should().NotBeNullOrEmpty();
            responseItem.Data["data"]["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public async Task Should_Update_CodeLens_Removing_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICodeLensHandler),
                typeof(ICodeLensResolveHandler)
            }, new object[0]);
            (resolveHandler as ICodeLensResolveHandler).CanResolve(Arg.Any<CodeLens>()).Returns(true);
            var descriptor = new HandlerDescriptor(
                            TextDocumentNames.CodeLensResolve,
                            "Key",
                            resolveHandler as IJsonRpcHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLens),
                            null,
                            null,
                            false,
                            null,
                            () => { });
            var handlerMatcher = new ResolveCommandPipeline<CodeLens, CodeLens>(
                new RequestContext() { Descriptor = descriptor },
                Substitute.For<ILogger<ResolveCommandPipeline<CodeLens, CodeLens>>>());

            var item = new CodeLens()
            {
                Data = JObject.FromObject(new { data = new { hello = "world" } })
            };
            item.Data[ResolveCommandMatcher.PrivateHandlerTypeName] = resolveHandler.GetType().FullName;

            // When
            var response = await handlerMatcher.Handle(item, CancellationToken.None, () => Task.FromResult(item));

            // Then
            response.Should().BeEquivalentTo(item);
            item.Data?[ResolveCommandMatcher.PrivateHandlerTypeName].Should().BeNull();
            item.Data["hello"].Value<string>().Should().Be("world");
        }
    }
}

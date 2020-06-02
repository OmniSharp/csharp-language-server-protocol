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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Server.Matchers;
using OmniSharp.Extensions.LanguageServer.Server.Pipelines;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Matchers
{
    public class ResolveCommandMatcherTests : AutoTestBase
    {
        private readonly Guid TrueId = Guid.NewGuid();
        private readonly Guid FalseId = Guid.NewGuid();
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
            var resolveHandler = Substitute.For<ICodeLensHandler<ResolvedData>>();
            var handlerDescriptor = new LspHandlerDescriptor(
                TextDocumentNames.CodeLens,
                "Key",
                resolveHandler,
                resolveHandler.GetType(),
                typeof(CodeLensParams<ResolvedData>),
                null,
                null,
                () => false,
                null,
                null,
                () => { });
            var handlerMatcher = new ResolveCommandPipeline<CodeLensParams<ResolvedData>, CodeLensContainer<ResolvedData>>(
                new RequestContext() {Descriptor = handlerDescriptor},
                LoggerFactory.CreateLogger<ResolveCommandPipeline<CodeLensParams<ResolvedData>, CodeLensContainer<ResolvedData>>>());

            // When
            Func<Task> a = async () => await handlerMatcher.Handle(new CodeLensParams<ResolvedData>(), CancellationToken.None, () => Task.FromResult(new CodeLensContainer<ResolvedData>()));
            a.Should().NotThrow();
        }

        [Fact]
        public void Should_Return_CodeLensResolve_Descriptor()
        {
            // Given
            var handlerMatcher = AutoSubstitute.Resolve<ResolveCommandMatcher>();
            var resolveHandler = Substitute.For<ICodeLensResolveHandler<ResolvedData>>();
            var resolveHandler2 = Substitute.For<ICodeLensResolveHandler<ResolvedData>>();
            resolveHandler.Id.Returns(FalseId);
            resolveHandler2.Id.Returns(TrueId);

            // When
            var result = handlerMatcher.FindHandler(new CodeLens<ResolvedData>() {
                        Data = new ResolvedData() { handler = TrueId, Data = new Dictionary<string, JToken>() { ["a"] = 1 }}
                    },
                    new List<LspHandlerDescriptor> {
                        new LspHandlerDescriptor(TextDocumentNames.CodeLensResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CodeLens<ResolvedData>),
                            null,
                            null,
                            () => false,
                            null,
                            null,
                            () => { }),
                        new LspHandlerDescriptor(TextDocumentNames.CodeLensResolve,
                            "Key2",
                            resolveHandler2,
                            typeof(ICodeLensResolveHandler<ResolvedData>),
                            typeof(CodeLens<ResolvedData>),
                            null,
                            null,
                            () => false,
                            null,
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
            var resolveHandler = Substitute.For<ICompletionResolveHandler<ResolvedData>>();
            resolveHandler.Id.Returns(TrueId);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem<ResolvedData>() { },
                    new List<LspHandlerDescriptor> {
                        new LspHandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem<ResolvedData>),
                            null,
                            null,
                            () => false,
                            null,
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
            var resolveHandler = Substitute.For<ICompletionResolveHandler<ResolvedData>>();
            var resolveHandler2 = Substitute.For<ICompletionResolveHandler<ResolvedData>>();
            resolveHandler.Id.Returns(FalseId);
            resolveHandler2.Id.Returns(TrueId);

            // When
            var result = handlerMatcher.FindHandler(new CompletionItem<ResolvedData>() {
                        Data = new ResolvedData() { handler = TrueId,  Data = new Dictionary<string, JToken>() { ["a"] = 1 }}
                    },
                    new List<LspHandlerDescriptor> {
                        new LspHandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key",
                            resolveHandler,
                            resolveHandler.GetType(),
                            typeof(CompletionItem<ResolvedData>),
                            null,
                            null,
                            () => false,
                            null,
                            null,
                            () => { }),
                        new LspHandlerDescriptor(TextDocumentNames.CompletionResolve,
                            "Key2",
                            resolveHandler2,
                            typeof(ICompletionResolveHandler<ResolvedData>),
                            typeof(CompletionItem<ResolvedData>),
                            null,
                            null,
                            () => false,
                            null,
                            null,
                            () => { }),
                    })
                .ToArray();

            // Then
            result.Should().NotBeNullOrEmpty();
            result.OfType<IHandlerDescriptor>().Should().Contain(x => x.Handler == resolveHandler2);
        }

        [Fact]
        public async Task Should_Update_CompletionItems_With_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICompletionHandler<ResolvedData>),
                typeof(ICompletionResolveHandler<ResolvedData>)
            }, new object[0]);
            (resolveHandler as ICompletionResolveHandler<ResolvedData>).Id.Returns(TrueId);
            var descriptor = new LspHandlerDescriptor(
                TextDocumentNames.Completion,
                "Key",
                resolveHandler as IJsonRpcHandler,
                resolveHandler.GetType(),
                typeof(CompletionParams<ResolvedData>),
                null,
                null,
                () => false,
                null,
                null,
                () => { });
            var handlerMatcher = new ResolveCommandPipeline<CompletionParams<ResolvedData>, CompletionList>(
                new RequestContext() {Descriptor = descriptor},
                Substitute.For<ILogger<ResolveCommandPipeline<CompletionParams<ResolvedData>, CompletionList>>>());

            var item = new CompletionItem<ResolvedData>() {
                Data = new ResolvedData() { Data = new Dictionary<string, JToken>() { ["hello"] = "world"}}
            };
            var list = new CompletionList(new[] {item});

            (list is IEnumerable<ICanBeResolved<ResolvedData>>).Should().BeTrue();

            // When
            var response = await handlerMatcher.Handle(new CompletionParams<ResolvedData>(), CancellationToken.None, () => Task.FromResult(list));

            // Then
            response.Should().BeEquivalentTo(list);
            (response as CompletionList).Items.Should().Contain(item);
            var responseItem = (response as CompletionList).Items.First();
            responseItem.Data.handler.Should().Be(TrueId);
            responseItem.Data.Data["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public async Task Should_Update_CodeLensContainer_With_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICodeLensHandler<ResolvedData>),
                typeof(ICodeLensResolveHandler<ResolvedData>)
            }, new object[0]);
            (resolveHandler as ICodeLensResolveHandler<ResolvedData>).Id.Returns(TrueId);
            var descriptor = new LspHandlerDescriptor(
                TextDocumentNames.CodeLens,
                "Key",
                resolveHandler as IJsonRpcHandler,
                resolveHandler.GetType(),
                typeof(CodeLensParams<ResolvedData>),
                null,
                null,
                () => false,
                null,
                null,
                () => { });
            var handlerMatcher = new ResolveCommandPipeline<CodeLensParams<ResolvedData>, CodeLensContainer<ResolvedData>>(
                new RequestContext() {Descriptor = descriptor},
                Substitute.For<ILogger<ResolveCommandPipeline<CodeLensParams<ResolvedData>, CodeLensContainer<ResolvedData>>>>());

            var item = new CodeLens<ResolvedData>() {
                Data = new ResolvedData() { Data = new Dictionary<string, JToken>() { ["hello"] = "world"}}
            };
            var list = new CodeLensContainer<ResolvedData>(new[] {item});

            (list is IEnumerable<ICanBeResolved<ResolvedData>>).Should().BeTrue();

            // When
            var response = await handlerMatcher.Handle(new CodeLensParams<ResolvedData>(), CancellationToken.None, () => Task.FromResult(list));

            // Then
            response.Should().BeEquivalentTo(list);
            (response as CodeLensContainer<ResolvedData>).Should().Contain(item);
            var responseItem = (response as CodeLensContainer<ResolvedData>).First();
            responseItem.Data.handler.Should().Be(TrueId);
            responseItem.Data.Data["hello"].Value<string>().Should().Be("world");
        }

        [Fact]
        public async Task Should_Update_CodeLens_Removing_HandlerType()
        {
            // Given
            var resolveHandler = Substitute.For(new Type[] {
                typeof(ICodeLensHandler<ResolvedData>),
                typeof(ICodeLensResolveHandler<ResolvedData>)
            }, new object[0]);

            (resolveHandler as ICodeLensResolveHandler<ResolvedData>).Id.Returns(TrueId);
            var descriptor = new LspHandlerDescriptor(
                TextDocumentNames.CodeLensResolve,
                "Key",
                resolveHandler as IJsonRpcHandler,
                resolveHandler.GetType(),
                typeof(CodeLens<ResolvedData>),
                null,
                null,
                () => false,
                null,
                null,
                () => { });
            var handlerMatcher = new ResolveCommandPipeline<CodeLens<ResolvedData>, CodeLens<ResolvedData>>(
                new RequestContext() {Descriptor = descriptor},
                Substitute.For<ILogger<ResolveCommandPipeline<CodeLens<ResolvedData>, CodeLens<ResolvedData>>>>());

            var item = new CodeLens<ResolvedData>() {
                Data = new ResolvedData() { Data = new Dictionary<string, JToken>() { ["hello"] = "world"}}
            };
            item.Data.handler = TrueId;

            // When
            var response = await handlerMatcher.Handle(item, CancellationToken.None, () => Task.FromResult(item));

            // Then
            response.Should().BeEquivalentTo(item);
            item.Data.Data["hello"].Value<string>().Should().Be("world");
        }
    }
}

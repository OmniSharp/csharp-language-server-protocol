using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
    public class ExecuteCommandHandlerMatcherTests
    {
        private readonly ILogger _logger;

        public ExecuteCommandHandlerMatcherTests()
        {
            _logger = Substitute.For<ILogger>();
        }

        [Fact]
        public void Should_Not_Return_Null()
        {
            // Given
            var handlerDescriptors = Enumerable.Empty<ILspHandlerDescriptor>();
            var handlerMatcher = new ExecuteCommandMatcher(_logger);

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
            var handlerMatcher = new ExecuteCommandMatcher(_logger);

            // When
            var result = handlerMatcher.FindHandler(1, handlerDescriptors);

            // Then
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_Return_Handler_Descriptor()
        {
            // Given
            var handlerMatcher = new ExecuteCommandMatcher(_logger);
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>().With(new Container<string>("Command"));

            // When
            var result = handlerMatcher.FindHandler(new ExecuteCommandParams { Command = "Command" },
                new List<HandlerDescriptor> {
                    new HandlerDescriptor("workspace/executeCommand",
                        "Key",
                        executeCommandHandler,
                        executeCommandHandler.GetType(),
                        typeof(ExecuteCommandParams),
                        typeof(ExecuteCommandRegistrationOptions),
                        typeof(ExecuteCommandCapability),
                        () => { })
                });

            // Then
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(x => x.Method == "workspace/executeCommand");
        }
    }
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
        public void Should_Return_CodeLensResolve_Descriptor()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICodeLensResolveHandler>();
            var resolveHandler2 = Substitute.For<ICodeLensResolveHandler>();

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
        public void Should_Return_CompletionResolve_Descriptor()
        {
            // Given
            var handlerMatcher = new ResolveCommandMatcher(_logger);
            var resolveHandler = Substitute.For<ICompletionResolveHandler>();
            var resolveHandler2 = Substitute.For<ICompletionResolveHandler>();

            // When
            var result = handlerMatcher.FindHandler(new CodeLens() {
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
    }
}

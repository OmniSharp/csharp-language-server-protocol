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
    public class ExecuteCommandHandlerMatcherTests
    {
        private readonly ILogger<ExecuteCommandMatcher> _logger;

        public ExecuteCommandHandlerMatcherTests()
        {
            _logger = Substitute.For<ILogger<ExecuteCommandMatcher>>();
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
}

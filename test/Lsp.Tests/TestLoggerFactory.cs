using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Models;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using HandlerCollection = OmniSharp.Extensions.LanguageServer.HandlerCollection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Lsp.Tests
{
    public class TestLoggerFactory : ILoggerFactory
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly SerilogLoggerProvider _loggerProvider;

        public TestLoggerFactory(ITestOutputHelper testOutputHelper)
        {
            _loggerProvider = new SerilogLoggerProvider(
                new Serilog.LoggerConfiguration()
                    .WriteTo.TestOutput(testOutputHelper)
                    .CreateLogger()
            );
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName)
        {
            return _loggerProvider.CreateLogger(categoryName);
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider) { }

        void IDisposable.Dispose() { }
    }

    public class ClientCapabilityProviderTests
    {
        private static Type[] TextDocumentCapabilities = {
            typeof(CompletionCapability),
            typeof(HoverCapability),
            typeof(SignatureHelpCapability),
            typeof(ReferencesCapability),
            typeof(DocumentHighlightCapability),
            typeof(DocumentSymbolCapability),
            typeof(DocumentFormattingCapability),
            typeof(DocumentRangeFormattingCapability),
            typeof(DocumentOnTypeFormattingCapability),
            typeof(DefinitionCapability),
            typeof(CodeActionCapability),
            typeof(CodeLensCapability),
            typeof(DocumentLinkCapability),
            typeof(RenameCapability),
            typeof(WorkspaceSymbolCapability),
            typeof(ExecuteCommandCapability),
        };

        [Theory, MemberData(nameof(AllowSupportedCapabilities))]
        public void Should_AllowSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var collection = new HandlerCollection { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection);

            HasHandler(provider, instance).Should().BeTrue();
        }

        public static IEnumerable<object[]> AllowSupportedCapabilities()
        {
            return GetItems(TextDocumentCapabilities, type => {
                var handlerType = GetHandlerType(type);
                var handler = Substitute.For(new Type[] { handlerType }, new object[0]);
                return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), true, Activator.CreateInstance(type)) };
            });
        }

        [Theory, MemberData(nameof(DisallowUnsupportedCapabilities))]
        public void Should_DisallowUnsupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var collection = new HandlerCollection { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection);

            HasHandler(provider, instance).Should().BeFalse();
        }

        public static IEnumerable<object[]> DisallowUnsupportedCapabilities()
        {
            return GetItems(TextDocumentCapabilities, type => {
                var handlerType = GetHandlerType(type);
                var handler = Substitute.For(new Type[] { handlerType }, new object[0]);
                return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), false) };
            });
        }

        [Theory, MemberData(nameof(DisallowNullSupportsCapabilities))]
        public void Should_DisallowNullSupportedCapabilities(IJsonRpcHandler handler, object instance)
        {
            var textDocumentSyncHandler = TextDocumentSyncHandlerExtensions.With(DocumentSelector.ForPattern("**/*.cs"));

            var collection = new HandlerCollection { textDocumentSyncHandler, handler };
            var provider = new ClientCapabilityProvider(collection);

            HasHandler(provider, instance).Should().BeFalse();
        }

        public static IEnumerable<object[]> DisallowNullSupportsCapabilities()
        {
            return GetItems(TextDocumentCapabilities, type => {
                var handlerType = GetHandlerType(type);
                var handler = Substitute.For(new Type[] { handlerType }, new object[0]);
                return new[] { handler, Activator.CreateInstance(typeof(Supports<>).MakeGenericType(type), true) };
            });
        }

        private static bool HasHandler(ClientCapabilityProvider provider, object instance)
        {
            return (bool)typeof(ClientCapabilityProviderTests).GetTypeInfo()
                .GetMethod(nameof(GenericHasHandler), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(instance.GetType().GetTypeInfo().GetGenericArguments()[0]).Invoke(null, new[] { provider, instance });
        }

        private static bool HasHandler(ClientCapabilityProvider provider, Type type)
        {
            return (bool)typeof(ClientCapabilityProviderTests).GetTypeInfo()
                .GetMethod(nameof(GenericHasHandler), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(type).Invoke(null, new object[] { provider, null });
        }

        private static bool GenericHasHandler<T>(ClientCapabilityProvider provider, Supports<T> supports)
            where T : DynamicCapability, ConnectedCapability<IJsonRpcHandler>
        {
            return provider.HasHandler(supports);
        }

        private static IEnumerable<object[]> GetItems<T>(IEnumerable<T> types, Func<T, IEnumerable<object>> func)
        {
            return types.Select(x => func(x).ToArray());
        }

        private static Type GetHandlerType(Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces
                .Single(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ConnectedCapability<>))
                .GetTypeInfo().GetGenericArguments()[0];
        }
    }
}

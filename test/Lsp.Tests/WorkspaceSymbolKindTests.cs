using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Xunit;
#pragma warning disable 618

namespace Lsp.Tests
{
    public class WorkspaceSymbolKindTests
    {
        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialSymbolKinds()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(new SymbolInformation()
            {
                Kind = SymbolKind.Event
            });

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Kind.Should().Be(SymbolKind.Event);
        }

        [Fact]
        public void DefaultBehavior_Should_Only_Support_InitialSymbolTags()
        {
            var serializer = new Serializer();
            var json = serializer.SerializeObject(new SymbolInformation()
            {
                Tags = new Container<SymbolTag>(SymbolTag.Deprecated)
            });

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Tags.Should().Contain(SymbolTag.Deprecated);
        }

        [Fact]
        public void CustomBehavior_When_SymbolKind_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(ClientVersion.Lsp3, new ClientCapabilities()
            {
                Workspace = new WorkspaceClientCapabilities()
                {
                    Symbol = new Supports<WorkspaceSymbolCapability>(true, new WorkspaceSymbolCapability()
                    {
                        DynamicRegistration = true,
                        SymbolKind = new SymbolKindCapability()
                        {
                            ValueSet = new Container<SymbolKind>(SymbolKind.Class)
                        }
                    })
                }
            });
            var json = serializer.SerializeObject(new SymbolInformation()
            {
                Kind = SymbolKind.Event
            });

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Kind.Should().Be(SymbolKind.Class);
        }

        [Fact]
        public void CustomBehavior_When_SymbolTag_Defined_By_Client()
        {
            var serializer = new Serializer();
            serializer.SetClientCapabilities(ClientVersion.Lsp3, new ClientCapabilities()
            {
                Workspace = new WorkspaceClientCapabilities()
                {
                    Symbol = new Supports<WorkspaceSymbolCapability>(true, new WorkspaceSymbolCapability()
                    {
                        DynamicRegistration = true,
                        TagSupport = new TagSupportCapability() {
                            ValueSet = new Container<SymbolTag>()
                        }
                    })
                }
            });

            var json = serializer.SerializeObject(new SymbolInformation()
            {
                Tags = new Container<SymbolTag>(SymbolTag.Deprecated)
            });

            var result = serializer.DeserializeObject<SymbolInformation>(json);
            result.Tags.Should().BeEmpty();
        }
    }

    public class FoundationTests
    {
        [Theory(DisplayName = "Params types should have a method attribute")]
        [ClassData(typeof(ParamsShouldHaveMethodAttributeData))]
        public void ParamsShouldHaveMethodAttribute(Type type)
        {
            type.GetCustomAttributes<MethodAttribute>().Any().Should()
                .Be(true, $"{type.Name} is missing a method attribute");
        }

        [Theory(DisplayName = "Handler interfaces should have a method attribute")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldHaveMethodAttribute(Type type)
        {
            type.GetCustomAttributes<MethodAttribute>().Any().Should()
                .Be(true, $"{type.Name} is missing a method attribute");
        }

        [Theory(DisplayName = "Handler interfaces should have a method attribute")]
        [ClassData(typeof(HandlersShouldAbstractClassData))]
        public void HandlersShouldAbstractClass(Type type)
        {
            var types = type.Assembly.ExportedTypes;
            var abstractHandler = type.Assembly.ExportedTypes
                .FirstOrDefault(z => z.IsAbstract && z.IsClass && type.IsAssignableFrom(z));
            abstractHandler.Should().NotBeNull($"{type.Name} is missing abstract base class");
            var delegatingHandler = type.Assembly.DefinedTypes.FirstOrDefault(z => abstractHandler.IsAssignableFrom(z) && abstractHandler != z);
            delegatingHandler.Should().NotBeNull($"{type.Name} is missing delegating handler class");
            delegatingHandler.DeclaringType.Should().NotBeNull();
            delegatingHandler.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(z => z.Name.StartsWith("On"))
                .Should().BeTrue($"{type.Name} is missing delegating extension method");
        }

        public class ParamsShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public ParamsShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                    .Where(z => z.IsClass && !z.IsAbstract && z.GetInterfaces().Any(z =>
                        z.IsGenericType &&
                        typeof(IRequest<>).IsAssignableFrom(z.GetGenericTypeDefinition()))))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public HandlersShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldAbstractClassData : TheoryData<Type>
        {
            public HandlersShouldAbstractClassData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    Add(type);
                }
            }
        }
    }
}

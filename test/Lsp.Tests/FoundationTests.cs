using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Xunit;

namespace Lsp.Tests
{
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

        [Theory(DisplayName = "Handler interfaces should have a abstract class")]
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

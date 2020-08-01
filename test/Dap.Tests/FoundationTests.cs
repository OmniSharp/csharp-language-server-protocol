using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using Xunit;
using Xunit.Abstractions;

namespace Dap.Tests
{
    public class FoundationTests
    {
        private readonly ILogger _logger;

        public FoundationTests(ITestOutputHelper outputHelper)
        {
            this._logger = new TestLoggerFactory(outputHelper).CreateLogger(typeof(FoundationTests));
        }

        [Theory(DisplayName = "Params types should have a method attribute")]
        [ClassData(typeof(ParamsShouldHaveMethodAttributeData))]
        public void ParamsShouldHaveMethodAttribute(Type type)
        {
            MethodAttribute.AllFrom(type).Any(z => z.Direction != Direction.Unspecified).Should()
                .Be(true, $"{type.Name} is missing a method attribute or the direction is not specified");
        }

        [Theory(DisplayName = "Handler interfaces should have a method attribute")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldHaveMethodAttribute(Type type)
        {
            MethodAttribute.AllFrom(type).Any(z => z.Direction != Direction.Unspecified).Should()
                .Be(true, $"{type.Name} is missing a method attribute or the direction is not specified");
        }

        [Theory(DisplayName = "Handler method should match params method")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldMatchParamsMethodAttribute(Type type)
        {
            if (typeof(IJsonRpcNotificationHandler).IsAssignableFrom(type)) return;
            var paramsType = HandlerTypeDescriptorHelper.GetHandlerInterface(type).GetGenericArguments()[0];

            var lhs = MethodAttribute.From(type);
            var rhs = MethodAttribute.From(paramsType);
            lhs.Method.Should().Be(rhs.Method, $"{type.FullName} method does not match {paramsType.FullName}");
            lhs.Direction.Should().Be(rhs.Direction, $"{type.FullName} direction does not match {paramsType.FullName}");
        }

        [Theory(DisplayName = "Handler interfaces should have a abstract class")]
        [ClassData(typeof(TypeHandlerData))]
        public void HandlersShouldAbstractClass(IHandlerTypeDescriptor descriptor)
        {
            _logger.LogInformation("Handler: {Type}", descriptor.HandlerType);
            // This test requires a refactor, the delegating handlers have been removed and replaced by shared implementations
            // TODO:
            // * Check for extension methods
            // * Check for IPartialItem(s)<> extension methods
            // * Check that the extension method calls `AddHandler` using the correct eventname
            // * check extension method name
            // * Also update events to have a nicer fire and forget abstract class
            // * Ensure all notifications have an action and task returning function
            var abstractHandler = descriptor.HandlerType.Assembly.ExportedTypes.FirstOrDefault(z => z.IsAbstract && z.IsClass && descriptor.HandlerType.IsAssignableFrom(z));
            abstractHandler.Should().NotBeNull($"{descriptor.HandlerType.FullName} is missing abstract base class");

            var delegatingHandler = descriptor.HandlerType.Assembly.DefinedTypes.FirstOrDefault(z => abstractHandler.IsAssignableFrom(z) && abstractHandler != z);
            if (delegatingHandler != null)
            {
                _logger.LogInformation("Delegating Handler: {Type}", delegatingHandler);
                delegatingHandler.DeclaringType.Should().NotBeNull();
                delegatingHandler.DeclaringType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(z => z.Name.StartsWith("On")).Should()
                    .BeTrue($"{descriptor.HandlerType.FullName} is missing delegating extension method");
            }
        }


        [Theory(DisplayName = "Handler extension method classes with appropriately named methods")]
        [ClassData(typeof(TypeHandlerExtensionData))]
        public void HandlersShouldExtensionMethodClassWithMethods(IHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
            Type extensionClass, string extensionClassName)
        {
            // This test requires a refactor, the delegating handlers have been removed and replaced by shared implementations
            // TODO:
            // * Check for IPartialItem(s)<> extension methods
            // * Also update events to have a nicer fire and forget abstract class

            _logger.LogInformation("Handler: {Type} {Extension} {ExtensionName} {OnMethod} {SendMethod}", descriptor.HandlerType,
                extensionClass, extensionClassName, onMethodName, sendMethodName);

            extensionClass.Should().NotBeNull($"{descriptor.HandlerType.FullName} is missing extension method class");
            extensionClass.GetMethods().Any(z => z.Name == onMethodName && typeof(IJsonRpcHandlerRegistry).IsAssignableFrom(z.GetParameters()[0].ParameterType)).Should()
                .BeTrue($"{descriptor.HandlerType.FullName} is missing event extension methods named {onMethodName}");
            extensionClass.GetMethods().Any(z => z.Name == sendMethodName && typeof(IResponseRouter).IsAssignableFrom(z.GetParameters()[0].ParameterType)).Should()
                .BeTrue($"{descriptor.HandlerType.FullName} is missing execute extension methods named {sendMethodName}");

            var registries = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(z => z.Name == onMethodName || z.Name == sendMethodName)
                .Select(z => z.GetParameters()[0].ParameterType)
                .Distinct()
                .ToHashSet();

            registries.Should().HaveCount(descriptor.Direction == Direction.Bidirectional ? 4 : 2,
                $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event");
        }

        [Theory(DisplayName = "Handler all expected extensions methods based on method direction")]
        [ClassData(typeof(TypeHandlerExtensionData))]
        public void HandlersShouldHaveExpectedExtensionMethodsBasedOnDirection(IHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
            Type extensionClass, string extensionClassName)
        {
            _logger.LogInformation("Handler: {Type} {Extension} {ExtensionName} {OnMethod} {SendMethod}", descriptor.HandlerType,
                extensionClass, extensionClassName, onMethodName, sendMethodName);

            var onMethodRegistries = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(z => z.Name == onMethodName)
                .Select(z => z.GetParameters()[0].ParameterType)
                .Distinct()
                .ToHashSet();

            var sendMethodRegistries = extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(z => z.Name == sendMethodName)
                .Select(z => z.GetParameters()[0].ParameterType)
                .Distinct()
                .ToHashSet();

            var expectedEventRegistries = descriptor.Direction switch {
                Direction.ClientToServer => new (string type, Func<ParameterInfo, bool> matcher)[] {("Server", info => info.ParameterType.Name.EndsWith("ServerRegistry"))},
                Direction.ServerToClient => new (string type, Func<ParameterInfo, bool> matcher)[] {("Client", info => info.ParameterType.Name.EndsWith("ClientRegistry"))},
                Direction.Bidirectional => new (string type, Func<ParameterInfo, bool> matcher)[]
                    {("Server", info => info.ParameterType.Name.EndsWith("ServerRegistry")), ("Client", info => info.ParameterType.Name.EndsWith("ClientRegistry"))},
                _ => throw new NotImplementedException(descriptor.HandlerType.FullName)
            };

            var expectedRequestHandlers = descriptor.Direction switch {
                Direction.ClientToServer => new (string type, Func<ParameterInfo, bool> matcher)[] {("Server", info => info.ParameterType.Name.EndsWith("Client"))},
                Direction.ServerToClient => new (string type, Func<ParameterInfo, bool> matcher)[] {("Client", info => info.ParameterType.Name.EndsWith("Server"))},
                Direction.Bidirectional => new (string type, Func<ParameterInfo, bool> matcher)[]
                    {("Server", info => info.ParameterType.Name.EndsWith("Client")), ("Client", info => info.ParameterType.Name.EndsWith("Server"))},
                _ => throw new NotImplementedException(descriptor.HandlerType.FullName)
            };

            foreach (var item in expectedEventRegistries)
            {
                extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(z => z.Name == onMethodName)
                    .Where(z => item.matcher(z.GetParameters()[0]))
                    .Should().HaveCountGreaterOrEqualTo(1, $"{descriptor.HandlerType.FullName} is missing a registry implementation for {item.type}");
            }

            foreach (var item in expectedRequestHandlers)
            {
                extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(z => z.Name == sendMethodName)
                    .Where(z => item.matcher(z.GetParameters()[0]))
                    .Should().HaveCountGreaterOrEqualTo(1, $"{descriptor.HandlerType.FullName} is missing a request implementation for {item.type}");
            }

            {
                var matcher = new MethodMatcher(onMethodRegistries, descriptor, extensionClass, onMethodName);

                Func<MethodInfo, bool> ForParameter(int index, Func<ParameterInfo, bool> m)
                {
                    return (info) => m(info.GetParameters()[index]);
                }

                var containsCancellationToken = ForParameter(1, info => info.ParameterType.GetGenericArguments().Reverse().Take(2).Any(x => x == typeof(CancellationToken)));
                var returnType = descriptor.HasResponseType ? typeof(Task<>).MakeGenericType(descriptor.ResponseType) : typeof(Task);
                var returns = ForParameter(1, info => info.ParameterType.GetGenericArguments().LastOrDefault() == returnType);
                var isAction = ForParameter(1, info => info.ParameterType.Name.StartsWith(nameof(Action)));
                var isFunc = ForParameter(1, info => info.ParameterType.Name.StartsWith("Func"));
                var takesParameter = ForParameter(1, info => info.ParameterType.GetGenericArguments().FirstOrDefault() == descriptor.ParamsType);

                if (descriptor.IsRequest)
                {
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, {returnType.Name}>", isFunc, takesParameter, returns);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                }

                if (descriptor.IsNotification)
                {
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, {returnType.Name}>", isFunc, takesParameter, returns);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                    matcher.Match($"Action<{descriptor.ParamsType.Name}>", isAction, takesParameter);
                    matcher.Match($"Action<{descriptor.ParamsType.Name}, CancellationToken>", isAction, takesParameter, containsCancellationToken);
                }
            }
            {
                var matcher = new MethodMatcher(sendMethodRegistries, descriptor, extensionClass, sendMethodName);
                Func<MethodInfo, bool> containsCancellationToken = info => info.GetParameters().Reverse().Take(2).Any(x => x.ParameterType == typeof(CancellationToken));
                var returnType = descriptor.HasResponseType ? typeof(Task<>).MakeGenericType(descriptor.ResponseType) : typeof(Task);
                Func<MethodInfo, bool> returns = info => info.ReturnType == returnType;
                Func<MethodInfo, bool> isAction = info => info.ReturnType.Name == "Void";
                var isFunc = returns;
                Func<MethodInfo, bool> takesParameter = info => info.GetParameters().Skip(1).Any(z => z.ParameterType == descriptor.ParamsType);

                if (descriptor.IsRequest)
                {
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                }

                if (descriptor.IsNotification)
                {
                    matcher.Match($"Action<{descriptor.ParamsType.Name}>", isAction, takesParameter);
                }
            }
        }

        class MethodMatcher
        {
            private readonly IEnumerable<Type> _registries;
            private readonly IHandlerTypeDescriptor _descriptor;
            private readonly Type _extensionClass;
            private readonly string _methodName;

            public MethodMatcher(IEnumerable<Type> registries,
                IHandlerTypeDescriptor descriptor, Type extensionClass, string methodName)
            {
                _registries = registries;
                _descriptor = descriptor;
                _extensionClass = extensionClass;
                _methodName = methodName;
            }

            public void Match(string description, params Func<MethodInfo, bool>[] matchers)
            {
                foreach (var registry in _registries)
                {
                    var methods = _extensionClass.GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(z => z.Name == _methodName)
                        .Where(z => matchers.All(matcher => matcher(z)))
                        .ToHashSet();
                    methods.Count.Should().BeGreaterThan(0,
                        $"{_descriptor.HandlerType.FullName} missing extension with parameter type {description} method for {registry.FullName}");

                    foreach (var method in methods)
                    {
                        if (method.Name == GetOnMethodName(_descriptor))
                        {
                            var registrySub = Substitute.For(new Type[] {method.GetParameters()[0].ParameterType},
                                Array.Empty<object>());

                            method.Invoke(null,
                                new[] {
                                        registrySub, Substitute.For(new Type[] {method.GetParameters()[1].ParameterType}, Array.Empty<object>()),
                                    }.Concat(method.GetParameters().Skip(2).Select(z =>
                                        !z.ParameterType.IsGenericType
                                            ? Activator.CreateInstance(z.ParameterType)
                                            : Substitute.For(new Type[] {z.ParameterType}, Array.Empty<object>()))
                                    )
                                    .ToArray());

                            registrySub.Received().ReceivedCalls()
                                .Any(z => z.GetMethodInfo().Name == nameof(IJsonRpcHandlerRegistry<IJsonRpcServerRegistry>.AddHandler) && z.GetArguments().Length == 3 &&
                                          z.GetArguments()[0].Equals(_descriptor.Method)).Should()
                                .BeTrue($"{_descriptor.HandlerType.Name} {description} should have the correct method.");
                        }

                        if (_descriptor.IsRequest && method.Name == GetSendMethodName(_descriptor))
                        {
                            method.GetParameters().Last().ParameterType.Should().Be(typeof(CancellationToken),
                                $"{_descriptor.HandlerType.Name} {description} send method must have optional cancellation token");
                            method.GetParameters().Last().IsOptional.Should().BeTrue(
                                $"{_descriptor.HandlerType.Name} {description} send method must have optional cancellation token");
                        }
                    }
                }
            }
        }

        public class ParamsShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public ParamsShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(IDataBreakpointInfoHandler).Assembly.ExportedTypes
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
                foreach (var type in typeof(IDataBreakpointInfoHandler).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z)))
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    Add(type);
                }
            }
        }

        public class HandlersShouldAbstractClassData : TheoryData<Type>
        {
            public HandlersShouldAbstractClassData()
            {
                foreach (var type in typeof(IDataBreakpointInfoHandler).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z)))
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    Add(type);
                }
            }
        }


        private static readonly Type[] HandlerTypes = {
            typeof(IJsonRpcNotificationHandler), typeof(IJsonRpcNotificationHandler<>),
            typeof(IJsonRpcRequestHandler<>), typeof(IJsonRpcRequestHandler<,>),
        };

        private static bool IsValidInterface(Type type)
        {
            if (type.GetTypeInfo().IsGenericType)
            {
                return HandlerTypes.Contains(type.GetGenericTypeDefinition());
            }

            return HandlerTypes.Contains(type);
        }

        public static Type GetHandlerInterface(Type type)
        {
            if (IsValidInterface(type)) return type;
            return type?.GetTypeInfo()
                .ImplementedInterfaces
                .First(IsValidInterface);
        }


        public class TypeHandlerData : TheoryData<IHandlerTypeDescriptor>
        {
            public TypeHandlerData()
            {
                foreach (var type in typeof(CompletionsArguments).Assembly.ExportedTypes.Where(
                    z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z)))
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    if (type == typeof(IProgressStartHandler) || type == typeof(IProgressUpdateHandler) || type == typeof(IProgressEndHandler)) continue;

                    Add(HandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(type));
                }
            }
        }

        public class TypeHandlerExtensionData : TheoryData<IHandlerTypeDescriptor, string, string, Type, string>
        {
            public TypeHandlerExtensionData()
            {
                foreach (var type in typeof(CompletionsArguments).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z)))
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    if (type == typeof(IProgressStartHandler) || type == typeof(IProgressUpdateHandler) || type == typeof(IProgressEndHandler)) continue;
                    var descriptor = HandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(type);

                    Add(
                        descriptor,
                        GetOnMethodName(descriptor),
                        GetSendMethodName(descriptor),
                        GetExtensionClass(descriptor),
                        GetExtensionClassName(descriptor).Substring(GetExtensionClassName(descriptor).LastIndexOf('.') + 1)
                    );
                }
            }
        }

        private static string GetExtensionClassName(IHandlerTypeDescriptor descriptor)
        {
            return SpecialCasedHandlerName(descriptor) + "Extensions";
            ;
        }

        private static string SpecialCasedHandlerName(IHandlerTypeDescriptor descriptor)
        {
            return new Regex(@"(\w+(?:\`\d)?)$")
                    .Replace(descriptor.HandlerType.Name ?? string.Empty,
                        descriptor.HandlerType.Name.Substring(1, descriptor.HandlerType.Name.IndexOf("Handler", StringComparison.Ordinal) - 1))
                ;
        }

        private static Type GetExtensionClass(IHandlerTypeDescriptor descriptor)
        {
            var name = GetExtensionClassName(descriptor);
            return descriptor.HandlerType.Assembly.GetExportedTypes()
                .FirstOrDefault(z => z.IsClass && z.IsAbstract && (z.Name == name || z.Name == name + "Base"));
        }

        private static string GetOnMethodName(IHandlerTypeDescriptor descriptor)
        {
            return "On" + SpecialCasedHandlerName(descriptor);
        }

        private static string GetSendMethodName(IHandlerTypeDescriptor descriptor)
        {
            var name = SpecialCasedHandlerName(descriptor);
            if (name.StartsWith("Run")
                // TODO: Change this next breaking change
                // || name.StartsWith("Set")
                // || name.StartsWith("Attach")
                // || name.StartsWith("Read")
            )
            {
                return name;
            }

            return descriptor.IsNotification ? "Send" + name : "Request" + name;
        }
    }
}

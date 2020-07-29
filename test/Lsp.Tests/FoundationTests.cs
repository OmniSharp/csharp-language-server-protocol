using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Extensions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class FoundationTests
    {
        private readonly ILogger _logger;

        public FoundationTests(ITestOutputHelper outputHelper)
        {
            _logger = new TestLoggerFactory(outputHelper).CreateLogger(typeof(FoundationTests));
        }

        [Theory(DisplayName = "Should not throw when accessing the debugger properties")]
        [ClassData(typeof(DebuggerDisplayTypes))]
        public void Debugger_Display_Should_Not_Throw(Type type)
        {
            var instance = Activator.CreateInstance(type);
            var property = type.GetProperty("DebuggerDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
            Func<string> a1 = () => property.GetValue(instance) as string;
            Func<string> a2 = () => instance.ToString();

            a1.Should().NotThrow().And.NotBeNull();
            a2.Should().NotThrow().And.NotBeNull();
        }

        class DebuggerDisplayTypes : TheoryData<Type>
        {
            public DebuggerDisplayTypes()
            {
                foreach (var item in typeof(DocumentSymbol).Assembly.ExportedTypes
                    .Where(z => z.GetCustomAttributes<DebuggerDisplayAttribute>().Any(z => z.Value.StartsWith("{DebuggerDisplay")))
                    .Where(z => z.GetConstructors().Any(z => z.GetParameters().Length == 0))
                )
                {
                    Add(item);
                }
            }
        }

        [Theory(DisplayName = "Params types should have a method attribute")]
        [ClassData(typeof(ParamsShouldHaveMethodAttributeData))]
        public void ParamsShouldHaveMethodAttribute(Type type)
        {
            type.GetCustomAttributes<MethodAttribute>().Any(z => z.Direction != Direction.Unspecified).Should()
                .Be(true, $"{type.Name} is missing a method attribute or the direction is not specified");
        }

        [Theory(DisplayName = "Handler interfaces should have a method attribute")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldHaveMethodAttribute(Type type)
        {
            type.GetCustomAttributes<MethodAttribute>().Any(z => z.Direction != Direction.Unspecified).Should()
                .Be(true, $"{type.Name} is missing a method attribute or the direction is not specified");
        }

        [Theory(DisplayName = "Handler method should match params method")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldMatchParamsMethodAttribute(Type type)
        {
            if (typeof(IJsonRpcNotificationHandler).IsAssignableFrom(type)) return;
            var paramsType = HandlerTypeDescriptorHelper.GetHandlerInterface(type).GetGenericArguments()[0];

            var lhs = type.GetCustomAttribute<MethodAttribute>(true);
            var rhs = paramsType.GetCustomAttribute<MethodAttribute>(true);
            lhs.Method.Should().Be(rhs.Method, $"{type.FullName} method does not match {paramsType.FullName}");
            lhs.Direction.Should().Be(rhs.Direction, $"{type.FullName} direction does not match {paramsType.FullName}");
        }

        [Theory(DisplayName = "Handler interfaces should have a abstract class")]
        [ClassData(typeof(TypeHandlerData))]
        public void HandlersShouldAbstractClass(ILspHandlerTypeDescriptor descriptor)
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
        public void HandlersShouldExtensionMethodClassWithMethods(ILspHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
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

            if (descriptor.Direction == Direction.Bidirectional)
            {
                registries
                    .Where(z => typeof(IClientProxy).IsAssignableFrom(z) || typeof(ILanguageServerRegistry).IsAssignableFrom(z))
                    .Should().HaveCountGreaterOrEqualTo(1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event");
                registries
                    .Where(z => typeof(IServerProxy).IsAssignableFrom(z) || typeof(ILanguageClientRegistry).IsAssignableFrom(z))
                    .Should().HaveCountGreaterOrEqualTo(1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event");
            }
            else if (descriptor.Direction == Direction.ServerToClient)
            {
                registries
                    .Where(z => typeof(IServerProxy).IsAssignableFrom(z) || typeof(ILanguageClientRegistry).IsAssignableFrom(z))
                    .Should().HaveCountGreaterOrEqualTo(1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event");
                registries
                    .Where(z => typeof(IClientProxy).IsAssignableFrom(z) || typeof(ILanguageServerRegistry).IsAssignableFrom(z))
                    .Should().HaveCount(0, $"{descriptor.HandlerType.FullName} must not cross the streams or be made bidirectional");
            }
            else if (descriptor.Direction == Direction.ClientToServer)
            {
                registries
                    .Where(z => typeof(IClientProxy).IsAssignableFrom(z) || typeof(ILanguageServerRegistry).IsAssignableFrom(z))
                    .Should().HaveCountGreaterOrEqualTo(1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event");
                registries
                    .Where(z => typeof(IServerProxy).IsAssignableFrom(z) || typeof(ILanguageClientRegistry).IsAssignableFrom(z))
                    .Should().HaveCount(0, $"{descriptor.HandlerType.FullName} must not cross the streams or be made bidirectional");
            }
        }

        [Theory(DisplayName = "Handler all expected extensions methods based on method direction")]
        [ClassData(typeof(TypeHandlerExtensionData))]
        public void HandlersShouldHaveExpectedExtensionMethodsBasedOnDirection(ILspHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
            Type extensionClass, string extensionClassName)
        {
            _logger.LogInformation("Handler: {Type} {Extension} {ExtensionName} {OnMethod} {SendMethod}", descriptor.HandlerType,
                extensionClass, extensionClassName, onMethodName, sendMethodName);

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
        }

        [Theory(DisplayName = "Extension methods handle partial results for partial items")]
        [ClassData(typeof(TypeHandlerExtensionData))]
        public void HandlersShouldHaveActionsForBothCompleteAndPartialResults(ILspHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
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

            {
                var matcher = new MethodMatcher(onMethodRegistries, descriptor, extensionClass);

                Func<MethodInfo, bool> ForAnyParameter(Func<ParameterInfo, bool> m)
                {
                    return (info) => info.GetParameters().Any(m);
                }

                Func<MethodInfo, bool> ForParameter(int index, Func<ParameterInfo, bool> m)
                {
                    return (info) => m(info.GetParameters()[index]);
                }

                var containsCancellationToken = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Reverse().Take(2).Any(x => x == typeof(CancellationToken)));
                var returnType = descriptor.HasResponseType ? typeof(Task<>).MakeGenericType(descriptor.ResponseType) : typeof(Task);
                var returns = ForAnyParameter(info => info.ParameterType.GetGenericArguments().LastOrDefault() == returnType);
                var isAction = ForAnyParameter(info => info.ParameterType.Name.StartsWith(nameof(Action)));
                var isFunc = ForAnyParameter(info => info.ParameterType.Name.StartsWith("Func"));
                var takesParameter = ForAnyParameter(info => info.ParameterType.GetGenericArguments().FirstOrDefault() == descriptor.ParamsType);
                var takesCapability = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Skip(1).FirstOrDefault() == descriptor.CapabilityType);
                var returnsTask = ForAnyParameter(info => info.ParameterType.GetGenericArguments().LastOrDefault() == typeof(Task));

                if (descriptor.IsRequest && TypeHandlerExtensionData.HandlersToSkip.All(z => descriptor.HandlerType != z))
                {
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, {returnType.Name}>", isFunc, takesParameter, returns);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                    if (descriptor.HasCapability)
                    {
                        matcher.Match($"Func<{descriptor.ParamsType.Name}, {descriptor.CapabilityType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter,
                            takesCapability, containsCancellationToken, returns);
                    }

                    if (descriptor.HasPartialItem)
                    {
                        var capability = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Skip(2).FirstOrDefault() == descriptor.CapabilityType);
                        var observesPartialResultType = ForAnyParameter(info =>
                            info.ParameterType.GetGenericArguments().Skip(1).FirstOrDefault() == typeof(IObserver<>).MakeGenericType(descriptor.PartialItemType));

                        matcher.Match($"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>>", isAction, takesParameter, observesPartialResultType);
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, Task>", isFunc, takesParameter, observesPartialResultType, returnsTask);
                        matcher.Match($"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, CancellationToken>", isAction, takesParameter,
                            containsCancellationToken, observesPartialResultType);
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, CancellationToken, Task>", isFunc, takesParameter,
                        //     containsCancellationToken, observesPartialResultType, returnsTask);
                        if (descriptor.HasCapability)
                        {
                            matcher.Match(
                                $"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, {descriptor.CapabilityType.Name}, CancellationToken>",
                                isAction,
                                takesParameter,
                                capability, containsCancellationToken, observesPartialResultType);
                            // matcher.Match(
                            //     $"Func<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, {descriptor.CapabilityType.Name}, CancellationToken, Task>",
                            //     isFunc,
                            //     takesParameter,
                            //     capability, containsCancellationToken, observesPartialResultType, returnsTask);
                        }
                    }

                    if (descriptor.HasPartialItems)
                    {
                        var capability = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Skip(2).FirstOrDefault() == descriptor.CapabilityType);
                        var observesPartialResultType = ForAnyParameter(info =>
                            info.ParameterType.GetGenericArguments().Skip(1).FirstOrDefault() ==
                            typeof(IObserver<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(descriptor.PartialItemsType)));

                        matcher.Match($"Action<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>>", isAction, takesParameter,
                            observesPartialResultType);
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>, Task>", isFunc, takesParameter,
                        //     observesPartialResultType, returnsTask);
                        matcher.Match($"Action<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>, CancellationToken>", isAction,
                            takesParameter,
                            containsCancellationToken, observesPartialResultType);
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>, CancellationToken, Task>", isFunc,
                        //     takesParameter,
                        //     containsCancellationToken, observesPartialResultType, returnsTask);
                        if (descriptor.HasCapability)
                        {
                            matcher.Match(
                                $"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemsType.Name}>, {descriptor.CapabilityType.Name}, CancellationToken>",
                                isAction,
                                takesParameter,
                                capability, containsCancellationToken, observesPartialResultType);
                            // matcher.Match(
                            //     $"Func<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemsType.Name}>, {descriptor.CapabilityType.Name}, CancellationToken, Task>",
                            //     isFunc,
                            //     takesParameter,
                            //     capability, containsCancellationToken, observesPartialResultType, returnsTask);
                        }
                    }
                }

                if (descriptor.IsNotification)
                {
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, {returnType.Name}>", isFunc, takesParameter, returns);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                    matcher.Match($"Action<{descriptor.ParamsType.Name}>", isAction, takesParameter);
                    matcher.Match($"Action<{descriptor.ParamsType.Name}, CancellationToken>", isAction, takesParameter, containsCancellationToken);
                    if (descriptor.HasCapability)
                    {
                        matcher.Match($"Func<{descriptor.ParamsType.Name}, {descriptor.CapabilityType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter,
                            takesCapability, containsCancellationToken, returns);
                        matcher.Match($"Action<{descriptor.ParamsType.Name}, {descriptor.CapabilityType.Name}, CancellationToken>", isAction, takesParameter, takesCapability,
                            containsCancellationToken);
                    }
                }
            }
            {
                var matcher = new MethodMatcher(sendMethodRegistries, descriptor, extensionClass);
                Func<MethodInfo, bool> containsCancellationToken = info => info.GetParameters().Reverse().Take(2).Any(x => x.ParameterType == typeof(CancellationToken));
                var returnType = descriptor.HasResponseType ? typeof(Task<>).MakeGenericType(descriptor.ResponseType) : typeof(Task);
                Func<MethodInfo, bool> returns = info => info.ReturnType == returnType;
                Func<MethodInfo, bool> isAction = info => info.ReturnType.Name == "Void";
                Func<MethodInfo, bool> takesParameter = info => info.GetParameters().Skip(1).Any(z => z.ParameterType == descriptor.ParamsType);

                if (descriptor.IsRequest && descriptor.HasPartialItems)
                {
                    Func<MethodInfo, bool> partialReturnType = info =>
                        typeof(IRequestProgressObservable<,>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(descriptor.PartialItemsType), descriptor.ResponseType)
                            .IsAssignableFrom(info.ReturnType);
                    matcher.Match(
                        $"Func<{descriptor.ParamsType.Name}, CancellationToken, IProgressObservable<IEnumerable<{descriptor.PartialItemsType.Name}>, {descriptor.ResponseType.Name}>>",
                        takesParameter, containsCancellationToken, partialReturnType);
                }
                else if (descriptor.IsRequest && descriptor.HasPartialItem)
                {
                    Func<MethodInfo, bool> partialReturnType = info =>
                        typeof(IRequestProgressObservable<,>).MakeGenericType(descriptor.PartialItemType, descriptor.ResponseType).IsAssignableFrom(info.ReturnType);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, IProgressObservable<{descriptor.PartialItemType.Name}, {descriptor.ResponseType.Name}>>",
                        takesParameter, containsCancellationToken, partialReturnType);
                }
                else if (descriptor.IsRequest)
                {
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", takesParameter, containsCancellationToken, returns);
                }
                else if (descriptor.IsNotification)
                {
                    matcher.Match($"Action<{descriptor.ParamsType.Name}>", isAction, takesParameter);
                }
            }
        }


        class MethodMatcher
        {
            private readonly IEnumerable<Type> _registries;
            private readonly ILspHandlerTypeDescriptor _descriptor;
            private readonly Type _extensionClass;
            private readonly string _methodName;

            public MethodMatcher(IEnumerable<Type> registries,
                ILspHandlerTypeDescriptor descriptor, Type extensionClass, string methodName = null)
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
                        .Where(z => _methodName == null || z.Name == _methodName)
                        .Where(z => matchers.All(matcher => matcher(z)))
                        .ToHashSet();
                    methods.Count.Should().BeGreaterThan(0,
                        $"{_descriptor.HandlerType.FullName} missing extension with parameter type {description} method for {registry.FullName}");

                    foreach (var method in methods)
                    {
                        if (method.Name == GetOnMethodName(_descriptor))
                        {
                            var registrySub = Substitute.For(new Type[] {method.GetParameters()[0].ParameterType}, Array.Empty<object>());
                            SubstitutionContext.Current.GetCallRouterFor(registrySub).SetReturnForType(
                                method.GetParameters()[0].ParameterType,
                                (IReturn) new ReturnValue(registrySub)
                            );
                            // registrySub.ReturnsForAll(x => registrySub);

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
                                .Any(z => z.GetMethodInfo().Name == nameof(IJsonRpcHandlerRegistry<IJsonRpcHandlerRegistry<IJsonRpcServerRegistry>>.AddHandler) &&
                                          z.GetArguments().Length == 3 &&
                                          z.GetArguments()[0].Equals(_descriptor.Method))
                                .Should().BeTrue($"{_descriptor.HandlerType.Name} {description} should have the correct method.");

                            if (_descriptor.HasRegistration && method.GetParameters().Length == 3)
                            {
                                method.GetParameters()[2].ParameterType.Should().Be(_descriptor.RegistrationType,
                                    $"{_descriptor.HandlerType.FullName} {description} is has incorrect registration type {method.GetParameters()[2].ParameterType.FullName}");
                                method.GetParameters()[2].IsOptional.Should()
                                    .BeFalse($"{_descriptor.HandlerType.FullName} {description} Registration types should not be optional");
                            }
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
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes.Where(z =>
                    z.IsClass && !z.IsAbstract && z.GetInterfaces().Any(z => z.IsGenericType && typeof(IRequest<>).IsAssignableFrom(z.GetGenericTypeDefinition()))))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public HandlersShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes.Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    Add(type);
                }
            }
        }

        public class TypeHandlerData : TheoryData<ILspHandlerTypeDescriptor>
        {
            public TypeHandlerData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes.Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    Add(LspHandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(type));
                }
            }
        }

        public class TypeHandlerExtensionData : TheoryData<ILspHandlerTypeDescriptor, string, string, Type, string>
        {
            public static Type[] HandlersToSkip = new[] {
                typeof(ISemanticTokensHandler),
                typeof(ISemanticTokensDeltaHandler),
                typeof(ISemanticTokensRangeHandler),
            };

            public TypeHandlerExtensionData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                    .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z) && !z.IsGenericType)
                    .Except(new[] {typeof(ITextDocumentSyncHandler)}))
                {
                    if (type == typeof(ICompletionResolveHandler) || type == typeof(ICodeLensResolveHandler) || type == typeof(IDocumentLinkResolveHandler)) continue;
                    var descriptor = LspHandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(type);

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

        private static string GetExtensionClassName(ILspHandlerTypeDescriptor descriptor)
        {
            return SpecialCasedHandlerFullName(descriptor) + "Extensions";
            ;
        }

        private static string SpecialCasedHandlerFullName(ILspHandlerTypeDescriptor descriptor)
        {
            return new Regex(@"(\w+)$")
                    .Replace(descriptor.HandlerType.FullName ?? string.Empty,
                        descriptor.HandlerType.Name.Substring(1, descriptor.HandlerType.Name.IndexOf("Handler", StringComparison.Ordinal) - 1))
                    .Replace("SemanticTokensEdits", "SemanticTokens")
                    .Replace("SemanticTokensDelta", "SemanticTokens")
                    .Replace("SemanticTokensRange", "SemanticTokens")
                ;
        }

        private static string HandlerName(ILspHandlerTypeDescriptor descriptor)
        {
            var name = HandlerFullName(descriptor);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private static string HandlerFullName(ILspHandlerTypeDescriptor descriptor)
        {
            return new Regex(@"(\w+)$")
                .Replace(descriptor.HandlerType.FullName ?? string.Empty,
                    descriptor.HandlerType.Name.Substring(1, descriptor.HandlerType.Name.IndexOf("Handler", StringComparison.Ordinal) - 1));
        }

        public static string SpecialCasedHandlerName(ILspHandlerTypeDescriptor descriptor)
        {
            var name = SpecialCasedHandlerFullName(descriptor);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private static Type GetExtensionClass(ILspHandlerTypeDescriptor descriptor)
        {
            var name = GetExtensionClassName(descriptor);
            return descriptor.HandlerType.Assembly.GetExportedTypes()
                .FirstOrDefault(z => z.IsClass && z.IsAbstract && (z.FullName == name || z.FullName == name+"Base"));
        }

        private static string GetOnMethodName(ILspHandlerTypeDescriptor descriptor)
        {
            return "On" + SpecialCasedHandlerName(descriptor);
        }

        private static string GetSendMethodName(ILspHandlerTypeDescriptor descriptor)
        {
            var name = SpecialCasedHandlerName(descriptor);
            if (name.StartsWith("Did")
                || name.StartsWith("Log")
                || name.StartsWith("Show")
                || name.StartsWith("Register")
                || name.StartsWith("Prepare")
                || name.StartsWith("Publish")
                || name.StartsWith("ApplyWorkspaceEdit")
                || name.StartsWith("Unregister"))
            {
                return name;
            }

            if (name.EndsWith("Resolve", StringComparison.Ordinal))
            {
                return "Resolve" + name.Substring(0, name.IndexOf("Resolve", StringComparison.Ordinal));
            }

            return descriptor.IsNotification ? "Send" + name : "Request" + name;
        }
    }
}

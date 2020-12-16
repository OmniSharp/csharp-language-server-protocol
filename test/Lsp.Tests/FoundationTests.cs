using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class FoundationTests
    {
        private readonly ILogger _logger;

        public FoundationTests(ITestOutputHelper outputHelper) => _logger = new TestLoggerFactory(outputHelper).CreateLogger(typeof(FoundationTests));

        [Theory]
        [ClassData(typeof(ActionDelegateData))]
        public void All_Create_Methods_Should_Work(ActionDelegate createDelegate) => createDelegate.Method.Should().NotThrow();

        public class ActionDelegateData : TheoryData<ActionDelegate>
        {
            public ActionDelegateData()
            {
                {
                    var baseOptions = new LanguageServerOptions().WithPipe(new Pipe());

                    void BaseDelegate(LanguageServerOptions o)
                    {
                        o.WithPipe(new Pipe());
                    }

                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    Add(new ActionDelegate("create (server): options", () => LanguageServer.Create(baseOptions)));
                    Add(new ActionDelegate("create (server): options, serviceProvider", () => LanguageServer.Create(baseOptions, serviceProvider)));
                    Add(new ActionDelegate("create (server): action", () => LanguageServer.Create(BaseDelegate)));
                    Add(new ActionDelegate("create (server): action, serviceProvider", () => LanguageServer.Create(BaseDelegate, serviceProvider)));

                    Add(new ActionDelegate("from (server): options", () => LanguageServer.From(baseOptions)));
                    Add(new ActionDelegate("from (server): options, cancellationToken", () => LanguageServer.From(baseOptions, CancellationToken.None)));
                    Add(
                        new ActionDelegate(
                            "from (server): options, serviceProvider, cancellationToken", () => LanguageServer.From(baseOptions, serviceProvider, CancellationToken.None)
                        )
                    );
                    Add(new ActionDelegate("from (server): options, serviceProvider", () => LanguageServer.From(baseOptions, serviceProvider)));
                    Add(new ActionDelegate("from (server): action", () => LanguageServer.From(BaseDelegate)));
                    Add(new ActionDelegate("from (server): action, cancellationToken", () => LanguageServer.From(BaseDelegate, CancellationToken.None)));
                    Add(
                        new ActionDelegate(
                            "from (server): action, serviceProvider, cancellationToken", () => LanguageServer.From(BaseDelegate, serviceProvider, CancellationToken.None)
                        )
                    );
                    Add(new ActionDelegate("from (server): action, serviceProvider", () => LanguageServer.From(BaseDelegate, serviceProvider)));
                }
                {
                    var baseOptions = new LanguageClientOptions().WithPipe(new Pipe());

                    void BaseDelegate(LanguageClientOptions o)
                    {
                        o.WithPipe(new Pipe());
                    }

                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    Add(new ActionDelegate("create (client): options", () => LanguageClient.Create(baseOptions)));
                    Add(new ActionDelegate("create (client): options, serviceProvider", () => LanguageClient.Create(baseOptions, serviceProvider)));
                    Add(new ActionDelegate("create (client): action", () => LanguageClient.Create(BaseDelegate)));
                    Add(new ActionDelegate("create (client): action, serviceProvider", () => LanguageClient.Create(BaseDelegate, serviceProvider)));

                    Add(new ActionDelegate("from (client): options", () => LanguageClient.From(baseOptions)));
                    Add(new ActionDelegate("from (client): options, cancellationToken", () => LanguageClient.From(baseOptions, CancellationToken.None)));
                    Add(
                        new ActionDelegate(
                            "from (client): options, serviceProvider, cancellationToken", () => LanguageClient.From(baseOptions, serviceProvider, CancellationToken.None)
                        )
                    );
                    Add(new ActionDelegate("from (client): options, serviceProvider", () => LanguageClient.From(baseOptions, serviceProvider)));
                    Add(new ActionDelegate("from (client): action", () => LanguageClient.From(BaseDelegate)));
                    Add(new ActionDelegate("from (client): action, cancellationToken", () => LanguageClient.From(BaseDelegate, CancellationToken.None)));
                    Add(
                        new ActionDelegate(
                            "from (client): action, serviceProvider, cancellationToken", () => LanguageClient.From(BaseDelegate, serviceProvider, CancellationToken.None)
                        )
                    );
                    Add(new ActionDelegate("from (client): action, serviceProvider", () => LanguageClient.From(BaseDelegate, serviceProvider)));
                }
            }
        }

        public class ActionDelegate
        {
            private readonly string _name;
            public Action Method { get; }

            public ActionDelegate(string name, Action method)
            {
                _name = name;
                Method = method;
            }

            public override string ToString() => _name;
        }


        [Theory(DisplayName = "Should not throw when accessing the debugger properties")]
        [ClassData(typeof(DebuggerDisplayTypes))]
        public void Debugger_Display_Should_Not_Throw(Type type)
        {
            var instance = Activator.CreateInstance(type);
            var property = type.GetProperty("DebuggerDisplay", BindingFlags.NonPublic | BindingFlags.Instance)!;
            Func<string> a1 = () => ( property.GetValue(instance) as string )!;
            Func<string> a2 = () => instance!.ToString()!;

            a1.Should().NotThrow().And.NotBeNull();
            a2.Should().NotThrow().And.NotBeNull();
        }

        private class DebuggerDisplayTypes : TheoryData<Type>
        {
            public DebuggerDisplayTypes()
            {
                foreach (var item in typeof(DocumentSymbol).Assembly.ExportedTypes
                                                           .Where(z => !z.IsGenericTypeDefinition)
                                                           .Where(z => z.GetCustomAttributes<DebuggerDisplayAttribute>().Any(x => x.Value.StartsWith("{DebuggerDisplay")))
                                                           .Where(z => z.GetConstructors().Any(x => x.GetParameters().Length == 0))
                )
                {
                    Add(item);
                }
            }
        }

        [Theory(DisplayName = "Params types should have a method attribute")]
        [ClassData(typeof(ParamsShouldHaveMethodAttributeData))]
        public void ParamsShouldHaveMethodAttribute(Type type) =>
            MethodAttribute.AllFrom(type).Any(z => z.Direction != Direction.Unspecified).Should()
                           .Be(true, $"{type.Name} is missing a method attribute or the direction is not specified");

        [Theory(DisplayName = "Handler interfaces should have a method attribute")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldHaveMethodAttribute(Type type) =>
            MethodAttribute.AllFrom(type).Any(z => z.Direction != Direction.Unspecified).Should()
                           .Be(true, $"{type.Name} is missing a method attribute or the direction is not specified");

        [Theory(DisplayName = "Handler method should match params method")]
        [ClassData(typeof(HandlersShouldHaveMethodAttributeData))]
        public void HandlersShouldMatchParamsMethodAttribute(Type type)
        {
            var paramsType = HandlerTypeDescriptorHelper.GetHandlerInterface(type).GetGenericArguments()[0];

            var lhs = MethodAttribute.From(type)!;
            var rhs = MethodAttribute.From(paramsType)!;
            lhs.Method.Should().Be(rhs.Method, $"{type.FullName} method does not match {paramsType.FullName}");
            lhs.Direction.Should().Be(rhs.Direction, $"{type.FullName} direction does not match {paramsType.FullName}");
        }

        [Theory(DisplayName = "Registration Options and Static Options should have the same properties")]
        [ClassData(typeof(RegistrationConverters))]
        public void Registration_Converters_Should_Have_THe_Same_Properties(Type type)
        {
            var types = type.BaseType?.GetGenericArguments() ?? Array.Empty<Type>();
            var source = types[0].GetProperties().Select(z => z.Name);
            var destination = types[1].GetProperties().Select(z => z.Name).Except(new[] { "Id" });
            source.Should().Contain(destination);
        }

        [Theory(DisplayName = "Handler interfaces should have a abstract class")]
        [ClassData(typeof(TypeHandlerData))]
        public void HandlersShouldAbstractClass(ILspHandlerTypeDescriptor descriptor)
        {
            _logger.LogInformation("Handler: {Type}", descriptor.HandlerType);
            // This test requires a refactor, the delegating handlers have been removed and replaced by shared implementations
            var abstractHandler = descriptor.HandlerType.Assembly.ExportedTypes.FirstOrDefault(z => z.IsAbstract && z.IsClass && descriptor.HandlerType.IsAssignableFrom(z));
            abstractHandler.Should().NotBeNull($"{descriptor.HandlerType.FullName} is missing abstract base class");

            var delegatingHandler = descriptor.HandlerType.Assembly.DefinedTypes.FirstOrDefault(
                z =>
                    abstractHandler.IsAssignableFrom(z)
                 && abstractHandler != z
                 && !z.IsGenericTypeDefinition
            );
            if (delegatingHandler != null)
            {
                _logger.LogInformation("Delegating Handler: {Type}", delegatingHandler);
                delegatingHandler.DeclaringType.Should().NotBeNull();
                delegatingHandler.DeclaringType!.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(z => z.Name.StartsWith("On")).Should()
                                                .BeTrue($"{descriptor.HandlerType.FullName} is missing delegating extension method");
            }
        }

        [Theory(DisplayName = "Handler extension method classes with appropriately named methods")]
        [ClassData(typeof(TypeHandlerExtensionData))]
        public void HandlersShouldExtensionMethodClassWithMethods(
            ILspHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
            Type extensionClass, string extensionClassName
        )
        {
            // This test requires a refactor, the delegating handlers have been removed and replaced by shared implementations

            _logger.LogInformation(
                "Handler: {Type} {Extension} {ExtensionName} {OnMethod} {SendMethod}", descriptor.HandlerType,
                extensionClass, extensionClassName, onMethodName, sendMethodName
            );

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
                   .Where(z => typeof(ILanguageClientProxy).IsAssignableFrom(z) || typeof(ILanguageServerRegistry).IsAssignableFrom(z))
                   .Should().HaveCountGreaterOrEqualTo(
                        1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event"
                    );
                registries
                   .Where(z => typeof(ILanguageServerProxy).IsAssignableFrom(z) || typeof(ILanguageClientRegistry).IsAssignableFrom(z))
                   .Should().HaveCountGreaterOrEqualTo(
                        1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event"
                    );
            }
            else if (descriptor.Direction == Direction.ServerToClient)
            {
                registries
                   .Where(z => typeof(ILanguageServerProxy).IsAssignableFrom(z) || typeof(ILanguageClientRegistry).IsAssignableFrom(z))
                   .Should().HaveCountGreaterOrEqualTo(
                        1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event"
                    );
                registries
                   .Where(z => typeof(ILanguageClientProxy).IsAssignableFrom(z) || typeof(ILanguageServerRegistry).IsAssignableFrom(z))
                   .Should().HaveCount(0, $"{descriptor.HandlerType.FullName} must not cross the streams or be made bidirectional");
            }
            else if (descriptor.Direction == Direction.ClientToServer)
            {
                registries
                   .Where(z => typeof(ILanguageClientProxy).IsAssignableFrom(z) || typeof(ILanguageServerRegistry).IsAssignableFrom(z))
                   .Should().HaveCountGreaterOrEqualTo(
                        1,
                        $"{descriptor.HandlerType.FullName} there should be methods for both handing the event and sending the event"
                    );
                registries
                   .Where(z => typeof(ILanguageServerProxy).IsAssignableFrom(z) || typeof(ILanguageClientRegistry).IsAssignableFrom(z))
                   .Should().HaveCount(0, $"{descriptor.HandlerType.FullName} must not cross the streams or be made bidirectional");
            }
        }

        [Theory(DisplayName = "Handler all expected extensions methods based on method direction")]
        [ClassData(typeof(TypeHandlerExtensionData))]
        public void HandlersShouldHaveExpectedExtensionMethodsBasedOnDirection(
            ILspHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
            Type extensionClass, string extensionClassName
        )
        {
            _logger.LogInformation(
                "Handler: {Type} {Extension} {ExtensionName} {OnMethod} {SendMethod}", descriptor.HandlerType,
                extensionClass, extensionClassName, onMethodName, sendMethodName
            );

            var expectedEventRegistries = descriptor.Direction switch {
                Direction.ClientToServer => new (string type, Func<ParameterInfo, bool> matcher)[] { ( "Server", info => info.ParameterType.Name.EndsWith("ServerRegistry") ) },
                Direction.ServerToClient => new (string type, Func<ParameterInfo, bool> matcher)[] { ( "Client", info => info.ParameterType.Name.EndsWith("ClientRegistry") ) },
                Direction.Bidirectional => new (string type, Func<ParameterInfo, bool> matcher)[]
                    { ( "Server", info => info.ParameterType.Name.EndsWith("ServerRegistry") ), ( "Client", info => info.ParameterType.Name.EndsWith("ClientRegistry") ) },
                _ => throw new NotImplementedException(descriptor.HandlerType.FullName)
            };

            var expectedRequestHandlers = descriptor.Direction switch {
                Direction.ClientToServer => new (string type, Func<ParameterInfo, bool> matcher)[] { ( "Server", info => info.ParameterType.Name.EndsWith("Client") ) },
                Direction.ServerToClient => new (string type, Func<ParameterInfo, bool> matcher)[] { ( "Client", info => info.ParameterType.Name.EndsWith("Server") ) },
                Direction.Bidirectional => new (string type, Func<ParameterInfo, bool> matcher)[]
                    { ( "Server", info => info.ParameterType.Name.EndsWith("Client") ), ( "Client", info => info.ParameterType.Name.EndsWith("Server") ) },
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
        public void HandlersShouldHaveActionsForBothCompleteAndPartialResults(
            ILspHandlerTypeDescriptor descriptor, string onMethodName, string sendMethodName,
            Type extensionClass, string extensionClassName
        )
        {
            _logger.LogInformation(
                "Handler: {Type} {Extension} {ExtensionName} {OnMethod} {SendMethod}", descriptor.HandlerType,
                extensionClass, extensionClassName, onMethodName, sendMethodName
            );

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
                    return info => info.GetParameters().Any(m);
                }

                var containsCancellationToken = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Reverse().Take(2).Any(x => x == typeof(CancellationToken)));
                var returnType = descriptor.HasResponseType ? typeof(Task<>).MakeGenericType(descriptor.ResponseType!) : typeof(Task);
                var returns = ForAnyParameter(info => info.ParameterType.GetGenericArguments().LastOrDefault() == returnType);
                var isAction = ForAnyParameter(info => info.ParameterType.Name.StartsWith(nameof(Action)));
                var isFunc = ForAnyParameter(info => info.ParameterType.Name.StartsWith("Func"));
                var takesParameter = ForAnyParameter(info => info.ParameterType.GetGenericArguments().FirstOrDefault() == descriptor.ParamsType);
                var takesCapability = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Skip(1).FirstOrDefault() == descriptor.CapabilityType);

                if (descriptor.IsRequest && TypeHandlerExtensionData.HandlersToSkip.All(z => descriptor.HandlerType != z))
                {
                    matcher.Match($"Func<{descriptor.ParamsType!.Name}, {returnType.Name}>", isFunc, takesParameter, returns);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                    if (descriptor.HasCapability)
                    {
                        matcher.Match(
                            $"Func<{descriptor.ParamsType.Name}, {descriptor.CapabilityType!.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter,
                            takesCapability, containsCancellationToken, returns
                        );
                    }

                    if (descriptor.HasPartialItem)
                    {
                        var capability = ForAnyParameter(info => info.ParameterType.GetGenericArguments().Skip(2).FirstOrDefault() == descriptor.CapabilityType);
                        var observesPartialResultType = ForAnyParameter(
                            info =>
                                info.ParameterType.GetGenericArguments().Skip(1).FirstOrDefault() == typeof(IObserver<>).MakeGenericType(descriptor.PartialItemType!)
                        );

                        matcher.Match($"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType!.Name}>>", isAction, takesParameter, observesPartialResultType);
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, Task>", isFunc, takesParameter, observesPartialResultType, returnsTask);
                        matcher.Match(
                            $"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, CancellationToken>", isAction, takesParameter,
                            containsCancellationToken, observesPartialResultType
                        );
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, CancellationToken, Task>", isFunc, takesParameter,
                        //     containsCancellationToken, observesPartialResultType, returnsTask);
                        if (descriptor.HasCapability)
                        {
                            matcher.Match(
                                $"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemType.Name}>, {descriptor.CapabilityType!.Name}, CancellationToken>",
                                isAction,
                                takesParameter,
                                capability, containsCancellationToken, observesPartialResultType
                            );
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
                        var observesPartialResultType = ForAnyParameter(
                            info =>
                                info.ParameterType.GetGenericArguments().Skip(1).FirstOrDefault() ==
                                typeof(IObserver<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(descriptor.PartialItemsType!))
                        );

                        matcher.Match(
                            $"Action<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType!.Name}>>>", isAction, takesParameter,
                            observesPartialResultType
                        );
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>, Task>", isFunc, takesParameter,
                        //     observesPartialResultType, returnsTask);
                        matcher.Match(
                            $"Action<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>, CancellationToken>", isAction,
                            takesParameter,
                            containsCancellationToken, observesPartialResultType
                        );
                        // matcher.Match($"Func<{descriptor.ParamsType.Name}, IObserver<IEnumerable<{descriptor.PartialItemsType.Name}>>, CancellationToken, Task>", isFunc,
                        //     takesParameter,
                        //     containsCancellationToken, observesPartialResultType, returnsTask);
                        if (descriptor.HasCapability)
                        {
                            matcher.Match(
                                $"Action<{descriptor.ParamsType.Name}, IObserver<{descriptor.PartialItemsType.Name}>, {descriptor.CapabilityType!.Name}, CancellationToken>",
                                isAction,
                                takesParameter,
                                capability, containsCancellationToken, observesPartialResultType
                            );
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
                    matcher.Match($"Func<{descriptor.ParamsType!.Name}, {returnType.Name}>", isFunc, takesParameter, returns);
                    matcher.Match($"Func<{descriptor.ParamsType.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter, containsCancellationToken, returns);
                    matcher.Match($"Action<{descriptor.ParamsType.Name}>", isAction, takesParameter);
                    matcher.Match($"Action<{descriptor.ParamsType.Name}, CancellationToken>", isAction, takesParameter, containsCancellationToken);
                    if (descriptor.HasCapability)
                    {
                        matcher.Match(
                            $"Func<{descriptor.ParamsType.Name}, {descriptor.CapabilityType!.Name}, CancellationToken, {returnType.Name}>", isFunc, takesParameter,
                            takesCapability, containsCancellationToken, returns
                        );
                        matcher.Match(
                            $"Action<{descriptor.ParamsType.Name}, {descriptor.CapabilityType.Name}, CancellationToken>", isAction, takesParameter, takesCapability,
                            containsCancellationToken
                        );
                    }
                }
            }
            {
                var matcher = new MethodMatcher(sendMethodRegistries, descriptor, extensionClass);
                Func<MethodInfo, bool> containsCancellationToken = info => info.GetParameters().Reverse().Take(2).Any(x => x.ParameterType == typeof(CancellationToken));
                var returnType = descriptor.HasResponseType ? typeof(Task<>).MakeGenericType(descriptor.ResponseType!) : typeof(Task);
                Func<MethodInfo, bool> returns = info => info.ReturnType == returnType;
                Func<MethodInfo, bool> isAction = info => info.ReturnType.Name == "Void";
                Func<MethodInfo, bool> takesParameter = info => info.GetParameters().Skip(1).Any(z => z.ParameterType == descriptor.ParamsType);

                if (descriptor.IsRequest && descriptor.HasPartialItems)
                {
                    Func<MethodInfo, bool> partialReturnType = info =>
                        typeof(IRequestProgressObservable<,>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(descriptor.PartialItemsType!), descriptor.ResponseType!)
                                                             .IsAssignableFrom(info.ReturnType);
                    matcher.Match(
                        $"Func<{descriptor.ParamsType!.Name}, CancellationToken, IProgressObservable<IEnumerable<{descriptor.PartialItemsType!.Name}>, {descriptor.ResponseType!.Name}>>",
                        takesParameter, containsCancellationToken, partialReturnType
                    );
                }
                else if (descriptor.IsRequest && descriptor.HasPartialItem)
                {
                    Func<MethodInfo, bool> partialReturnType = info =>
                        typeof(IRequestProgressObservable<,>).MakeGenericType(descriptor.PartialItemType!, descriptor.ResponseType!).IsAssignableFrom(info.ReturnType);
                    matcher.Match(
                        $"Func<{descriptor.ParamsType!.Name}, CancellationToken, IProgressObservable<{descriptor.PartialItemType!.Name}, {descriptor.ResponseType!.Name}>>",
                        takesParameter, containsCancellationToken, partialReturnType
                    );
                }
                else if (descriptor.IsRequest)
                {
                    matcher.Match($"Func<{descriptor.ParamsType!.Name}, CancellationToken, {returnType.Name}>", takesParameter, containsCancellationToken, returns);
                }
                else if (descriptor.IsNotification)
                {
                    matcher.Match($"Action<{descriptor.ParamsType!.Name}>", isAction, takesParameter);
                }
            }
        }


        private class MethodMatcher
        {
            private readonly IEnumerable<Type> _registries;
            private readonly ILspHandlerTypeDescriptor _descriptor;
            private readonly Type _extensionClass;
            private readonly string? _methodName;

            public MethodMatcher(
                IEnumerable<Type> registries,
                ILspHandlerTypeDescriptor descriptor, Type extensionClass, string? methodName = null
            )
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
                    methods.Count.Should().BeGreaterThan(
                        0,
                        $"{_descriptor.HandlerType.FullName} missing extension with parameter type {description} method for {registry.FullName}"
                    );

                    foreach (var method in methods)
                    {
                        if (method.Name == GetOnMethodName(_descriptor))
                        {
                            var registrySub = Substitute.For(new[] { method.GetParameters()[0].ParameterType }, Array.Empty<object>());
                            SubstitutionContext.Current.GetCallRouterFor(registrySub).SetReturnForType(
                                method.GetParameters()[0].ParameterType,
                                new ReturnValue(registrySub)
                            );
                            // registrySub.ReturnsForAll(x => registrySub);

                            method.Invoke(
                                null,
                                new[] {
                                        registrySub, Substitute.For(new[] { method.GetParameters()[1].ParameterType }, Array.Empty<object>()),
                                    }.Concat(
                                          method.GetParameters().Skip(2).Select(
                                              z =>
                                                  !z.ParameterType.IsGenericType
                                                      ? Activator.CreateInstance(z.ParameterType)
                                                      : Substitute.For(new[] { z.ParameterType }, Array.Empty<object>())
                                          )
                                      )
                                     .ToArray()
                            );

                            registrySub.Received().ReceivedCalls()
                                       .Any(
                                            z => z.GetMethodInfo().Name == nameof(IJsonRpcHandlerRegistry<IJsonRpcHandlerRegistry<IJsonRpcServerRegistry>>.AddHandler) &&
                                                 z.GetArguments().Length == 3 &&
                                                 z.GetArguments()[0].Equals(_descriptor.Method)
                                        )
                                       .Should().BeTrue($"{_descriptor.HandlerType.Name} {description} should have the correct method.");

                            if (
                                _descriptor.HasRegistration
                             && _descriptor.RegistrationType is { }
                             && _descriptor.HasCapability
                             && _descriptor.CapabilityType is { }
                             && method.GetParameters().Length == 3
                            )
                            {
                                method.GetParameters()[2].ParameterType.Should().Be(
                                    typeof(RegistrationOptionsDelegate<,>).MakeGenericType(_descriptor.RegistrationType, _descriptor.CapabilityType),
                                    $"{_descriptor.HandlerType.FullName} {description} is has incorrect registration type {method.GetParameters()[2].ParameterType.FullName}"
                                );
                                method.GetParameters()[2].IsOptional.Should()
                                      .BeFalse($"{_descriptor.HandlerType.FullName} {description} Registration types should not be optional");
                            }
                            else if (_descriptor.HasRegistration && _descriptor.RegistrationType is { } && method.GetParameters().Length == 3)
                            {
                                method.GetParameters()[2].ParameterType.Should().Be(
                                    typeof(RegistrationOptionsDelegate<>).MakeGenericType(_descriptor.RegistrationType),
                                    $"{_descriptor.HandlerType.FullName} {description} is has incorrect registration type {method.GetParameters()[2].ParameterType.FullName}"
                                );
                                method.GetParameters()[2].IsOptional.Should()
                                      .BeFalse($"{_descriptor.HandlerType.FullName} {description} Registration types should not be optional");
                            }
                        }

                        if (_descriptor.IsRequest && method.Name == GetSendMethodName(_descriptor))
                        {
                            method.GetParameters().Last().ParameterType.Should().Be(
                                typeof(CancellationToken),
                                $"{_descriptor.HandlerType.Name} {description} send method must have optional cancellation token"
                            );
                            method.GetParameters().Last().IsOptional.Should().BeTrue(
                                $"{_descriptor.HandlerType.Name} {description} send method must have optional cancellation token"
                            );
                        }
                    }
                }
            }
        }

        public class ParamsShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public ParamsShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes.Where(
                    z =>
                        z.IsClass && !z.IsAbstract && z.GetInterfaces().Any(x => x.IsGenericType && typeof(IRequest<>).IsAssignableFrom(x.GetGenericTypeDefinition()))
                ))
                {
                    Add(type);
                }
            }
        }

        public class HandlersShouldHaveMethodAttributeData : TheoryData<Type>
        {
            public HandlersShouldHaveMethodAttributeData()
            {
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes.Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z))
                                                             .Where(z => !z.Name.EndsWith("Manager"))
                                                             .Except(new[] { typeof(ITextDocumentSyncHandler) })
                )
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    Add(type);
                }
            }
        }

        public class RegistrationConverters : TheoryData<Type>
        {
            public RegistrationConverters()
            {
                foreach (var type in typeof(CompletionParams)
                                    .Assembly.DefinedTypes
                                    .Where(z => z.IsClass && !z.IsAbstract && typeof(IRegistrationOptionsConverter).IsAssignableFrom(z))
                                    .Where(z => z.BaseType?.IsGenericType == true && z.BaseType.GetGenericArguments().Length == 2)
                                    .Where(z => !z.FullName?.Contains("TextDocumentSyncRegistrationOptions") == true)
                )
                {
                    Add(type);
                }
            }
        }

        public class TypeHandlerData : TheoryData<ILspHandlerTypeDescriptor>
        {
            public TypeHandlerData()
            {
                var handlerTypeDescriptorProvider =
                    new LspHandlerTypeDescriptorProvider(
                        new[] {
                            typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly,
                            typeof(LspHandlerTypeDescriptorProvider).Assembly,
                            typeof(LanguageServer).Assembly,
                            typeof(LanguageClient).Assembly,
                            typeof(ISupports).Assembly,
                            typeof(HandlerResolverTests).Assembly
                        }
                    );
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes.Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z))
                                                             .Where(z => !z.Name.EndsWith("Manager"))
                                                             .Except(new[] { typeof(ITextDocumentSyncHandler), typeof(IExecuteCommandHandler<>) })
                )
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    Add(handlerTypeDescriptorProvider.GetHandlerTypeDescriptor(type)!);
                }
            }
        }

        public class TypeHandlerExtensionData : TheoryData<ILspHandlerTypeDescriptor, string, string, Type, string>
        {
            public static Type[] HandlersToSkip = {
                typeof(ISemanticTokensFullHandler),
                typeof(ISemanticTokensDeltaHandler),
                typeof(ISemanticTokensRangeHandler),
                typeof(ICodeActionHandler)
            };

            public TypeHandlerExtensionData()
            {
                var handlerTypeDescriptorProvider =
                    new LspHandlerTypeDescriptorProvider(
                        new[] {
                            typeof(AssemblyScanningHandlerTypeDescriptorProvider).Assembly,
                            typeof(LspHandlerTypeDescriptorProvider).Assembly,
                            typeof(LanguageServer).Assembly,
                            typeof(LanguageClient).Assembly,
                            typeof(ISupports).Assembly,
                            typeof(HandlerResolverTests).Assembly
                        }
                    );
                foreach (var type in typeof(CompletionParams).Assembly.ExportedTypes
                                                             .Where(z => z.IsInterface && typeof(IJsonRpcHandler).IsAssignableFrom(z))
                                                             .Where(z => !z.Name.EndsWith("Manager"))
                                                             .Except(new[] { typeof(ITextDocumentSyncHandler) })
                )
                {
                    if (type.IsGenericTypeDefinition && !MethodAttribute.AllFrom(type).Any()) continue;
                    if (type.Name.EndsWith("Manager")) continue;
                    if (type == typeof(IExecuteCommandHandler<>)) continue;
                    if (type == typeof(ICompletionResolveHandler) || type == typeof(ICodeLensResolveHandler) || type == typeof(IDocumentLinkResolveHandler)
                     || type == typeof(ICodeActionResolveHandler)) continue;
                    if (type == typeof(ISemanticTokensFullHandler) || type == typeof(ISemanticTokensDeltaHandler) || type == typeof(ISemanticTokensRangeHandler)) continue;
                    var descriptor = handlerTypeDescriptorProvider.GetHandlerTypeDescriptor(type);

                    if (descriptor == null)
                    {
                        throw new Exception("");
                    }

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

        private static string GetExtensionClassName(IHandlerTypeDescriptor descriptor) => SpecialCasedHandlerName(descriptor) + "Extensions";

        private static string SpecialCasedHandlerName(IHandlerTypeDescriptor descriptor) =>
            new Regex(@"(\w+(?:\`\d)?)$")
               .Replace(
                    descriptor.HandlerType.Name,
                    descriptor.HandlerType.Name.Substring(1, descriptor.HandlerType.Name.IndexOf("Handler", StringComparison.Ordinal) - 1)
                );

        private static Type GetExtensionClass(IHandlerTypeDescriptor descriptor)
        {
            var name = GetExtensionClassName(descriptor);
            return descriptor.HandlerType.Assembly.GetExportedTypes()
                             .FirstOrDefault(z => z.IsClass && z.IsAbstract && ( z.Name == name || z.Name == name + "Base" ))!;
        }

        private static string GetOnMethodName(IHandlerTypeDescriptor descriptor) => "On" + SpecialCasedHandlerName(descriptor);

        private static string GetSendMethodName(ILspHandlerTypeDescriptor descriptor)
        {
            var name = SpecialCasedHandlerName(descriptor);
            if (name.StartsWith("Did")
             || name.StartsWith("Log")
             || name.StartsWith("Set")
             || name.StartsWith("Attach")
             || name.StartsWith("Launch")
             || name.StartsWith("Show")
             || name.StartsWith("Register")
             || name.StartsWith("Prepare")
             || name.StartsWith("Publish")
             || name.StartsWith("ApplyWorkspaceEdit")
             || name.StartsWith("Execute")
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

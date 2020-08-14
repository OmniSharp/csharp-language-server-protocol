using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using Xunit;
using static Generation.Tests.GenerationHelpers;

namespace Generation.Tests
{
    public class JsonRpcGenerationTests
    {
        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_ExitHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            var expected = @"using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExitExtensions
    {
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams, CancellationToken> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, CancellationToken, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
    }
}

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    public static partial class ExitExtensions
    {
        public static void SendExit(this ILanguageClient mediator, ExitParams @params) => mediator.SendNotification(@params);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_CapabilitiesHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test
{
    [Parallel, Method(EventNames.Capabilities, Direction.ServerToClient), GenerateHandlerMethods, GenerateRequestMethods]
    public interface ICapabilitiesHandler : IJsonRpcNotificationHandler<CapabilitiesEvent> { }
}";

            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class CapabilitiesExtensions
    {
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, Task> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent, CancellationToken> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, CancellationToken, Task> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
    }
}

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test
{
    public static partial class CapabilitiesExtensions
    {
        public static void SendCapabilities(this IDebugAdapterServer mediator, CapabilitiesEvent @params) => mediator.SendNotification(@params);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }


        [Fact]
        public async Task Supports_Generating_Notifications_ExitHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry)), GenerateRequestMethods(typeof(ILanguageClient))]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            var expected = @"using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExitExtensions
    {
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams, CancellationToken> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, CancellationToken, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
    }
}

namespace Test
{
    public static partial class ExitExtensions
    {
        public static void SendExit(this ILanguageClient mediator, ExitParams @params) => mediator.SendNotification(@params);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_DidChangeTextHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial, Method(TextDocumentNames.DidChange, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
        IRegistration<TextDocumentChangeRegistrationOptions>, ICapability<SynchronizationCapability>
    { }
}";

            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DidChangeTextDocumentExtensions
    {
        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams> handler, TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, Task> handler, TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams, CancellationToken> handler, TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, CancellationToken, Task> handler, TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> handler, TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability, TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler, TextDocumentChangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TextDocumentChangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, SynchronizationCapability, TextDocumentChangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    public static partial class DidChangeTextDocumentExtensions
    {
        public static void DidChangeTextDocument(this ILanguageClient mediator, DidChangeTextDocumentParams @params) => mediator.SendNotification(@params);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Generating_Notifications_And_Infers_Direction_FoldingRangeHandler()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Parallel, Method(TextDocumentNames.FoldingRange, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>,
        IRegistration<FoldingRangeRegistrationOptions>, ICapability<FoldingRangeCapability>
    {
    }
}";

            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class FoldingRangeExtensions
    {
        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, Task<OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>>> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.RequestRegistration<FoldingRangeRequestParam, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>, FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, CancellationToken, Task<OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>>> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.RequestRegistration<FoldingRangeRequestParam, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>, FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, CancellationToken> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>(values)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>(values)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, FoldingRangeCapability, CancellationToken> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>, FoldingRange, FoldingRangeCapability, FoldingRangeRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>(values)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, FoldingRangeCapability, CancellationToken, Task<OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>>> handler, FoldingRangeRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new FoldingRangeRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>, FoldingRangeCapability, FoldingRangeRegistrationOptions>(handler, registrationOptions));
        }
    }
}

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    public static partial class FoldingRangeExtensions
    {
        public static IRequestProgressObservable<IEnumerable<FoldingRange>, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>> RequestFoldingRange(this ITextDocumentLanguageClient mediator, FoldingRangeRequestParam @params, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(@params, value => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>(value), cancellationToken);
        public static IRequestProgressObservable<IEnumerable<FoldingRange>, OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>> RequestFoldingRange(this ILanguageClient mediator, FoldingRangeRequestParam @params, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(@params, value => new OmniSharp.Extensions.LanguageServer.Protocol.Models.Container<OmniSharp.Extensions.LanguageServer.Protocol.Models.FoldingRange>(value), cancellationToken);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Generating_Requests_And_Infers_Direction()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions>, ICapability<DefinitionCapability> { }
}";

            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DefinitionExtensions
    {
        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, Task<LocationOrLocationLinks>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.RequestRegistration<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.RequestRegistration<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new LocationOrLocationLinks(values)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new LocationOrLocationLinks(values)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, DefinitionCapability, CancellationToken> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionCapability, DefinitionRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new LocationOrLocationLinks(values)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, DefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionCapability, DefinitionRegistrationOptions>(handler, registrationOptions));
        }
    }
}

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    public static partial class DefinitionExtensions
    {
        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestDefinition(this ILanguageClient mediator, DefinitionParams @params, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(@params, value => new LocationOrLocationLinks(value), cancellationToken);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }


        [Fact]
        public async Task Supports_Generating_Requests()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace Test
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry)), GenerateRequestMethods(typeof(ITextDocumentLanguageClient))]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions>, ICapability<DefinitionCapability> { }
}";

            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DefinitionExtensions
    {
        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, Task<LocationOrLocationLinks>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.RequestRegistration<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.RequestRegistration<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions>(handler, registrationOptions));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new LocationOrLocationLinks(values)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new LocationOrLocationLinks(values)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, DefinitionCapability, CancellationToken> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionCapability, DefinitionRegistrationOptions>(handler, registrationOptions, _.GetService<IProgressManager>(), values => new LocationOrLocationLinks(values)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, DefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>> handler, DefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionCapability, DefinitionRegistrationOptions>(handler, registrationOptions));
        }
    }
}

namespace Test
{
    public static partial class DefinitionExtensions
    {
        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestDefinition(this ITextDocumentLanguageClient mediator, DefinitionParams @params, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(@params, value => new LocationOrLocationLinks(value), cancellationToken);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Custom_Method_Names()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Test
{
    [Serial, Method(GeneralNames.Initialize, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry), MethodName = ""OnLanguageProtocolInitialize""), GenerateRequestMethods(typeof(ITextDocumentLanguageClient), MethodName = ""RequestLanguageProtocolInitialize"")]
    public interface ILanguageProtocolInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> {}
}";
            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class LanguageProtocolInitializeExtensions
    {
        public static ILanguageServerRegistry OnLanguageProtocolInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, Task<InitializeResult>> handler) => registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnLanguageProtocolInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, CancellationToken, Task<InitializeResult>> handler) => registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
    }
}

namespace Test
{
    public static partial class LanguageProtocolInitializeExtensions
    {
        public static Task<InitializeResult> RequestLanguageProtocolInitialize(this ITextDocumentLanguageClient mediator, InitializeParams @params, CancellationToken cancellationToken = default) => mediator.SendRequest(@params, cancellationToken);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Allow_Derived_Requests()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Attach, Direction.ClientToServer)]
    [GenerateHandlerMethods(AllowDerivedRequests = true), GenerateRequestMethods]
    public interface IAttachHandler : IJsonRpcRequestHandler<AttachRequestArguments, AttachResponse> { }
}";
            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class AttachExtensions
    {
        public static IDebugAdapterServerRegistry OnAttach(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse>> handler) => registry.AddHandler(RequestNames.Attach, RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttach(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler) => registry.AddHandler(RequestNames.Attach, RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttach<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(RequestNames.Attach, RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttach<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(RequestNames.Attach, RequestHandler.For(handler));
    }
}

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public static partial class AttachExtensions
    {
        public static Task<AttachResponse> RequestAttach(this IDebugAdapterClient mediator, AttachRequestArguments @params, CancellationToken cancellationToken = default) => mediator.SendRequest(@params, cancellationToken);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Allow_Generic_Types()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public class AttachResponse { }
    [Method(""attach"", Direction.ClientToServer)]
    public class AttachRequestArguments: IRequest<AttachResponse> { }

    [Parallel, Method(""attach"", Direction.ClientToServer)]
    [GenerateHandlerMethods(AllowDerivedRequests = true), GenerateRequestMethods]
    public interface IAttachHandler<in T> : IJsonRpcRequestHandler<T, AttachResponse> where T : AttachRequestArguments { }
    public interface IAttachHandler : IAttachHandler<AttachRequestArguments> { }
}";
            var expected = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class AttachExtensions
    {
        public static IDebugAdapterServerRegistry OnAttach(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttach(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttach<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttach<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
    }
}

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public static partial class AttachExtensions
    {
        public static Task<AttachResponse> RequestAttach(this IDebugAdapterClient mediator, AttachRequestArguments @params, CancellationToken cancellationToken = default) => mediator.SendRequest(@params, cancellationToken);
    }
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }
    }
}

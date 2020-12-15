using System;
using System.Threading.Tasks;
using FluentAssertions;
using OmniSharp.Extensions.JsonRpc.Generators;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
using TestingUtils;
using Xunit;
using Xunit.Sdk;
using static Generation.Tests.GenerationHelpers;

namespace Generation.Tests
{
    public class JsonRpcGenerationTests
    {
        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExitExtensions
    {
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams, CancellationToken> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, CancellationToken, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static void SendExit(this ILanguageClient mediator, ExitParams request) => mediator.SendNotification(request);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Generating_Generic_Response_Types()
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial]
    [Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    [
        GenerateHandler(""OmniSharp.Extensions.LanguageServer.Protocol.Workspace"" /*, AllowDerivedRequests = true*/),
        GenerateHandlerMethods,
        GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
            ]
        [RegistrationOptions(typeof(ExecuteCommandRegistrationOptions)), Capability(typeof(ExecuteCommandCapability))]
    public partial record ExecuteCommandParams<T> : IRequest<T>, IWorkDoneProgressParams, IExecuteCommandParams
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        public string Command { get; init; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        [Optional]
        public JArray? Arguments { get; init; }
    }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Serial, Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IExecuteCommandHandler<T> : IJsonRpcRequestHandler<ExecuteCommandParams<T>, T>, IRegistration<ExecuteCommandRegistrationOptions, ExecuteCommandCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class ExecuteCommandHandlerBase<T> : AbstractHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>, IExecuteCommandHandler<T>
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExecuteCommandExtensions1
    {
        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, Func<ExecuteCommandParams<T>, Task<T>> handler, RegistrationOptionsDelegate<ExecuteCommandRegistrationOptions, ExecuteCommandCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.ExecuteCommand, new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>(HandlerAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandParams<T>, T>(handler), RegistrationAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, Func<ExecuteCommandParams<T>, CancellationToken, Task<T>> handler, RegistrationOptionsDelegate<ExecuteCommandRegistrationOptions, ExecuteCommandCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.ExecuteCommand, new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>(HandlerAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandParams<T>, T>(handler), RegistrationAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, Func<ExecuteCommandParams<T>, ExecuteCommandCapability, CancellationToken, Task<T>> handler, RegistrationOptionsDelegate<ExecuteCommandRegistrationOptions, ExecuteCommandCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.ExecuteCommand, new LanguageProtocolDelegatingHandlers.Request<ExecuteCommandParams<T>, T, ExecuteCommandRegistrationOptions, ExecuteCommandCapability>(HandlerAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandParams<T>, T>(handler), RegistrationAdapter<ExecuteCommandCapability>.Adapt<ExecuteCommandRegistrationOptions>(registrationOptions)));
        }

        public static Task<T> ExecuteCommand<T>(this IWorkspaceLanguageClient mediator, ExecuteCommandParams<T> request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<T> ExecuteCommand<T>(this ILanguageClient mediator, ExecuteCommandParams<T> request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Should_Report_Diagnostic_If_Missing_Information()
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            CacheKeyHasher.Cache = true;
            Func<Task> a = () => AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, "");
            a.Should().Throw<EmptyException>().WithMessage("*Could not infer the request router(s)*");
            a.Should().Throw<EmptyException>("cache").WithMessage("*Could not infer the request router(s)*");
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class CapabilitiesExtensions
    {
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, Task> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Action<CapabilitiesEvent, CancellationToken> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static IDebugAdapterClientRegistry OnCapabilities(this IDebugAdapterClientRegistry registry, Func<CapabilitiesEvent, CancellationToken, Task> handler) => registry.AddHandler(EventNames.Capabilities, NotificationHandler.For(handler));
        public static void SendCapabilities(this IDebugAdapterServer mediator, CapabilitiesEvent request) => mediator.SendNotification(request);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }


        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Test
{
    [Serial, Method(GeneralNames.Exit, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry)), GenerateRequestMethods(typeof(ILanguageClient))]
    public interface IExitHandler : IJsonRpcNotificationHandler<ExitParams>
    {
    }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class ExitExtensions
    {
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Action<ExitParams, CancellationToken> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static ILanguageServerRegistry OnExit(this ILanguageServerRegistry registry, Func<ExitParams, CancellationToken, Task> handler) => registry.AddHandler(GeneralNames.Exit, NotificationHandler.For(handler));
        public static void SendExit(this ILanguageClient mediator, ExitParams request) => mediator.SendNotification(request);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Serial, Method(TextDocumentNames.DidChange, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods]
    public interface IDidChangeTextDocumentHandler : IJsonRpcNotificationHandler<DidChangeTextDocumentParams>,
        IRegistration<TextDocumentChangeRegistrationOptions, SynchronizationCapability>
    { }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DidChangeTextDocumentExtensions
    {
        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, SynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, SynchronizationCapability>(HandlerAdapter<SynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<SynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, Task> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, SynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, SynchronizationCapability>(HandlerAdapter<SynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<SynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams, CancellationToken> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, SynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, SynchronizationCapability>(HandlerAdapter<SynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<SynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, SynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, SynchronizationCapability>(HandlerAdapter<SynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<SynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, SynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, SynchronizationCapability>(HandlerAdapter<SynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<SynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, SynchronizationCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, SynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, SynchronizationCapability>(HandlerAdapter<SynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<SynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static void DidChangeTextDocument(this ILanguageClient mediator, DidChangeTextDocumentParams request) => mediator.SendNotification(request);
    }
#nullable restore
}
";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Parallel, Method(TextDocumentNames.FoldingRange, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IFoldingRangeHandler : IJsonRpcRequestHandler<FoldingRangeRequestParam, Container<FoldingRange>>,
        IRegistration<FoldingRangeRegistrationOptions, FoldingRangeCapability>
    {
    }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class FoldingRangeExtensions
    {
        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, Task<Container<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeRegistrationOptions, FoldingRangeCapability>(HandlerAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, Container<FoldingRange>>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, CancellationToken, Task<Container<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeRegistrationOptions, FoldingRangeCapability>(HandlerAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, Container<FoldingRange>>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnFoldingRange(this ILanguageServerRegistry registry, Func<FoldingRangeRequestParam, FoldingRangeCapability, CancellationToken, Task<Container<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, new LanguageProtocolDelegatingHandlers.Request<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRangeRegistrationOptions, FoldingRangeCapability>(HandlerAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, Container<FoldingRange>>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions, FoldingRangeCapability>(PartialAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, FoldingRange>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<FoldingRange>.From));
        }

        public static ILanguageServerRegistry ObserveFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, CancellationToken> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions, FoldingRangeCapability>(PartialAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, FoldingRange>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<FoldingRange>.From));
        }

        public static ILanguageServerRegistry ObserveFoldingRange(this ILanguageServerRegistry registry, Action<FoldingRangeRequestParam, IObserver<IEnumerable<FoldingRange>>, FoldingRangeCapability, CancellationToken> handler, RegistrationOptionsDelegate<FoldingRangeRegistrationOptions, FoldingRangeCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.FoldingRange, _ => new LanguageProtocolDelegatingHandlers.PartialResults<FoldingRangeRequestParam, Container<FoldingRange>, FoldingRange, FoldingRangeRegistrationOptions, FoldingRangeCapability>(PartialAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRequestParam, FoldingRange>(handler), RegistrationAdapter<FoldingRangeCapability>.Adapt<FoldingRangeRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<FoldingRange>.From));
        }

        public static IRequestProgressObservable<IEnumerable<FoldingRange>, Container<FoldingRange>> RequestFoldingRange(this ITextDocumentLanguageClient mediator, FoldingRangeRequestParam request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new Container<FoldingRange>(value), cancellationToken);
        public static IRequestProgressObservable<IEnumerable<FoldingRange>, Container<FoldingRange>> RequestFoldingRange(this ILanguageClient mediator, FoldingRangeRequestParam request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new Container<FoldingRange>(value), cancellationToken);
    }
#nullable restore
}
";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer), GenerateHandlerMethods, GenerateRequestMethods, Obsolete(""This is obsolete"")]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions, DefinitionCapability> { }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute, Obsolete(""This is obsolete"")]
    public static partial class DefinitionExtensions
    {
        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, Task<LocationOrLocationLinks>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions, DefinitionCapability>(HandlerAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLinks>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions, DefinitionCapability>(HandlerAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLinks>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, DefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions, DefinitionCapability>(HandlerAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLinks>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions, DefinitionCapability>(PartialAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLink>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), LocationOrLocationLinks.From));
        }

        public static ILanguageServerRegistry ObserveDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions, DefinitionCapability>(PartialAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLink>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), LocationOrLocationLinks.From));
        }

        public static ILanguageServerRegistry ObserveDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, DefinitionCapability, CancellationToken> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions, DefinitionCapability>(PartialAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLink>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), LocationOrLocationLinks.From));
        }

        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestDefinition(this ILanguageClient mediator, DefinitionParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new LocationOrLocationLinks(value), cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }


        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Generating_Requests()
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace Test
{
    [Parallel, Method(TextDocumentNames.Definition, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry)), GenerateRequestMethods(typeof(ITextDocumentLanguageClient))]
    public interface IDefinitionHandler : IJsonRpcRequestHandler<DefinitionParams, LocationOrLocationLinks>, IRegistration<DefinitionRegistrationOptions, DefinitionCapability> { }
}";

            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DefinitionExtensions
    {
        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, Task<LocationOrLocationLinks>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions, DefinitionCapability>(HandlerAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLinks>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions, DefinitionCapability>(HandlerAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLinks>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDefinition(this ILanguageServerRegistry registry, Func<DefinitionParams, DefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, new LanguageProtocolDelegatingHandlers.Request<DefinitionParams, LocationOrLocationLinks, DefinitionRegistrationOptions, DefinitionCapability>(HandlerAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLinks>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions, DefinitionCapability>(PartialAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLink>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), LocationOrLocationLinks.From));
        }

        public static ILanguageServerRegistry ObserveDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions, DefinitionCapability>(PartialAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLink>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), LocationOrLocationLinks.From));
        }

        public static ILanguageServerRegistry ObserveDefinition(this ILanguageServerRegistry registry, Action<DefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, DefinitionCapability, CancellationToken> handler, RegistrationOptionsDelegate<DefinitionRegistrationOptions, DefinitionCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Definition, _ => new LanguageProtocolDelegatingHandlers.PartialResults<DefinitionParams, LocationOrLocationLinks, LocationOrLocationLink, DefinitionRegistrationOptions, DefinitionCapability>(PartialAdapter<DefinitionCapability>.Adapt<DefinitionParams, LocationOrLocationLink>(handler), RegistrationAdapter<DefinitionCapability>.Adapt<DefinitionRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), LocationOrLocationLinks.From));
        }

        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestDefinition(this ITextDocumentLanguageClient mediator, DefinitionParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, value => new LocationOrLocationLinks(value), cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Custom_Method_Names()
        {
            var source = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace Test
{
    [Serial, Method(GeneralNames.Initialize, Direction.ClientToServer), GenerateHandlerMethods(typeof(ILanguageServerRegistry), MethodName = ""OnLanguageProtocolInitialize""), GenerateRequestMethods(typeof(ITextDocumentLanguageClient), MethodName = ""RequestLanguageProtocolInitialize"")]
    public interface ILanguageProtocolInitializeHandler : IJsonRpcRequestHandler<InitializeParams, InitializeResult> {}
}";
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Test;

namespace Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class LanguageProtocolInitializeExtensions
    {
        public static ILanguageServerRegistry OnLanguageProtocolInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, Task<InitializeResult>> handler) => registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        public static ILanguageServerRegistry OnLanguageProtocolInitialize(this ILanguageServerRegistry registry, Func<InitializeParams, CancellationToken, Task<InitializeResult>> handler) => registry.AddHandler(GeneralNames.Initialize, RequestHandler.For(handler));
        public static Task<InitializeResult> RequestLanguageProtocolInitialize(this ITextDocumentLanguageClient mediator, InitializeParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    public class AttachResponse { }
    [Method(""attach"", Direction.ClientToServer)]
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class AttachRequestArguments: IRequest<AttachResponse> { }
}";
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Bogus;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    [Method(""attach"", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IAttachRequestHandler<in T> : IJsonRpcRequestHandler<T, AttachResponse> where T : AttachRequestArguments
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase<T> : AbstractHandlers.Request<T, AttachResponse>, IAttachRequestHandler<T> where T : AttachRequestArguments
    {
    }

    public partial interface IAttachRequestHandler : IAttachRequestHandler<AttachRequestArguments>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase : AttachRequestHandlerBase<AttachRequestArguments>, IAttachRequestHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class AttachRequestExtensions
    {
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static Task<AttachResponse> AttachRequest(this IDebugAdapterClient mediator, AttachRequestArguments request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Allow_Derived_Requests_Nullable()
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
    public class LaunchResponse { }
    [Method(""launch"", Direction.ClientToServer)]
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class LaunchRequestArguments: IRequest<LaunchResponse?> { }
}";
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Bogus;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    [Method(""launch"", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface ILaunchRequestHandler<in T> : IJsonRpcRequestHandler<T, LaunchResponse?> where T : LaunchRequestArguments
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class LaunchRequestHandlerBase<T> : AbstractHandlers.Request<T, LaunchResponse?>, ILaunchRequestHandler<T> where T : LaunchRequestArguments
    {
    }

    public partial interface ILaunchRequestHandler : ILaunchRequestHandler<LaunchRequestArguments>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class LaunchRequestHandlerBase : LaunchRequestHandlerBase<LaunchRequestArguments>, ILaunchRequestHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class LaunchRequestExtensions
    {
        public static IDebugAdapterServerRegistry OnLaunchRequest(this IDebugAdapterServerRegistry registry, Func<LaunchRequestArguments, Task<LaunchResponse?>> handler) => registry.AddHandler(""launch"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnLaunchRequest(this IDebugAdapterServerRegistry registry, Func<LaunchRequestArguments, CancellationToken, Task<LaunchResponse?>> handler) => registry.AddHandler(""launch"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnLaunchRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<LaunchResponse?>> handler)
            where T : LaunchRequestArguments => registry.AddHandler(""launch"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnLaunchRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<LaunchResponse?>> handler)
            where T : LaunchRequestArguments => registry.AddHandler(""launch"", RequestHandler.For(handler));
        public static Task<LaunchResponse?> LaunchRequest(this IDebugAdapterClient mediator, LaunchRequestArguments request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [Fact]
        public async Task Supports_Allows_Nullable_Responses()
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
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class AttachRequestArguments: IRequest<AttachResponse?> { }
}";
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Bogus;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    [Method(""attach"", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IAttachRequestHandler<in T> : IJsonRpcRequestHandler<T, AttachResponse?> where T : AttachRequestArguments
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase<T> : AbstractHandlers.Request<T, AttachResponse?>, IAttachRequestHandler<T> where T : AttachRequestArguments
    {
    }

    public partial interface IAttachRequestHandler : IAttachRequestHandler<AttachRequestArguments>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase : AttachRequestHandlerBase<AttachRequestArguments>, IAttachRequestHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class AttachRequestExtensions
    {
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse?>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse?>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<AttachResponse?>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<AttachResponse?>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static Task<AttachResponse?> AttachRequest(this IDebugAdapterClient mediator, AttachRequestArguments request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
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
    [GenerateHandler(AllowDerivedRequests = true), GenerateHandlerMethods, GenerateRequestMethods]
    public class AttachRequestArguments: IRequest<AttachResponse> { }
}";
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Bogus;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.DebugAdapter.Protocol.Server;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
    [Method(""attach"", Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IAttachRequestHandler<in T> : IJsonRpcRequestHandler<T, AttachResponse> where T : AttachRequestArguments
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase<T> : AbstractHandlers.Request<T, AttachResponse>, IAttachRequestHandler<T> where T : AttachRequestArguments
    {
    }

    public partial interface IAttachRequestHandler : IAttachRequestHandler<AttachRequestArguments>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class AttachRequestHandlerBase : AttachRequestHandlerBase<AttachRequestArguments>, IAttachRequestHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Bogus
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class AttachRequestExtensions
    {
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, Task<AttachResponse>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest(this IDebugAdapterServerRegistry registry, Func<AttachRequestArguments, CancellationToken, Task<AttachResponse>> handler) => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static IDebugAdapterServerRegistry OnAttachRequest<T>(this IDebugAdapterServerRegistry registry, Func<T, CancellationToken, Task<AttachResponse>> handler)
            where T : AttachRequestArguments => registry.AddHandler(""attach"", RequestHandler.For(handler));
        public static Task<AttachResponse> AttachRequest(this IDebugAdapterClient mediator, AttachRequestArguments request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Params_Type_As_Source()
        {
            var source = @"
using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using System.Collections.Generic;
using MediatR;
using RenameRegistrationOptions = OmniSharp.Extensions.LanguageServer.Protocol.Models.RenameRegistrationOptions;
using RenameCapability = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.RenameCapability;
using ITextDocumentIdentifierParams = OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentIdentifierParams;
using WorkspaceEdit = OmniSharp.Extensions.LanguageServer.Protocol.Models.WorkspaceEdit;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus
{
    [Parallel]
    [Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    [GenerateHandler(""OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers""), GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    [RegistrationOptions(typeof(RenameRegistrationOptions))]
    [Capability(typeof(RenameCapability))]
    public partial class RenameParams : ITextDocumentIdentifierParams, IRequest<WorkspaceEdit?>
    {
    }
}";
            var expected = @"
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.DebugAdapter.Protocol;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Bogus;
using OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using RenameCapability = OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities.RenameCapability;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ITextDocumentIdentifierParams = OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentIdentifierParams;
using RenameRegistrationOptions = OmniSharp.Extensions.LanguageServer.Protocol.Models.RenameRegistrationOptions;
using WorkspaceEdit = OmniSharp.Extensions.LanguageServer.Protocol.Models.WorkspaceEdit;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers
{
    [Parallel, Method(TextDocumentNames.Rename, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface IRenameHandler : IJsonRpcRequestHandler<RenameParams, WorkspaceEdit?>, IRegistration<RenameRegistrationOptions, RenameCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class RenameHandlerBase : AbstractHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>, IRenameHandler
    {
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Bogus.Handlers
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class RenameExtensions
    {
        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, Task<WorkspaceEdit?>> handler, RegistrationOptionsDelegate<RenameRegistrationOptions, RenameCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(HandlerAdapter<RenameCapability>.Adapt<RenameParams, WorkspaceEdit?>(handler), RegistrationAdapter<RenameCapability>.Adapt<RenameRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, CancellationToken, Task<WorkspaceEdit?>> handler, RegistrationOptionsDelegate<RenameRegistrationOptions, RenameCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(HandlerAdapter<RenameCapability>.Adapt<RenameParams, WorkspaceEdit?>(handler), RegistrationAdapter<RenameCapability>.Adapt<RenameRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnRename(this ILanguageServerRegistry registry, Func<RenameParams, RenameCapability, CancellationToken, Task<WorkspaceEdit?>> handler, RegistrationOptionsDelegate<RenameRegistrationOptions, RenameCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.Rename, new LanguageProtocolDelegatingHandlers.Request<RenameParams, WorkspaceEdit?, RenameRegistrationOptions, RenameCapability>(HandlerAdapter<RenameCapability>.Adapt<RenameParams, WorkspaceEdit?>(handler), RegistrationAdapter<RenameCapability>.Adapt<RenameRegistrationOptions>(registrationOptions)));
        }

        public static Task<WorkspaceEdit?> RequestRename(this ITextDocumentLanguageClient mediator, RenameParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
        public static Task<WorkspaceEdit?> RequestRename(this ILanguageClient mediator, RenameParams request, CancellationToken cancellationToken = default) => mediator.SendRequest(request, cancellationToken);
    }
#nullable restore
}";
            await AssertGeneratedAsExpected<GenerateHandlerMethodsGenerator>(source, expected);
        }
    }
}

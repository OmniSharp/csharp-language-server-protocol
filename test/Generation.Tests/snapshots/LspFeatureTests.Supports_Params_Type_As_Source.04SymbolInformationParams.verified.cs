//HintName: SymbolInformationParams.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial interface ISymbolInformationHandler : IJsonRpcRequestHandler<SymbolInformationParams, Container<SymbolInformation>?>, IRegistration<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SymbolInformationHandlerBase : AbstractHandlers.Request<SymbolInformationParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>, ISymbolInformationHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class SymbolInformationPartialHandlerBase : AbstractHandlers.PartialResults<SymbolInformationParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>, ISymbolInformationHandler
    {
        protected SymbolInformationPartialHandlerBase(System.Guid id, IProgressManager progressManager) : base(progressManager, Container<SymbolInformation>.From)
        {
        }
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class SymbolInformationExtensions
    {
        public static ILanguageServerRegistry OnSymbolInformation(this ILanguageServerRegistry registry, Func<SymbolInformationParams, Task<Container<SymbolInformation>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<SymbolInformationParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<SymbolInformationParams, Container<SymbolInformation>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSymbolInformation(this ILanguageServerRegistry registry, Func<SymbolInformationParams, CancellationToken, Task<Container<SymbolInformation>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<SymbolInformationParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<SymbolInformationParams, Container<SymbolInformation>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnSymbolInformation(this ILanguageServerRegistry registry, Func<SymbolInformationParams, WorkspaceSymbolCapability, CancellationToken, Task<Container<SymbolInformation>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<SymbolInformationParams, Container<SymbolInformation>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<SymbolInformationParams, Container<SymbolInformation>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveSymbolInformation(this ILanguageServerRegistry registry, Action<SymbolInformationParams, IObserver<IEnumerable<SymbolInformation>>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SymbolInformationParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<SymbolInformationParams, SymbolInformation>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<SymbolInformation>.From));
        }

        public static ILanguageServerRegistry ObserveSymbolInformation(this ILanguageServerRegistry registry, Action<SymbolInformationParams, IObserver<IEnumerable<SymbolInformation>>, CancellationToken> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SymbolInformationParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<SymbolInformationParams, SymbolInformation>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<SymbolInformation>.From));
        }

        public static ILanguageServerRegistry ObserveSymbolInformation(this ILanguageServerRegistry registry, Action<SymbolInformationParams, IObserver<IEnumerable<SymbolInformation>>, WorkspaceSymbolCapability, CancellationToken> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<SymbolInformationParams, Container<SymbolInformation>?, SymbolInformation, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<SymbolInformationParams, SymbolInformation>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<SymbolInformation>.From));
        }

        public static IRequestProgressObservable<IEnumerable<SymbolInformation>, Container<SymbolInformation>?> RequestSymbolInformation(this ITextDocumentLanguageClient mediator, SymbolInformationParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<SymbolInformation>.From, cancellationToken);
        public static IRequestProgressObservable<IEnumerable<SymbolInformation>, Container<SymbolInformation>?> RequestSymbolInformation(this ILanguageClient mediator, SymbolInformationParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<SymbolInformation>.From, cancellationToken);
    }
#nullable restore
}
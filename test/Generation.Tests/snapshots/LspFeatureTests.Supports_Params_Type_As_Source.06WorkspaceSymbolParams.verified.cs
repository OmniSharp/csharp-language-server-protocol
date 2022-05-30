//HintName: WorkspaceSymbolParams.cs
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
    public partial interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<WorkspaceSymbol>?>, IRegistration<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class WorkspaceSymbolsHandlerBase : AbstractHandlers.Request<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>, IWorkspaceSymbolsHandler
    {
    }

    [System.Runtime.CompilerServices.CompilerGeneratedAttribute, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    abstract public partial class WorkspaceSymbolsPartialHandlerBase : AbstractHandlers.PartialResults<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbol, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>, IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolsPartialHandlerBase(System.Guid id, IProgressManager progressManager) : base(progressManager, Container<WorkspaceSymbol>.From)
        {
        }
    }
}
#nullable restore

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class WorkspaceSymbolsExtensions
    {
        public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry, Func<WorkspaceSymbolParams, Task<Container<WorkspaceSymbol>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, Container<WorkspaceSymbol>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry, Func<WorkspaceSymbolParams, CancellationToken, Task<Container<WorkspaceSymbol>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, Container<WorkspaceSymbol>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry, Func<WorkspaceSymbolParams, WorkspaceSymbolCapability, CancellationToken, Task<Container<WorkspaceSymbol>?>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(HandlerAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, Container<WorkspaceSymbol>?>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry ObserveWorkspaceSymbols(this ILanguageServerRegistry registry, Action<WorkspaceSymbolParams, IObserver<IEnumerable<WorkspaceSymbol>>> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbol, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, WorkspaceSymbol>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<WorkspaceSymbol>.From));
        }

        public static ILanguageServerRegistry ObserveWorkspaceSymbols(this ILanguageServerRegistry registry, Action<WorkspaceSymbolParams, IObserver<IEnumerable<WorkspaceSymbol>>, CancellationToken> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbol, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, WorkspaceSymbol>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<WorkspaceSymbol>.From));
        }

        public static ILanguageServerRegistry ObserveWorkspaceSymbols(this ILanguageServerRegistry registry, Action<WorkspaceSymbolParams, IObserver<IEnumerable<WorkspaceSymbol>>, WorkspaceSymbolCapability, CancellationToken> handler, RegistrationOptionsDelegate<WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability> registrationOptions)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol, _ => new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<WorkspaceSymbol>?, WorkspaceSymbol, WorkspaceSymbolRegistrationOptions, WorkspaceSymbolCapability>(PartialAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolParams, WorkspaceSymbol>(handler), RegistrationAdapter<WorkspaceSymbolCapability>.Adapt<WorkspaceSymbolRegistrationOptions>(registrationOptions), _.GetService<IProgressManager>(), Container<WorkspaceSymbol>.From));
        }

        public static IRequestProgressObservable<IEnumerable<WorkspaceSymbol>, Container<WorkspaceSymbol>?> RequestWorkspaceSymbols(this ITextDocumentLanguageClient mediator, WorkspaceSymbolParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<WorkspaceSymbol>.From, cancellationToken);
        public static IRequestProgressObservable<IEnumerable<WorkspaceSymbol>, Container<WorkspaceSymbol>?> RequestWorkspaceSymbols(this ILanguageClient mediator, WorkspaceSymbolParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, Container<WorkspaceSymbol>.From, cancellationToken);
    }
#nullable restore
}
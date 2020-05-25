using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>>, ICapability<WorkspaceSymbolCapability>, IRegistration<WorkspaceSymbolRegistrationOptions> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolCapability Capability { get; private set; }
        private readonly WorkspaceSymbolRegistrationOptions _options;

        public WorkspaceSymbolsHandler(WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public WorkspaceSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(WorkspaceSymbolCapability capability) => Capability = capability;
    }

    public static class WorkspaceSymbolsExtensions
    {
public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, WorkspaceSymbolCapability, CancellationToken, Task<Container<SymbolInformation>>>
                handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>, WorkspaceSymbolCapability,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, WorkspaceSymbolCapability, Task<Container<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.Request<WorkspaceSymbolParams, Container<SymbolInformation>, WorkspaceSymbolCapability,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, CancellationToken, Task<Container<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<WorkspaceSymbolParams, Container<SymbolInformation>,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Func<WorkspaceSymbolParams, Task<Container<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<WorkspaceSymbolParams, Container<SymbolInformation>,
                    WorkspaceSymbolRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>, WorkspaceSymbolCapability, CancellationToken> handler, WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>,
                        SymbolInformation, WorkspaceSymbolCapability, WorkspaceSymbolRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new Container<SymbolInformation>(x)));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>, WorkspaceSymbolCapability>
                handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>,
                        SymbolInformation, WorkspaceSymbolCapability, WorkspaceSymbolRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new Container<SymbolInformation>(x)));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>, CancellationToken> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>,
                        SymbolInformation, WorkspaceSymbolRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new Container<SymbolInformation>(x)));
        }

public static ILanguageServerRegistry OnWorkspaceSymbols(this ILanguageServerRegistry registry,
            Action<WorkspaceSymbolParams, IObserver<IEnumerable<SymbolInformation>>> handler,
            WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new WorkspaceSymbolRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.WorkspaceSymbol,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<WorkspaceSymbolParams, Container<SymbolInformation>,
                        SymbolInformation, WorkspaceSymbolRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new Container<SymbolInformation>(x)));
        }

        public static IRequestProgressObservable<IEnumerable<SymbolInformation>, Container<SymbolInformation>> RequestWorkspaceSymbols(
            this IWorkspaceLanguageClient mediator,
            WorkspaceSymbolParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new Container<SymbolInformation>(x), cancellationToken);
        }
    }
}

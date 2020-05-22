using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Serial, Method(WorkspaceNames.DidChangeWatchedFiles, Direction.ClientToServer)]
    public interface IDidChangeWatchedFilesHandler : IJsonRpcNotificationHandler<DidChangeWatchedFilesParams>, IRegistration<DidChangeWatchedFilesRegistrationOptions>, ICapability<DidChangeWatchedFilesCapability> { }

    public abstract class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private readonly DidChangeWatchedFilesRegistrationOptions _options;
        public DidChangeWatchedFilesHandler(DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWatchedFilesCapability capability) => Capability = capability;
        protected DidChangeWatchedFilesCapability Capability { get; private set; }
    }

    public static class DidChangeWatchedFilesExtensions
    {
        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Action<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability, CancellationToken> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Action<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Action<DidChangeWatchedFilesParams, CancellationToken> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Action<DidChangeWatchedFilesParams> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Func<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability, CancellationToken, Task> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Func<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability, Task> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams, DidChangeWatchedFilesCapability,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Func<DidChangeWatchedFilesParams, CancellationToken, Task> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Func<DidChangeWatchedFilesParams, Task> handler,
            DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandler(WorkspaceNames.DidChangeWatchedFiles,
                new LanguageProtocolDelegatingHandlers.Notification<DidChangeWatchedFilesParams,
                    DidChangeWatchedFilesRegistrationOptions>(handler, registrationOptions));
        }

        public static void DidChangeWatchedFiles(this IWorkspaceLanguageClient router, DidChangeWatchedFilesParams @params)
        {
            router.SendNotification(@params);
        }
    }
}

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
    [Serial, Method(WorkspaceNames.DidChangeConfiguration, Direction.ClientToServer)]
    public interface IDidChangeConfigurationHandler : IJsonRpcNotificationHandler<DidChangeConfigurationParams>,
        IRegistration<object>, ICapability<DidChangeConfigurationCapability>
    {
    }

    public abstract class DidChangeConfigurationHandler : IDidChangeConfigurationHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeConfigurationCapability capability) => Capability = capability;
        protected DidChangeConfigurationCapability Capability { get; private set; }
    }

    public static class DidChangeConfigurationExtensions
    {
        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Action<DidChangeConfigurationParams, DidChangeConfigurationCapability, CancellationToken> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeConfigurationParams,
                    DidChangeConfigurationCapability>((r, c, ct) => {
                        handler(r, c, ct);
                        return Task.CompletedTask;
                    }));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Action<DidChangeConfigurationParams, DidChangeConfigurationCapability> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeConfigurationParams,
                    DidChangeConfigurationCapability>(handler));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Action<DidChangeConfigurationParams, CancellationToken> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration, NotificationHandler.For(handler));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Action<DidChangeConfigurationParams> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration, NotificationHandler.For(handler));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Func<DidChangeConfigurationParams, DidChangeConfigurationCapability, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeConfigurationParams,
                    DidChangeConfigurationCapability>(handler));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Func<DidChangeConfigurationParams, DidChangeConfigurationCapability, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration,
                new LanguageProtocolDelegatingHandlers.NotificationCapability<DidChangeConfigurationParams,
                    DidChangeConfigurationCapability>(handler));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Func<DidChangeConfigurationParams, CancellationToken, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration, NotificationHandler.For(handler));
        }

        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Func<DidChangeConfigurationParams, Task> handler)
        {
            return registry.AddHandler(WorkspaceNames.DidChangeConfiguration, NotificationHandler.For(handler));
        }

        public static void DidChangeConfiguration(this IWorkspaceLanguageClient router, DidChangeConfigurationParams @params)
        {
            router.SendNotification(@params);
        }
    }
}

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

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.Implementation, Direction.ClientToServer)]
    public interface IImplementationHandler : IJsonRpcRequestHandler<ImplementationParams, LocationOrLocationLinks>,
        IRegistration<ImplementationRegistrationOptions>, ICapability<ImplementationCapability>
    {
    }

    public abstract class ImplementationHandler :
        AbstractHandlers.Request<ImplementationParams, LocationOrLocationLinks, ImplementationCapability,
            ImplementationRegistrationOptions>, IImplementationHandler
    {
        protected ImplementationHandler(ImplementationRegistrationOptions registrationOptions) : base(
            registrationOptions)
        {
        }
    }

    public static class ImplementationExtensions
    {
        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, ImplementationCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                new LanguageProtocolDelegatingHandlers.Request<ImplementationParams, LocationOrLocationLinks, ImplementationCapability,
                    ImplementationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ImplementationParams, LocationOrLocationLinks,
                    ImplementationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Func<ImplementationParams, Task<LocationOrLocationLinks>> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<ImplementationParams, LocationOrLocationLinks,
                    ImplementationRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<IEnumerable<LocationOrLocationLink>>, ImplementationCapability,
                CancellationToken> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationCapability, ImplementationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<IEnumerable<LocationOrLocationLink>>, ImplementationCapability>
                handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationCapability, ImplementationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

        public static IDisposable OnImplementation(
            this ILanguageServerRegistry registry,
            Action<ImplementationParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler,
            ImplementationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new ImplementationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Implementation,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<ImplementationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, ImplementationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestImplementation(
            this ITextDocumentLanguageClient mediator,
            ImplementationParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new LocationOrLocationLinks(x), cancellationToken);
        }
    }
}

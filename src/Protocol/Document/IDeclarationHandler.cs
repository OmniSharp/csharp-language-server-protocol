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
    [Parallel, Method(TextDocumentNames.Declaration, Direction.ClientToServer)]
    public interface IDeclarationHandler : IJsonRpcRequestHandler<DeclarationParams, LocationOrLocationLinks>, IRegistration<DeclarationRegistrationOptions>, ICapability<DeclarationCapability> { }

    public abstract class DeclarationHandler : IDeclarationHandler
    {
        private readonly DeclarationRegistrationOptions _options;
        public DeclarationHandler(DeclarationRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DeclarationRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(DeclarationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DeclarationCapability capability) => Capability = capability;
        protected DeclarationCapability Capability { get; private set; }
    }

    public static class DeclarationExtensions
    {
public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Func<DeclarationParams, DeclarationCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.Request<DeclarationParams, LocationOrLocationLinks, DeclarationCapability,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Func<DeclarationParams, DeclarationCapability, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.Request<DeclarationParams, LocationOrLocationLinks, DeclarationCapability,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Func<DeclarationParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DeclarationParams, LocationOrLocationLinks,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Func<DeclarationParams, Task<LocationOrLocationLinks>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<DeclarationParams, LocationOrLocationLinks,
                    DeclarationRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<IEnumerable<LocationOrLocationLink>>, DeclarationCapability,
                CancellationToken> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DeclarationCapability, DeclarationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new LocationOrLocationLinks(x)));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<IEnumerable<LocationOrLocationLink>>, DeclarationCapability>
                handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DeclarationCapability, DeclarationRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(),
                        x => new LocationOrLocationLinks(x)
                        ));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DeclarationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new LocationOrLocationLinks(x)));
        }

public static ILanguageServerRegistry OnDeclaration(this ILanguageServerRegistry registry,
            Action<DeclarationParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler,
            DeclarationRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DeclarationRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.Declaration,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<DeclarationParams, LocationOrLocationLinks,
                        LocationOrLocationLink, DeclarationRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(),
                        x => new LocationOrLocationLinks(x)));
        }

        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestDeclaration(
            this ITextDocumentLanguageClient mediator,
            DeclarationParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new LocationOrLocationLinks(x), cancellationToken);
        }
    }
}

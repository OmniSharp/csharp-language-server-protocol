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
    [Parallel, Method(TextDocumentNames.TypeDefinition, Direction.ClientToServer)]
    public interface ITypeDefinitionHandler : IJsonRpcRequestHandler<TypeDefinitionParams, LocationOrLocationLinks>, IRegistration<TypeDefinitionRegistrationOptions>, ICapability<TypeDefinitionCapability> { }

    public abstract class TypeDefinitionHandler : ITypeDefinitionHandler
    {
        private readonly TypeDefinitionRegistrationOptions _options;
        public TypeDefinitionHandler(TypeDefinitionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public TypeDefinitionRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<LocationOrLocationLinks> Handle(TypeDefinitionParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(TypeDefinitionCapability capability) => Capability = capability;
        protected TypeDefinitionCapability Capability { get; private set; }
    }

    public static class TypeDefinitionExtensions
    {
public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, TypeDefinitionCapability, CancellationToken, Task<LocationOrLocationLinks>>
                handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                new LanguageProtocolDelegatingHandlers.Request<TypeDefinitionParams, LocationOrLocationLinks, TypeDefinitionCapability,
                    TypeDefinitionRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, CancellationToken, Task<LocationOrLocationLinks>> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<TypeDefinitionParams, LocationOrLocationLinks,
                    TypeDefinitionRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Func<TypeDefinitionParams, Task<LocationOrLocationLinks>> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<TypeDefinitionParams, LocationOrLocationLinks,
                    TypeDefinitionRegistrationOptions>(handler, registrationOptions));
        }

public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, TypeDefinitionCapability,
                CancellationToken> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionCapability, TypeDefinitionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, TypeDefinitionCapability>
                handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionCapability, TypeDefinitionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>, CancellationToken> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

public static ILanguageServerRegistry OnTypeDefinition(this ILanguageServerRegistry registry,
            Action<TypeDefinitionParams, IObserver<IEnumerable<LocationOrLocationLink>>> handler,
            TypeDefinitionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new TypeDefinitionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.TypeDefinition,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<TypeDefinitionParams, LocationOrLocationLinks,
                        LocationOrLocationLink, TypeDefinitionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new LocationOrLocationLinks(x)));
        }

        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestTypeDefinition(
            this ITextDocumentLanguageClient mediator,
            TypeDefinitionParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new LocationOrLocationLinks(x), cancellationToken);
        }
    }
}

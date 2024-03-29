﻿//HintName: IDefinitionHandler.cs
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
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute, Obsolete("This is obsolete")]
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

        public static IRequestProgressObservable<IEnumerable<LocationOrLocationLink>, LocationOrLocationLinks> RequestDefinition(this ILanguageClient mediator, DefinitionParams request, CancellationToken cancellationToken = default) => mediator.ProgressManager.MonitorUntil(request, LocationOrLocationLinks.From, cancellationToken);
    }
#nullable restore
}
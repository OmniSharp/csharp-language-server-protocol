﻿//HintName: RenameParams.cs
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
}
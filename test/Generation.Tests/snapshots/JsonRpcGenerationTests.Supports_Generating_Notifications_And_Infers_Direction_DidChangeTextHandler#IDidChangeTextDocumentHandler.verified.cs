﻿//HintName: IDidChangeTextDocumentHandler.cs
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
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Test;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
#nullable enable
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public static partial class DidChangeTextDocumentExtensions
    {
        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>(HandlerAdapter<TextSynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<TextSynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, Task> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>(HandlerAdapter<TextSynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<TextSynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams, CancellationToken> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>(HandlerAdapter<TextSynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<TextSynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, CancellationToken, Task> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>(HandlerAdapter<TextSynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<TextSynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Action<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>(HandlerAdapter<TextSynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<TextSynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static ILanguageServerRegistry OnDidChangeTextDocument(this ILanguageServerRegistry registry, Func<DidChangeTextDocumentParams, TextSynchronizationCapability, CancellationToken, Task> handler, RegistrationOptionsDelegate<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability> registrationOptions)
        {
            return registry.AddHandler(TextDocumentNames.DidChange, new LanguageProtocolDelegatingHandlers.Notification<DidChangeTextDocumentParams, TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>(HandlerAdapter<TextSynchronizationCapability>.Adapt<DidChangeTextDocumentParams>(handler), RegistrationAdapter<TextSynchronizationCapability>.Adapt<TextDocumentChangeRegistrationOptions>(registrationOptions)));
        }

        public static void DidChangeTextDocument(this ILanguageClient mediator, DidChangeTextDocumentParams request) => mediator.SendNotification(request);
    }
#nullable restore
}
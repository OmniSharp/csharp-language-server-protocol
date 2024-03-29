﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public abstract class LanguageProtocolRpcOptionsBase<T> : JsonRpcServerOptionsBase<T> where T : IJsonRpcHandlerRegistry<T>
    {
        public LanguageProtocolRpcOptionsBase()
        {
            WithAssemblies(typeof(LanguageProtocolRpcOptionsBase<>).Assembly);
        }

        public T AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            foreach (var item in handlers)
            {
                Services.AddSingleton(typeof(ITextDocumentIdentifier), item);
            }

            return (T)(object)this;
        }

        public T AddTextDocumentIdentifier<TI>() where TI : ITextDocumentIdentifier
        {
            Services.AddSingleton(typeof(ITextDocumentIdentifier), typeof(TI));
            return (T)(object)this;
        }

        public T AddNotebookDocumentIdentifier(params INotebookDocumentIdentifier[] handlers)
        {
            foreach (var item in handlers)
            {
                Services.AddSingleton(typeof(INotebookDocumentIdentifier), item);
            }

            return (T)(object)this;
        }

        public T AddNotebookDocumentIdentifier<TI>() where TI : INotebookDocumentIdentifier
        {
            Services.AddSingleton(typeof(INotebookDocumentIdentifier), typeof(TI));
            return (T)(object)this;
        }

        public LspSerializer Serializer { get; set; } = new LspSerializer(ClientVersion.Lsp3);
        public ConfigurationBuilder ConfigurationBuilder { get; set; } = new ConfigurationBuilder();
        internal bool AddDefaultLoggingProvider { get; set; }
        internal Action<ILoggingBuilder>? LoggingBuilderAction { get; set; } = _ => { };
        internal Action<IConfigurationBuilder>? ConfigurationBuilderAction { get; set; } = _ => { };
    }
}

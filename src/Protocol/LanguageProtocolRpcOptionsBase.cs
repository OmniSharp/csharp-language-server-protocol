using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

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

            return (T) (object) this;
        }

        public T AddTextDocumentIdentifier<TI>() where TI : ITextDocumentIdentifier
        {
            Services.AddSingleton(typeof(ITextDocumentIdentifier), typeof(TI));
            return (T) (object) this;
        }

        public ISerializer Serializer { get; set; } = new Serializer(ClientVersion.Lsp3);
        internal bool AddDefaultLoggingProvider { get; set; }
        internal Action<ILoggingBuilder> LoggingBuilderAction { get; set; } = _ => { };
        internal Action<IConfigurationBuilder> ConfigurationBuilderAction { get; set; } = _ => { };
    }
}

using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClientRegistry : IJsonRpcHandlerRegistry<ILanguageClientRegistry>
    {
    }
}

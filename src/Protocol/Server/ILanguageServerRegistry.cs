using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerRegistry : IJsonRpcHandlerRegistry<ILanguageServerRegistry>
    {
    }
}

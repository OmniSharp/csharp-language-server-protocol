using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol
{
    public interface ILanguageServer : IResponseRouter
    {
        IDisposable AddHandler(IJsonRpcHandler handler);

        InitializeParams Client { get; }
        InitializeResult Server { get; }
    }
}
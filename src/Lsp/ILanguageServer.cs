using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer
{
    public interface ILanguageServer : IResponseRouter
    {
        IDisposable AddHandler(IJsonRpcHandler handler);

        InitializeParams Client { get; }
        InitializeResult Server { get; }
    }
}
using System;
using JsonRpc;
using Lsp.Models;

namespace Lsp
{
    public interface ILanguageServer : IResponseRouter
    {
        IDisposable AddHandler(IJsonRpcHandler handler);

        InitializeParams Client { get; }
        InitializeResult Server { get; }
    }
}
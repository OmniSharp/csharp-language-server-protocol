using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Serial, Method(WindowNames.ShowMessageRequest)]
    public interface IShowMessageRequestHandler : IJsonRpcRequestHandler<ShowMessageRequestParams, MessageActionItem> { }
}

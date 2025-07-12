using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ILanguageProtocolProxy : IResponseRouter, IServiceProvider
    {
        IProgressManager ProgressManager { get; }
        InitializeParams ClientSettings { get; }
        InitializeResult ServerSettings { get; }
    }
}

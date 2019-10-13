using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerWindow : IResponseRouter
    {
        ILanguageServerWindowProgress Progress { get; }
    }
    public interface ILanguageServerWindowProgress : IResponseRouter { }
}

using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClientWindow : IResponseRouter
    {
        ILanguageClientWindowProgress Progress { get; }
    }
    public interface ILanguageClientWindowProgress : IResponseRouter { }
}

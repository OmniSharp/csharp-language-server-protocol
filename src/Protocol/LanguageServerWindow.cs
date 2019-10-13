using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class LanguageServerWindow : ClientProxyBase, ILanguageServerWindow
    {
        public LanguageServerWindow(IResponseRouter responseRouter) : base(responseRouter)
        {
            Progress = new LanguageServerWindowProgress(responseRouter);
        }

        public ILanguageServerWindowProgress Progress { get; }
    }
}

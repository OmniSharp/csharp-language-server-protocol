using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public interface ILspClientReceiver : IReceiver
    {
        void Initialized();
    }
}

using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILspServerReceiver : IReceiver
    {
        void Initialized();
    }
}

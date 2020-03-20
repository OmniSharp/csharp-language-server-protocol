using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILspReceiver : IReceiver
    {
        void Initialized();
    }
}
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILspReciever : IReciever
    {
        void Initialized();
    }
}
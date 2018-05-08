using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface ILspRequestRouter : IRequestRouter
    {
        void CancelRequest(object id);
    }
}
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.LanguageServer
{
    class RequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(Renor renor)
        {
            // TODO: Update to it infer based on incoming messages
            return RequestProcessType.Serial;
        }
    }
}
using JsonRpc;
using JsonRpc.Server;

namespace Lsp
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
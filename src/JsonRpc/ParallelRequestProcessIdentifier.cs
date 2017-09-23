using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public class ParallelRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(Renor renor)
        {
            return RequestProcessType.Parallel;
        }
    }
}
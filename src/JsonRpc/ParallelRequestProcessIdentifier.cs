using JsonRpc.Server;

namespace JsonRpc
{
    public class ParallelRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(Renor renor)
        {
            return RequestProcessType.Parallel;
        }
    }
}
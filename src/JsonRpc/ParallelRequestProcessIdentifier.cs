namespace OmniSharp.Extensions.JsonRpc
{
    public class ParallelRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(IHandlerDescriptor descriptor)
        {
            return RequestProcessType.Parallel;
        }
    }
}

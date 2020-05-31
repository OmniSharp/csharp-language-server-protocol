namespace OmniSharp.Extensions.JsonRpc
{
    public class ParallelRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(IHandlerDescriptor descriptor)
        {
            return descriptor.RequestProcessType ?? RequestProcessType.Parallel;
        }
    }
}

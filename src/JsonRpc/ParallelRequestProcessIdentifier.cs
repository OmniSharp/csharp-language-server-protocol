namespace OmniSharp.Extensions.JsonRpc
{
    public class ParallelRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(IHandlerDescriptor descriptor) => descriptor.RequestProcessType ?? RequestProcessType.Parallel;
    }
}

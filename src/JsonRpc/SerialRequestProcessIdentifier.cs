namespace OmniSharp.Extensions.JsonRpc
{
    public class SerialRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(IHandlerDescriptor descriptor)
        {
            return descriptor.RequestProcessType ?? RequestProcessType.Serial;
        }
    }
}

namespace OmniSharp.Extensions.JsonRpc
{
    public class SerialRequestProcessIdentifier : IRequestProcessIdentifier
    {
        public RequestProcessType Identify(IHandlerDescriptor descriptor) => descriptor.RequestProcessType ?? RequestProcessType.Serial;
    }
}

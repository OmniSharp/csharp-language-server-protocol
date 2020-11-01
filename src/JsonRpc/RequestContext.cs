namespace OmniSharp.Extensions.JsonRpc
{
    internal class RequestContext : IRequestContext
    {
        public IHandlerDescriptor Descriptor { get; set; } = null!;
    }
}

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestContext
    {
        IHandlerDescriptor Descriptor { get; set; }
    }
}

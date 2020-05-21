using System.Threading;

namespace OmniSharp.Extensions.JsonRpc
{
    class RequestContext : IRequestContext
    {
        public IHandlerDescriptor Descriptor { get; set; }
    }
}

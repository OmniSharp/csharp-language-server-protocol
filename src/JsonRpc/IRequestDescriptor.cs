using System.Collections.Generic;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestDescriptor<out TDescriptor> : IEnumerable<TDescriptor>
    {
        TDescriptor Default { get; }
    }
}

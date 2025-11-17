using System.Diagnostics.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IRequestDescriptor<out TDescriptor> : IEnumerable<TDescriptor>
    {
        [MaybeNull]
        TDescriptor Default { get; }
    }
}

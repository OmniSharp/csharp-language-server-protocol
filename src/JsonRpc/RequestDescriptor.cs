using System.Collections;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class RequestDescriptor<TDescriptor> : IRequestDescriptor<TDescriptor>
    {
        private readonly IEnumerable<TDescriptor> _descriptors;

        public RequestDescriptor(IEnumerable<TDescriptor> descriptors)
        {
            var enumerable = descriptors as TDescriptor[] ?? descriptors.ToArray();
            _descriptors = enumerable;
            Default = enumerable.FirstOrDefault();
        }

        public RequestDescriptor(params TDescriptor[] descriptors)
        {
            var enumerable = descriptors.ToArray();
            _descriptors = enumerable;
            Default = enumerable.FirstOrDefault();
        }

        public IEnumerator<TDescriptor> GetEnumerator() => _descriptors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable) _descriptors ).GetEnumerator();

        public TDescriptor Default { get; }
    }
}

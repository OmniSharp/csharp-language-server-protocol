using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingNotification<T> : IRequest
    {
        public DelegatingNotification(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}

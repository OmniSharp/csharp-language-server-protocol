using System;
using System.Text.Json;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequest<T> : IRequest<Memory<byte>>, IRequest
    {
        public DelegatingRequest(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}

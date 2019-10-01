using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class EmptyRequest : IRequest
    {
        private EmptyRequest() { }
        public static EmptyRequest Instance { get; } = new EmptyRequest();
    }
}

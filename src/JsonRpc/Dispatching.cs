using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public readonly struct Unit : System.IEquatable<Unit>, System.IComparable<Unit>
    {
        public static readonly Unit Value;
        public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);

        public int CompareTo(Unit other) => 0;
        public bool Equals(Unit other) => true;
        public override bool Equals(object? obj) => obj is Unit;
        public override int GetHashCode() => 0;
        public override string ToString() => "()";
    }

    public interface IRequest<out TResponse>
    {
    }

    public interface IRequest : IRequest<Unit>
    {
    }

    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }

    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken);

    public interface IPipelineBehavior<in TRequest, TResponse>
        where TRequest : notnull
    {
        Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }
}
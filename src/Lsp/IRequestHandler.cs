using JsonRpc;

namespace Lsp
{
    public interface IRegistrableRequestHandler<TRequest, TResponse, TRegistration> : IRequestHandler<TRequest, TResponse>, IRegistration<TRegistration> { }
    public interface IRegistrableRequestHandler<TRequest, TRegistration> : IRequestHandler<TRequest>, IRegistration<TRegistration> { }
}
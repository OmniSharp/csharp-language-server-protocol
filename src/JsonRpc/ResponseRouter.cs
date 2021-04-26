using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc
{
    internal class ResponseRouter : IResponseRouter
    {
        private readonly IHandlerTypeDescriptorProvider<IHandlerTypeDescriptor?> _handlerTypeDescriptorProvider;
        private readonly StreamJsonRpc.JsonRpc _rpc;

        public ResponseRouter(IHandlerTypeDescriptorProvider<IHandlerTypeDescriptor?> handlerTypeDescriptorProvider, StreamJsonRpc.JsonRpc rpc)
        {
            _handlerTypeDescriptorProvider = handlerTypeDescriptorProvider;
            _rpc = rpc;
        }

        public Task SendNotification(string method) => _rpc.NotifyAsync(method);

        public Task SendNotification<T>(string method, T @params) => _rpc.NotifyWithParameterObjectAsync(method, @params);

        public Task SendNotification(IRequest @params) => SendNotification(GetMethodName(@params.GetType()), @params);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken) =>
            SendRequest(GetMethodName(@params.GetType()), @params).Returning<TResponse>(cancellationToken);

        public IResponseRouterReturns SendRequest(string method) => new ResponseRouterReturnsImpl(_rpc, method, null);

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => new ResponseRouterReturnsImpl(_rpc, method, @params);

        private string GetMethodName(Type type) =>
            _handlerTypeDescriptorProvider.GetMethodName(type) ?? throw new NotSupportedException($"Unable to infer method name for type {type.FullName}");

        private class ResponseRouterReturnsImpl : IResponseRouterReturns
        {
            private readonly StreamJsonRpc.JsonRpc _rpc;
            private readonly string _method;
            private readonly object? _params;

            public ResponseRouterReturnsImpl(StreamJsonRpc.JsonRpc rpc, string method, object? @params)
            {
                _rpc = rpc;
                _method = method;
                _params = @params;
            }

            public Task<TResponse> Returning<TResponse>(CancellationToken cancellationToken) =>
                // TODO: Track request for content modified cancellation?
                _rpc.InvokeWithParameterObjectAsync<TResponse>(_method, _params, cancellationToken);

            public async Task ReturningVoid(CancellationToken cancellationToken) =>
                // TODO: Track request for content modified cancellation?
                _rpc.InvokeWithParameterObjectAsync(_method, _params, cancellationToken);
        }
    }
}

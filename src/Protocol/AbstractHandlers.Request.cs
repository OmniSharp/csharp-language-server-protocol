using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class AbstractHandlers
    {

        public abstract class Request<TParams, TResult> :
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Request<TParams, TResult, TRegistrationOptions> :
            Base<TRegistrationOptions>,
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class Request<TParams, TResult, TRegistrationOptions, TCapability> :
            Base<TRegistrationOptions, TCapability>,
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }

        public abstract class RequestCapability<TParams, TResult, TCapability> :
            BaseCapability<TCapability>,
            IJsonRpcRequestHandler<TParams, TResult>
            where TParams : IRequest<TResult>
            where TCapability : ICapability
        {
            public abstract Task<TResult> Handle(TParams request, CancellationToken cancellationToken);
        }
    }
}

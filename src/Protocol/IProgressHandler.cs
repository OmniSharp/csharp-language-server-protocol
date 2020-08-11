using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [Parallel]
    [Method(GeneralNames.Progress, Direction.Bidirectional)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IGeneralLanguageClient), typeof(ILanguageClient), typeof(IGeneralLanguageServer), typeof(ILanguageServer))]
    public interface IProgressHandler : IJsonRpcNotificationHandler<ProgressParams>
    {
    }

    public abstract class ProgressHandler : IProgressHandler
    {
        public abstract Task<Unit> Handle(ProgressParams request, CancellationToken cancellationToken);
    }

    public static partial class ProgressExtensions
    {
        public static IRequestProgressObservable<TItem, TResponse> RequestProgress<TResponse, TItem>(
            this ILanguageProtocolProxy requestRouter, IPartialItemRequest<TResponse, TItem> @params, Func<TItem, TResponse> factory, CancellationToken cancellationToken = default
        )
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }

        public static IRequestProgressObservable<IEnumerable<TItem>, TResponse> RequestProgress<TResponse, TItem>(
            this ILanguageProtocolProxy requestRouter, IPartialItemsRequest<TResponse, TItem> @params, Func<IEnumerable<TItem>, TResponse> factory,
            CancellationToken cancellationToken = default
        )
            where TResponse : IEnumerable<TItem>
        {
            var resultToken = new ProgressToken(Guid.NewGuid().ToString());
            @params.PartialResultToken = resultToken;

            return requestRouter.ProgressManager.MonitorUntil(@params, factory, cancellationToken);
        }
    }
}

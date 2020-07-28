using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{

    public interface IClientProgressManager
    {
        IProgressObservable Monitor(ProgressToken progressToken);
    }

    public interface IServerProgressManager
    {
        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent> onError = null, Func<ProgressEndEvent> onComplete = null);
    }

    public interface IProgressObserver : IObserver<ProgressUpdateEvent>, IDisposable
    {
        ProgressToken ProgressId { get; }
    }

    public interface IProgressObservable : IObservable<ProgressEvent>
    {

    }

    public class ServerProgressManager : IServerProgressManager
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;
        private readonly ConcurrentDictionary<ProgressToken, IProgressObserver> _activeObservers = new ConcurrentDictionary<ProgressToken, IProgressObserver>(EqualityComparer<ProgressToken>.Default);
        private readonly ConcurrentDictionary<ProgressToken, IProgressObservable> _activeObservables = new ConcurrentDictionary<ProgressToken, IProgressObservable>(EqualityComparer<ProgressToken>.Default);

        public ServerProgressManager(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
        }

        public IProgressObserver Create(ProgressStartEvent begin, Func<Exception, ProgressEndEvent> onError = null, Func<ProgressEndEvent> onComplete = null)
        {

        }
    }

    public class ClientProgressManager : IProgressHandler, IClientProgressManager
    {
        private readonly IResponseRouter _router;
        private readonly ISerializer _serializer;
        private readonly ConcurrentDictionary<ProgressToken, IProgressObserver> _activeObservers = new ConcurrentDictionary<ProgressToken, IProgressObserver>(EqualityComparer<ProgressToken>.Default);
        private readonly ConcurrentDictionary<ProgressToken, IProgressObservable> _activeObservables = new ConcurrentDictionary<ProgressToken, IProgressObservable>(EqualityComparer<ProgressToken>.Default);

        public ClientProgressManager(IResponseRouter router, ISerializer serializer)
        {
            _router = router;
            _serializer = serializer;
        }

        Task<Unit> IRequestHandler<ProgressStartEvent, Unit>.Handle(ProgressStartEvent request, CancellationToken cancellationToken)
        {
            if (_activeObservables.TryGetValue(request.ProgressId, out var observable) && observable is IObserver<ProgressStartEvent> observer)
            {
                observer.OnNext(request);
            }

            // TODO: Add log message for unhandled?
            return Unit.Task;
        }

        Task<Unit> IRequestHandler<ProgressUpdateEvent, Unit>.Handle(ProgressUpdateEvent request, CancellationToken cancellationToken)
        {
            if (_activeObservables.TryGetValue(request.ProgressId, out var observable) && observable is IObserver<ProgressUpdateEvent> observer)
            {
                observer.OnNext(request);
            }

            // TODO: Add log message for unhandled?
            return Unit.Task;
        }

        Task<Unit> IRequestHandler<ProgressEndEvent, Unit>.Handle(ProgressEndEvent request, CancellationToken cancellationToken)
        {
            if (_activeObservables.TryGetValue(request.ProgressId, out var observable) && observable is IObserver<ProgressEndEvent> observer)
            {
                observer.OnNext(request);
            }

            // TODO: Add log message for unhandled?
            return Unit.Task;
        }

        public IProgressObservable Monitor(ProgressToken progressToken)
        {
            
        }
    }


    public class DapReceiver : IReceiver
    {
        private bool _initialized;

        public (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            var result = GetRenor(container);
            return (new[] { result }, result.IsResponse);
        }

        public bool IsValid(JToken container)
        {
            if (container is JObject)
            {
                return true;
            }

            return false;
        }

        protected virtual Renor GetRenor(JToken @object)
        {
            if (!( @object is JObject request ))
            {
                return new InvalidRequest(null, "Not an object");
            }

            if (!request.TryGetValue("seq", out var id))
            {
                return new InvalidRequest(null, "No sequence given");
            }

            if (!request.TryGetValue("type", out var type))
            {
                return new InvalidRequest(null, "No type given");
            }
            var sequence = id.Value<long>();
            var messageType = type.Value<string>();

            if (messageType == "event")
            {
                if (!request.TryGetValue("event", out var @event))
                {
                    return new InvalidRequest(null, "No event given");
                }
                return new Notification(@event.Value<string>(), request.TryGetValue("body", out var body) ? body : null);
            }
            if (messageType == "request")
            {
                if (!request.TryGetValue("command", out var command))
                {
                    return new InvalidRequest(null, "No command given");
                }
                return new Request(sequence, command.Value<string>(), request.TryGetValue("arguments", out var body) ? body : new JObject());
            }
            if (messageType == "response")
            {
                if (!request.TryGetValue("request_seq", out var request_seq))
                {
                    return new InvalidRequest(null, "No request_seq given");
                }
                if (!request.TryGetValue("command", out var command))
                {
                    return new InvalidRequest(null, "No command given");
                }
                if (!request.TryGetValue("success", out var success))
                {
                    return new InvalidRequest(null, "No success given");
                }

                var bodyValue = request.TryGetValue("body", out var body) ? body : null;

                var requestSequence = request_seq.Value<long>();
                var successValue = success.Value<bool>();

                if (successValue)
                {
                    return new ServerResponse(requestSequence, bodyValue);
                }
                return new ServerError(requestSequence, bodyValue.ToObject<ServerErrorResult>());
            }

            throw new NotSupportedException($"Message type {messageType} is not supported");
        }

        public void Initialized()
        {
            _initialized = true;
        }

        public bool ShouldFilterOutput(object value)
        {
            if (_initialized) return true;
            return value is OutgoingResponse ||
                   (value is OutgoingNotification n && (n.Params is InitializedEvent)) ||
                   (value is OutgoingRequest r && r.Params is InitializeRequestArguments);
        }
    }
}

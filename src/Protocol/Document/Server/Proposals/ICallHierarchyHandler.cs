using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Server.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.PrepareCallHierarchy)]
    public interface ICallHierarchyHandler :
        IJsonRpcRequestHandler<CallHierarchyPrepareParams, Container<CallHierarchyItem>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.CallHierarchyIncoming)]
    public interface ICallHierarchyIncomingHandler : IJsonRpcRequestHandler<CallHierarchyIncomingCallsParams,
        Container<CallHierarchyIncomingCall>>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(DocumentNames.CallHierarchyOutgoing)]
    public interface ICallHierarchyOutgoingHandler : IJsonRpcRequestHandler<CallHierarchyOutgoingCallsParams,
        Container<CallHierarchyOutgoingCall>>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class CallHierarchyHandler : ICallHierarchyHandler, ICallHierarchyIncomingHandler,
        ICallHierarchyOutgoingHandler
    {
        private readonly CallHierarchyRegistrationOptions _options;
        protected ProgressManager ProgressManager { get; }

        public CallHierarchyHandler(CallHierarchyRegistrationOptions registrationOptions,
            ProgressManager progressManager)
        {
            _options = registrationOptions;
            ProgressManager = progressManager;
        }

        public CallHierarchyRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<Container<CallHierarchyItem>> Handle(CallHierarchyPrepareParams request,
            CancellationToken cancellationToken);

        public abstract Task<Container<CallHierarchyIncomingCall>> Handle(CallHierarchyIncomingCallsParams request,
            CancellationToken cancellationToken);

        public abstract Task<Container<CallHierarchyOutgoingCall>> Handle(CallHierarchyOutgoingCallsParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(CallHierarchyCapability capability) => Capability = capability;
        protected CallHierarchyCapability Capability { get; private set; }
    }

    [Obsolete(Constants.Proposal)]
    public static class CallHierarchyHandlerExtensions
    {
        public static IDisposable OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
            Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions = null,
            Action<CallHierarchyCapability> setCapability = null)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, incomingHandler, outgoingHandler,
                registry.ProgressManager,
                setCapability, registrationOptions));
        }

        class DelegatingHandler : CallHierarchyHandler
        {
            private readonly Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>>
                _handler;

            private readonly
                Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                _incomingHandler;

            private readonly
                Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                _outgoingHandler;

            private readonly Action<CallHierarchyCapability> _setCapability;
            private CallHierarchyHandler _callHierarchyHandlerImplementation;

            public DelegatingHandler(
                Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
                Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>>
                    incomingHandler,
                Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>>
                    outgoingHandler,
                ProgressManager progressManager,
                Action<CallHierarchyCapability> setCapability,
                CallHierarchyRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _incomingHandler = incomingHandler;
                _outgoingHandler = outgoingHandler;
                _setCapability = setCapability;
            }

            public override Task<Container<CallHierarchyItem>> Handle(CallHierarchyPrepareParams request,
                CancellationToken cancellationToken) => _handler(request, cancellationToken);

            public override Task<Container<CallHierarchyIncomingCall>> Handle(CallHierarchyIncomingCallsParams request,
                CancellationToken cancellationToken) => _incomingHandler(request, cancellationToken);

            public override Task<Container<CallHierarchyOutgoingCall>> Handle(CallHierarchyOutgoingCallsParams request,
                CancellationToken cancellationToken) => _outgoingHandler(request, cancellationToken);

            public override void SetCapability(CallHierarchyCapability capability) =>
                _setCapability?.Invoke(capability);
        }
    }
}

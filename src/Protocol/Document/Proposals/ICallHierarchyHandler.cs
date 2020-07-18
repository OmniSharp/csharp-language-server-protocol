using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.PrepareCallHierarchy, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICallHierarchyHandler :
        IJsonRpcRequestHandler<CallHierarchyPrepareParams, Container<CallHierarchyItem>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.CallHierarchyIncoming, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICallHierarchyIncomingHandler : IJsonRpcRequestHandler<CallHierarchyIncomingCallsParams,
            Container<CallHierarchyIncomingCall>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    [Parallel, Method(TextDocumentNames.CallHierarchyOutgoing, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface ICallHierarchyOutgoingHandler : IJsonRpcRequestHandler<CallHierarchyOutgoingCallsParams,
            Container<CallHierarchyOutgoingCall>>,
        IRegistration<CallHierarchyRegistrationOptions>, ICapability<CallHierarchyCapability>
    {
    }

    [Obsolete(Constants.Proposal)]
    public abstract class CallHierarchyHandler : ICallHierarchyHandler, ICallHierarchyIncomingHandler,
        ICallHierarchyOutgoingHandler
    {
        private readonly CallHierarchyRegistrationOptions _options;

        public CallHierarchyHandler(CallHierarchyRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
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
    public static partial class CallHierarchyExtensions
    {
        public static ILanguageServerRegistry OnCallHierarchy(
            this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
            Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyIncomingCall>>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, Task<Container<CallHierarchyItem>>> handler,
            Func<CallHierarchyIncomingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyIncomingCall>>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CallHierarchyCapability, Task<Container<CallHierarchyOutgoingCall>>> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
            Func<CallHierarchyIncomingCallsParams, CancellationToken, Task<Container<CallHierarchyIncomingCall>>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, CancellationToken, Task<Container<CallHierarchyOutgoingCall>>> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, Task<Container<CallHierarchyItem>>> handler,
            Func<CallHierarchyIncomingCallsParams, Task<Container<CallHierarchyIncomingCall>>> incomingHandler,
            Func<CallHierarchyOutgoingCallsParams, Task<Container<CallHierarchyOutgoingCall>>> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability, CancellationToken> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability, CancellationToken> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>, CallHierarchyIncomingCall,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>, CallHierarchyOutgoingCall,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CallHierarchyCapability, Task<Container<CallHierarchyItem>>> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CallHierarchyCapability> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CallHierarchyCapability> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.Request<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>, CallHierarchyIncomingCall,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>, CallHierarchyOutgoingCall,
                            CallHierarchyCapability,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, CancellationToken, Task<Container<CallHierarchyItem>>> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>, CancellationToken> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>, CancellationToken> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>, CallHierarchyIncomingCall,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>, CallHierarchyOutgoingCall,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)))
                ;
        }

        public static ILanguageServerRegistry OnCallHierarchy(this ILanguageServerRegistry registry,
            Func<CallHierarchyPrepareParams, Task<Container<CallHierarchyItem>>> handler,
            Action<CallHierarchyIncomingCallsParams, IObserver<IEnumerable<CallHierarchyIncomingCall>>> incomingHandler,
            Action<CallHierarchyOutgoingCallsParams, IObserver<IEnumerable<CallHierarchyOutgoingCall>>> outgoingHandler,
            CallHierarchyRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CallHierarchyRegistrationOptions();
            return
                registry.AddHandler(TextDocumentNames.PrepareCallHierarchy,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<CallHierarchyPrepareParams,
                            Container<CallHierarchyItem>,
                            CallHierarchyRegistrationOptions>(handler, registrationOptions))
                    .AddHandler(TextDocumentNames.CallHierarchyIncoming,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyIncomingCallsParams,
                            Container<CallHierarchyIncomingCall>, CallHierarchyIncomingCall,
                            CallHierarchyRegistrationOptions>(incomingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyIncomingCall>(x)))
                    .AddHandler(TextDocumentNames.CallHierarchyOutgoing,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<CallHierarchyOutgoingCallsParams,
                            Container<CallHierarchyOutgoingCall>, CallHierarchyOutgoingCall,
                            CallHierarchyRegistrationOptions>(outgoingHandler, registrationOptions,
                            _.GetRequiredService<IProgressManager>(), x => new Container<CallHierarchyOutgoingCall>(x)))
                ;
        }
    }
}

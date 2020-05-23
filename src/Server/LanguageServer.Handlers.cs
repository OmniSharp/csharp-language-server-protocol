using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public partial class LanguageServer
    {
        public IDisposable AddHandler(string method, IJsonRpcHandler handler)
        {
            var handlerDisposable = _collection.Add(method, handler);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandler(string method, Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            var handlerDisposable = _collection.Add(method, handlerFunc);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandler(Func<IServiceProvider, IJsonRpcHandler> handlerFunc)
        {
            var instance = handlerFunc(_serviceProvider);
            var handlerDisposable = _collection.Add(HandlerTypeDescriptorHelper.GetMethodName(instance.GetType()), instance);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandlers(params IJsonRpcHandler[] handlers)
        {
            var handlerDisposable = _collection.Add(handlers);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandler(string method, Type handlerType)
        {
            var handlerDisposable = _collection.Add(method, handlerType);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddHandler<T>()
            where T : IJsonRpcHandler
        {
            return AddHandlers(typeof(T));
        }

        public IDisposable AddHandler<T>(Func<IServiceProvider, T> factory)
            where T : IJsonRpcHandler
        {
            return AddHandlers(factory(_serviceProvider));
        }

        public IDisposable AddHandlers(params Type[] handlerTypes)
        {
            var handlerDisposable = _collection.Add(_serviceProvider, handlerTypes);
            return RegisterHandlers(handlerDisposable);
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            var cd = new CompositeDisposable();
            foreach (var textDocumentIdentifier in handlers)
            {
                cd.Add(_textDocumentIdentifiers.Add(textDocumentIdentifier));
            }

            return cd;
        }

        public IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier
        {
            return _textDocumentIdentifiers.Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
        }

        private IDisposable RegisterHandlers(LspHandlerDescriptorDisposable handlerDisposable)
        {
            var registrations = new List<Registration>();
            foreach (var descriptor in handlerDisposable.Descriptors)
            {
                if (descriptor.AllowsDynamicRegistration)
                {
                    if (descriptor.RegistrationOptions is IWorkDoneProgressOptions wdpo)
                    {
                        wdpo.WorkDoneProgress = _serverWorkDoneManager.IsSupported;
                    }

                    registrations.Add(new Registration() {
                        Id = descriptor.Id.ToString(),
                        Method = descriptor.Method,
                        RegisterOptions = descriptor.RegistrationOptions
                    });
                }

                if (descriptor.OnServerStartedDelegate != null)
                {
                    // Fire and forget to initialize the handler
                    _initializeComplete
                        .Select(result =>
                            Observable.FromAsync((ct) => descriptor.OnServerStartedDelegate(this, result, ct)))
                        .Merge()
                        .Subscribe();
                }
            }

            // Fire and forget
            DynamicallyRegisterHandlers(registrations.ToArray()).ToObservable().Subscribe();

            return new CompositeDisposable(
                handlerDisposable,
                Disposable.Create(() => {
                    Client.UnregisterCapability(new UnregistrationParams() {
                        Unregisterations = registrations.ToArray()
                    }).ToObservable().Subscribe();
                }));
        }
    }
}

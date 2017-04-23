using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using Lsp.Capabilities.Client;
using Lsp.Capabilities.Server;
using Lsp.Handlers;
using Lsp.Models;
using Lsp.Protocol;
using Newtonsoft.Json.Linq;

namespace Lsp
{
    public interface ILanguageServer : IResponseRouter
    {
        IDisposable AddHandler(IJsonRpcHandler handler);
        void RemoveHandler(IJsonRpcHandler handler);

        InitializeParams Client { get; }
        InitializeResult Server { get; }
    }

    public class LanguageServer : IInitializeHandler, ILanguageServer
    {
        private readonly Connection _connection;
        private readonly LspRequestRouter _requestRouter;
        private readonly ShutdownHandler _shutdownHandler = new ShutdownHandler();
        private readonly ExitHandler _exitHandler;
        private ClientVersion? _clientVersion;
        private readonly HandlerCollection _collection = new HandlerCollection();
        private readonly IResponseRouter _responseRouter;

        public LanguageServer(TextReader input, TextWriter output)
        {
            var outputHandler = new OutputHandler(output);
            _requestRouter = new LspRequestRouter(_collection);
            _responseRouter = new ResponseRouter(outputHandler);

            _connection = new Connection(
                input,
                outputHandler,
                new Reciever(),
                new RequestProcessIdentifier(),
                new LspRequestRouter(_collection),
                _responseRouter);

            _exitHandler = new ExitHandler(_shutdownHandler);

            AddHandler(this);
            AddHandler(_shutdownHandler);
            AddHandler(_exitHandler);
            AddHandler(new CancelRequestHandler(_requestRouter));
        }

        internal LanguageServer(
            TextReader input,
            IOutputHandler output,
            IReciever reciever,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter requestRouter,
            IResponseRouter responseRouter
        )
        {
            _connection = new Connection(input, output, reciever, requestProcessIdentifier, requestRouter, responseRouter);
            _requestRouter = new LspRequestRouter(_collection);

            _exitHandler = new ExitHandler(_shutdownHandler);

            AddHandler(this);
            AddHandler(_shutdownHandler);
            AddHandler(_exitHandler);
            AddHandler(new CancelRequestHandler(_requestRouter));
        }

        public InitializeParams Client { get; private set; }
        public InitializeResult Server { get; private set; }

        public IDisposable AddHandler(IJsonRpcHandler handler)
        {
            return _requestRouter.Add(handler);
        }

        public void RemoveHandler(IJsonRpcHandler handler)
        {
            _requestRouter.Remove(handler);
        }

        public async Task Initialize()
        {
            _connection.Open();



            await DynamicallyRegisterHandlers();
        }

        async Task<InitializeResult> IRequestHandler<InitializeParams, InitializeResult>.Handle(InitializeParams request, CancellationToken token)
        {
            Client = request;
            //_tcs.SetResult(request);

            //new ServerCapabilities() {

            //}

            _clientVersion = request.Capabilities.GetClientVersion();

            if (_clientVersion == ClientVersion.Lsp3)
            {
                // handle client capabilites
                if (request.Capabilities.TextDocument != null)
                {
                    ProcessCapabilties(request.Capabilities.TextDocument);
                }

                if (request.Capabilities.Workspace != null)
                {
                    ProcessCapabilties(request.Capabilities.Workspace);
                }
            }

            var serverCapabilities = new ServerCapabilities() {
                CodeActionProvider
            }
        }

        private void ProcessCapabilties(object instance)
        {
            var values = instance
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(x => x.CanRead)
                .Select(x => x.GetValue(instance))
                .OfType<ISupports>();

            foreach (var value in values)
            {
                foreach (var handler in _collection.Where(x => x.HasCapability && x.CapabilityType == value.ValueType))
                {
                    handler.SetCapability(value.Value);
                }
            }
        }

        private async Task DynamicallyRegisterHandlers()
        {
            var registrations = new List<Registration>();
            foreach (var handler in _collection.Where(x => x.AllowsDynamicRegistration))
            {
                registrations.Add(handler.Registration);
            }

            var @params = new RegistrationParams() { Registrations = registrations };

            await this.RegisterCapability(@params);

        }

        public event ShutdownEventHandler Shutdown
        {
            add => _shutdownHandler.Shutdown += value;
            remove => _shutdownHandler.Shutdown -= value;
        }

        public event ExitEventHandler Exit
        {
            add => _exitHandler.Exit += value;
            remove => _exitHandler.Exit -= value;
        }

        public Task SendNotification<T>(string method, T @params)
        {
            return _responseRouter.SendNotification(method, @params);
        }

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
        {
            return _responseRouter.SendRequest<T, TResponse>(method, @params);
        }

        public Task SendRequest<T>(string method, T @params)
        {
            return _responseRouter.SendRequest(method, @params);
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            return _responseRouter.GetRequest(id);
        }
    }
}
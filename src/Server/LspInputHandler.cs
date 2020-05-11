using System.Collections.Generic;
using System.IO.Pipelines;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LspInputHandler : InputHandler
    {
        private bool _initialized;
        public LspInputHandler(
            PipeReader pipeReader,
            IOutputHandler outputHandler,
            IRequestProcessIdentifier requestProcessIdentifier,
            IRequestRouter<IHandlerDescriptor> requestRouter,
            IResponseRouter responseRouter,
            ILoggerFactory loggerFactory,
            ISerializer serializer,
            int? concurrency
        ) : base(
            pipeReader,
            outputHandler,
            requestProcessIdentifier,
            requestRouter,
            responseRouter,
            loggerFactory,
            serializer,
            concurrency)
        {
        }

        protected override void HandleRequest(IHandlerDescriptor descriptor, Request request)
        {
            if (_initialized || request.Method == LspHelper.GetMethodName<IInitializeHandler>())
            {
                base.HandleRequest(descriptor, request);
                return;
            }

            _outputHandler.Send(new ServerNotInitialized());
        }

        public void Initialized()
        {
            _initialized = true;
        }
    }
}

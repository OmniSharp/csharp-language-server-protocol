using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal class LspRequestRouter : RequestRouterBase<ILspHandlerDescriptor>, IRequestRouter<IHandlerDescriptor>
    {
        private readonly IHandlerCollection _collection;
        private readonly IEnumerable<IHandlerMatcher> _handlerMatchers;

        public LspRequestRouter(
            IHandlerCollection collection,
            ILoggerFactory loggerFactory,
            IEnumerable<IHandlerMatcher> handlerMatchers,
            ISerializer serializer,
            IServiceProvider serviceProvider,
            IServiceScopeFactory serviceScopeFactory) :
            base(serializer, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger<LspRequestRouter>())
        {
            _collection = collection;
            _handlerMatchers = handlerMatchers;
        }

        public override ILspHandlerDescriptor GetDescriptor(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public override ILspHandlerDescriptor GetDescriptor(Request request)
        {
            return FindDescriptor(request);
        }

        private ILspHandlerDescriptor FindDescriptor(IMethodWithParams instance)
        {
            return FindDescriptor(instance.Method, instance.Params);
        }

        private ILspHandlerDescriptor FindDescriptor(string method, JToken @params)
        {
            _logger.LogDebug("Finding descriptor for {Method}", method);
            var descriptor = _collection.FirstOrDefault(x => x.Method == method);
            if (descriptor is null)
            {
                _logger.LogDebug("Unable to find {Method}, methods found include {Methods}", method,
                    string.Join(", ", _collection.Select(x => x.Method + ":" + x.Handler?.GetType()?.FullName)));
                return null;
            }


            if (@params == null || descriptor.Params == null) return descriptor;

            var lspHandlerDescriptors = _collection.Where(handler => handler.Method == method).ToList();

            var paramsValue = @params.ToObject(descriptor.Params, _serializer.JsonSerializer);
            var matchDescriptor = _handlerMatchers.SelectMany(strat => strat.FindHandler(paramsValue, lspHandlerDescriptors)).FirstOrDefault();
            if (matchDescriptor != null) return matchDescriptor;
            // execute command is a special case
            // if no command was found to execute this must error
            // this is not great coupling but other options require api changes
            if (paramsValue is ExecuteCommandParams) return null;
            if (lspHandlerDescriptors.Count == 1) return descriptor;
            return null;
        }

        IHandlerDescriptor IRequestRouter<IHandlerDescriptor>.GetDescriptor(Notification notification) => GetDescriptor(notification);
        IHandlerDescriptor IRequestRouter<IHandlerDescriptor>.GetDescriptor(Request request) => GetDescriptor(request);

        Task IRequestRouter<IHandlerDescriptor>.RouteNotification(IHandlerDescriptor descriptor, Notification notification, CancellationToken token) =>
            RouteNotification(
                descriptor is ILspHandlerDescriptor d ? d : throw new Exception("This should really never happen, seriously, only hand this correct descriptors"),
                notification,
                token);

        Task<ErrorResponse> IRequestRouter<IHandlerDescriptor>.RouteRequest(IHandlerDescriptor descriptor, Request request, CancellationToken token) =>
            RouteRequest(
                descriptor is ILspHandlerDescriptor d ? d : throw new Exception("This should really never happen, seriously, only hand this correct descriptors"),
                request,
                token);
    }
}

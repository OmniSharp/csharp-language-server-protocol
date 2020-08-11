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
        private readonly HashSet<IHandlerMatcher> _handlerMatchers;

        public LspRequestRouter(
            IHandlerCollection collection,
            ILogger<LspRequestRouter> logger,
            IEnumerable<IHandlerMatcher> handlerMatchers,
            ISerializer serializer,
            IServiceScopeFactory serviceScopeFactory
        ) :
            base(serializer, serviceScopeFactory, logger)
        {
            _collection = collection;
            _handlerMatchers = new HashSet<IHandlerMatcher>(handlerMatchers);
        }

        public override IRequestDescriptor<ILspHandlerDescriptor> GetDescriptors(Notification notification) => FindDescriptor(notification);

        public override IRequestDescriptor<ILspHandlerDescriptor> GetDescriptors(Request request) => FindDescriptor(request);

        private IRequestDescriptor<ILspHandlerDescriptor> FindDescriptor(IMethodWithParams instance) => FindDescriptor(instance.Method, instance.Params);

        private IRequestDescriptor<ILspHandlerDescriptor> FindDescriptor(string method, JToken @params)
        {
            _logger.LogDebug("Finding descriptors for {Method}", method);
            var descriptor = _collection.FirstOrDefault(x => x.Method == method);
            if (descriptor is null)
            {
                _logger.LogDebug(
                    "Unable to find {Method}, methods found include {Methods}", method,
                    string.Join(", ", _collection.Select(x => x.Method + ":" + x.Handler?.GetType()?.FullName))
                );
                return new RequestDescriptor<ILspHandlerDescriptor>();
            }

            if (@params == null || descriptor.Params == null) return new RequestDescriptor<ILspHandlerDescriptor>(descriptor);

            object paramsValue = null;
            if (descriptor.IsDelegatingHandler)
            {
                var o = @params?.ToObject(descriptor.Params.GetGenericArguments()[0], _serializer.JsonSerializer);
                paramsValue = Activator.CreateInstance(descriptor.Params, o);
            }
            else
            {
                paramsValue = @params?.ToObject(descriptor.Params, _serializer.JsonSerializer);
            }

            var lspHandlerDescriptors = _collection.Where(handler => handler.Method == method).ToList();

            var matchDescriptor = _handlerMatchers.SelectMany(strat => strat.FindHandler(paramsValue, lspHandlerDescriptors)).ToArray();
            if (matchDescriptor.Length > 0) return new RequestDescriptor<ILspHandlerDescriptor>(matchDescriptor);
            // execute command is a special case
            // if no command was found to execute this must error
            // this is not great coupling but other options require api changes
            if (paramsValue is ExecuteCommandParams) return new RequestDescriptor<ILspHandlerDescriptor>();
            if (lspHandlerDescriptors.Count > 0) return new RequestDescriptor<ILspHandlerDescriptor>(lspHandlerDescriptors);
            return new RequestDescriptor<ILspHandlerDescriptor>();
        }

        IRequestDescriptor<IHandlerDescriptor> IRequestRouter<IHandlerDescriptor>.GetDescriptors(Notification notification) => GetDescriptors(notification);
        IRequestDescriptor<IHandlerDescriptor> IRequestRouter<IHandlerDescriptor>.GetDescriptors(Request request) => GetDescriptors(request);

        Task IRequestRouter<IHandlerDescriptor>.RouteNotification(IRequestDescriptor<IHandlerDescriptor> descriptors, Notification notification, CancellationToken token) =>
            RouteNotification(
                descriptors is IRequestDescriptor<ILspHandlerDescriptor> d
                    ? d
                    : throw new Exception("This should really never happen, seriously, only hand this correct descriptors"),
                notification,
                token
            );

        Task<ErrorResponse> IRequestRouter<IHandlerDescriptor>.RouteRequest(IRequestDescriptor<IHandlerDescriptor> descriptors, Request request, CancellationToken token) =>
            RouteRequest(
                descriptors is IRequestDescriptor<ILspHandlerDescriptor> d
                    ? d
                    : throw new Exception("This should really never happen, seriously, only hand this correct descriptors"),
                request,
                token
            );
    }
}

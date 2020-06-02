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
            ILoggerFactory loggerFactory,
            IEnumerable<IHandlerMatcher> handlerMatchers,
            ISerializer serializer,
            IServiceProvider serviceProvider,
            IServiceScopeFactory serviceScopeFactory) :
            base(serializer, serviceProvider, serviceScopeFactory, loggerFactory.CreateLogger<LspRequestRouter>())
        {
            _collection = collection;
            _handlerMatchers = new HashSet<IHandlerMatcher>(handlerMatchers);
        }

        public override IRequestDescriptor<ILspHandlerDescriptor> GetDescriptor(Notification notification)
        {
            return FindDescriptor(notification);
        }

        public override IRequestDescriptor<ILspHandlerDescriptor> GetDescriptor(Request request)
        {
            return FindDescriptor(request);
        }

        private IRequestDescriptor<ILspHandlerDescriptor> FindDescriptor(IMethodWithParams instance)
        {
            return FindDescriptor(instance.Method, instance.Params);
        }

        private IRequestDescriptor<ILspHandlerDescriptor> FindDescriptor(string method, JToken @params)
        {
            _logger.LogDebug("Finding descriptor for {Method}", method);
            var descriptor = _collection.FirstOrDefault(x => x.Method == method);
            if (descriptor is null)
            {
                _logger.LogDebug("Unable to find {Method}, methods found include {Methods}", method,
                    string.Join(", ", _collection.Select(x => x.Method + ":" + x.Handler?.GetType()?.FullName)));
                return null;
            }

            if (@params == null || descriptor.Params == null) return new RequestDescriptor<ILspHandlerDescriptor>(new [] { descriptor });

            var lspHandlerDescriptors = _collection.Where(handler => handler.Method == method).ToList();
            if (lspHandlerDescriptors.Count == 1) return new RequestDescriptor<ILspHandlerDescriptor>(lspHandlerDescriptors);

            var paramsValue = @params.ToObject(descriptor.Params, _serializer.JsonSerializer);
            var matchingDescriptors = _handlerMatchers.SelectMany(strat => strat.FindHandler(paramsValue, lspHandlerDescriptors)).ToArray();
            return matchingDescriptors.Length == 0 ? new RequestDescriptor<ILspHandlerDescriptor>(new [] { descriptor }) : new RequestDescriptor<ILspHandlerDescriptor>(matchingDescriptors);
        }

        IRequestDescriptor<IHandlerDescriptor> IRequestRouter<IHandlerDescriptor>.GetDescriptor(Notification notification) => GetDescriptor(notification);
        IRequestDescriptor<IHandlerDescriptor> IRequestRouter<IHandlerDescriptor>.GetDescriptor(Request request) => GetDescriptor(request);

        Task IRequestRouter<IHandlerDescriptor>.RouteNotification(IRequestDescriptor<IHandlerDescriptor> descriptors, Notification notification, CancellationToken token) =>
            RouteNotification(
                descriptors is IRequestDescriptor<ILspHandlerDescriptor> d ? d : throw new Exception("This should really never happen, seriously, only hand this correct descriptors"),
                notification,
                token);

        Task<ErrorResponse> IRequestRouter<IHandlerDescriptor>.RouteRequest(IRequestDescriptor<IHandlerDescriptor> descriptors, Request request, CancellationToken token) =>
            RouteRequest(
                descriptors is IRequestDescriptor<ILspHandlerDescriptor> d ? d : throw new Exception("This should really never happen, seriously, only hand this correct descriptors"),
                request,
                token);
    }
}

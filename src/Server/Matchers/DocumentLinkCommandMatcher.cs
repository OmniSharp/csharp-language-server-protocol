using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class DocumentLinkCommandMatcher : IHandlerMatcher, IHandlerPostProcessorMatcher, IHandlerPostProcessor
    {
        private readonly ILogger _logger;
        private ILspHandlerDescriptor _descriptor;

        public DocumentLinkCommandMatcher(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Finds the first handler that matches the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns></returns>
        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            if (parameters is DocumentLink)
            {
                _logger.LogTrace("Using handler {Method}:{Handler}",
                    _descriptor.Method,
                    _descriptor.Handler.GetType().FullName);
                yield return _descriptor;
            }
        }

        public IEnumerable<IHandlerPostProcessor> FindPostProcessor(ILspHandlerDescriptor descriptor, object parameters, object response)
        {
            if (descriptor.Method == DocumentNames.DocumentLink)
            {
                _logger.LogTrace("Using handler {Method}:{Handler}",
                    descriptor.Method,
                    descriptor.Handler.GetType().FullName);
                yield return this;
            }
        }

        public object Process(ILspHandlerDescriptor descriptor, object parameters, object response)
        {
            _descriptor = descriptor;
            return response;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class HandlerMatcherCollection : IHandlerMatcherCollection
    {
        internal readonly HashSet<object> _handlers = new HashSet<object>();

        private static readonly Type[] ValidHandlers = {
                typeof(IHandlerMatcher),
                typeof(IHandlerPostProcessor),
                typeof(IHandlerPostProcessorMatcher)
            };

        public IEnumerator<object> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        public IDisposable Add(object handler)
        {
            if (!ValidHandlers.Any(type => type.GetTypeInfo().IsAssignableFrom(handler.GetType())))
                throw new ArgumentException($"Handler must be one of {string.Join(", ", ValidHandlers.Select(x => x.FullName))}");

            _handlers.Add(handler);
            return new Disposable(() => _handlers.Remove(handler));
        }

        public IEnumerable<IHandlerMatcher> ForHandlerMatchers()
        {
            return _handlers.OfType<IHandlerMatcher>();
        }

        public IEnumerable<IHandlerPostProcessorMatcher> ForHandlerPostProcessorMatcher()
        {
            return _handlers.OfType<IHandlerPostProcessorMatcher>();
        }

        public IEnumerable<IHandlerPostProcessor> ForHandlerPostProcessor()
        {
            return _handlers.OfType<IHandlerPostProcessor>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

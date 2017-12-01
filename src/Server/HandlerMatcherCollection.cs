using System;
using System.Collections;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class HandlerMatcherCollection : IHandlerMatcherCollection
    {
        internal readonly HashSet<IHandlerMatcher> _handlers = new HashSet<IHandlerMatcher>();

        public IEnumerator<IHandlerMatcher> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        public IDisposable Add(IHandlerMatcher handler)
        {
            _handlers.Add(handler);
            return new Disposable(() => _handlers.Remove(handler));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

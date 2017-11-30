using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Abstractions;

namespace OmniSharp.Extensions.LanguageServer
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

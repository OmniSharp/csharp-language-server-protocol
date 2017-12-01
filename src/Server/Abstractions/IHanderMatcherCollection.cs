using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IHandlerMatcherCollection : IEnumerable<IHandlerMatcher>
    {
        IDisposable Add(IHandlerMatcher handler);
    }
}

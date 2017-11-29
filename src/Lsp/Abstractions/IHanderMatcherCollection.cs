using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    public interface IHandlerMatcherCollection : IEnumerable<IHandlerMatcher>
    {
        IDisposable Add(IHandlerMatcher handler);
    }
}

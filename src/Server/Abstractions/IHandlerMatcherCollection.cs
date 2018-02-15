using System;
using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IHandlerMatcherCollection : IEnumerable<object>
    {
        IDisposable Add(object handler);
        IEnumerable<IHandlerMatcher> ForHandlerMatchers();
        IEnumerable<IHandlerPreProcessorMatcher> ForHandlerPreProcessorMatcher();
        IEnumerable<IHandlerPreProcessor> ForHandlerPreProcessor();
        IEnumerable<IHandlerPostProcessorMatcher> ForHandlerPostProcessorMatcher();
        IEnumerable<IHandlerPostProcessor> ForHandlerPostProcessor();
    }
}

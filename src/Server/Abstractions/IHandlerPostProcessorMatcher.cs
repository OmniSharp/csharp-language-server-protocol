using System.Collections.Generic;

namespace OmniSharp.Extensions.LanguageServer.Server.Abstractions
{
    public interface IHandlerPostProcessorMatcher
    {
        /// <summary>
        /// Finds any postproessor for a given descriptor and response
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        IEnumerable<IHandlerPostProcessor> FindPostProcessor(ILspHandlerDescriptor descriptor, object parameters, object response);
    }
}

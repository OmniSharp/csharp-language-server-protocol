using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    public interface IHandlerMatcher
    {
        /// <summary>
        /// Finds the first handler that matches the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns></returns>
        IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors);
    }
}

using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server.Matchers
{
    public class ExecuteCommandMatcher : IHandlerMatcher
    {
        private readonly ILogger<ExecuteCommandMatcher> _logger;

        public ExecuteCommandMatcher(ILogger<ExecuteCommandMatcher> logger) => _logger = logger;

        /// <summary>
        /// Finds the first handler that matches the parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="descriptors">The descriptors.</param>
        /// <returns></returns>
        public IEnumerable<ILspHandlerDescriptor> FindHandler(object parameters, IEnumerable<ILspHandlerDescriptor> descriptors)
        {
            if (parameters is IExecuteCommandParams executeCommandParams)
            {
                _logger.LogTrace("Registration options {OptionsName}", executeCommandParams.GetType().FullName);
                foreach (var descriptor in descriptors)
                {
                    if (descriptor.RegistrationOptions is ExecuteCommandRegistrationOptions registrationOptions
                     && registrationOptions.Commands.Any(x => x == executeCommandParams.Command))
                    {
                        _logger.LogTrace(
                            "Checking handler {Method}:{Handler}",
                            executeCommandParams.Command,
                            executeCommandParams.GetType().FullName
                        );
                        yield return descriptor;
                    }
                }
            }
        }
    }
}

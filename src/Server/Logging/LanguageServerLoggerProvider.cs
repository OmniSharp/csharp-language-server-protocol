using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LanguageServerLoggerProvider : ILoggerProvider
    {
        private readonly ILanguageServerFacade _languageServer;
        private readonly LanguageServerLoggerSettings _settings;

        public LanguageServerLoggerProvider(ILanguageServerFacade languageServer, LanguageServerLoggerSettings settings)
        {
            _languageServer = languageServer;
            _settings = settings;
        }

        public ILogger CreateLogger(string categoryName) => new LanguageServerLogger(_languageServer, categoryName, () => _settings.MinimumLogLevel);

        public void Dispose()
        {
        }
    }
}

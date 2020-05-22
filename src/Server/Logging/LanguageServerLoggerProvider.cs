using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class LanguageServerLoggerProvider : ILoggerProvider
    {
        private readonly ILanguageServer _languageServer;
        private readonly LanguageServerLoggerSettings _settings;

        public LanguageServerLoggerProvider(ILanguageServer languageServer, LanguageServerLoggerSettings settings)
        {
            _languageServer = languageServer;
            _settings = settings;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LanguageServerLogger(_languageServer, categoryName, () => _settings.MinimumLogLevel);
        }

        public void Dispose()
        {
        }
    }
}

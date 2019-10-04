using System;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerLoggerProvider : ILoggerProvider
    {
        private readonly LanguageServer _languageServer;

        public LanguageServerLoggerProvider(LanguageServer languageServer)
        {
            _languageServer = languageServer;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LanguageServerLogger(_languageServer, () => _languageServer.MinimumLogLevel);
        }

        public void Dispose()
        {
        }
    }
}

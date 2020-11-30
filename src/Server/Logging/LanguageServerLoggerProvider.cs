using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LanguageServerLoggerProvider : ILoggerProvider
    {
        private readonly ILanguageServerFacade _languageServer;

        public LanguageServerLoggerProvider(ILanguageServerFacade languageServer)
        {
            _languageServer = languageServer;
        }

        public ILogger CreateLogger(string categoryName) => new LanguageServerLogger(_languageServer, categoryName);

        public void Dispose()
        {
        }
    }
}

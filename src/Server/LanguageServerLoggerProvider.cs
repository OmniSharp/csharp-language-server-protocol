using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using OmniSharp.Extensions.LanguageServer.Capabilities.Client;
using OmniSharp.Extensions.LanguageServer.Capabilities.Server;
using OmniSharp.Extensions.LanguageServer.Handlers;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.LanguageServer
{
    class LanguageServerLoggerProvider : ILoggerProvider
    {
        private readonly LanguageServer _languageServer;

        public LanguageServerLoggerProvider(LanguageServer languageServer)
        {
            _languageServer = languageServer;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LanguageServerLogger(_languageServer);
        }

        public void Dispose()
        {
        }
    }
}

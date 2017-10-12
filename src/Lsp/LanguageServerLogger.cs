using System;
using OmniSharp.Extensions.LanguageServer.Models;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer
{
    class LanguageServerLogger : ILogger
    {
        private LanguageServer _languageServer;

        public LanguageServerLogger(LanguageServer languageServer)
        {
            _languageServer = languageServer;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // TODO
            return new ImmutableDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // TODO: setup as configuration somehwhere (from trace perhaps?)
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (TryGetMessageType(logLevel, out var messageType))
            {
                _languageServer.Log(new LogMessageParams()
                {
                    Type = messageType,
                    Message = formatter(state, exception)
                });
            }
        }

        private bool TryGetMessageType(LogLevel logLevel, out MessageType messageType)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    messageType = MessageType.Error;
                    return true;
                case LogLevel.Warning:
                    messageType = MessageType.Warning;
                    return true;
                case LogLevel.Information:
                    messageType = MessageType.Info;
                    return true;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    messageType = MessageType.Info;
                    return true;
            }
            messageType = MessageType.Log;
            return false;
        }
    }
}

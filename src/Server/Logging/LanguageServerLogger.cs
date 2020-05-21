using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class LanguageServerLogger : ILogger
    {
        private readonly ILanguageServer _responseRouter;
        private readonly string _categoryName;
        private readonly Func<LogLevel> _logLevelGetter;

        public LanguageServerLogger(ILanguageServer responseRouter, string categoryName, Func<LogLevel> logLevelGetter)
        {
            _logLevelGetter = logLevelGetter;
            _responseRouter = responseRouter;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new CompositeDisposable();
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= _logLevelGetter();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (logLevel < _logLevelGetter())
                return;

            if (TryGetMessageType(logLevel, out var messageType))
            {
                _responseRouter.Window.Log(new LogMessageParams()
                {
                    Type = messageType,
                    Message = _categoryName + ": " + formatter(state, exception) +
                              (exception != null ? " - " + exception : "") + " | " +
                              //Hopefully this isn't too expensive in the long run
                              (state is IEnumerable<KeyValuePair<string, object>> dict
                                  ? string.Join(" ", dict.Where(z => z.Key != "{OriginalFormat}").Select(z => $"{z.Key}='{z.Value}'"))
                                  : JsonConvert.SerializeObject(state).Replace("\"", "'")
                              )
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

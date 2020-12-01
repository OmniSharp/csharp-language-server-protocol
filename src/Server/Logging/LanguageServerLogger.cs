using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LanguageServerLogger : ILogger
    {
        private readonly ILanguageServerFacade _responseRouter;
        private readonly string _categoryName;

        public LanguageServerLogger(ILanguageServerFacade responseRouter, string categoryName)
        {;
            _responseRouter = responseRouter;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state) => new CompositeDisposable();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (TryGetMessageType(logLevel, out var messageType))
            {
                _responseRouter.Window.Log(
                    new LogMessageParams {
                        Type = messageType,
                        Message = _categoryName + ": " + formatter(state, exception) +
                                  ( exception != null ? " - " + exception : "" ) + " | " +
                                  //Hopefully this isn't too expensive in the long run
                                  ( state is IEnumerable<KeyValuePair<string, object>> dict
                                      ? string.Join(" ", dict.Where(z => z.Key != "{OriginalFormat}").Select(z => $"{z.Key}='{z.Value}'"))
                                      : JsonConvert.SerializeObject(state).Replace("\"", "'")
                                  )
                    }
                );
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
                    // TODO: Integrate with set trace?
                    messageType = MessageType.Log;
                    return true;
            }

            messageType = MessageType.Log;
            return false;
        }
    }
}

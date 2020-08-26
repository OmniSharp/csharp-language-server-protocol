using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.LanguageServer.Server.Logging
{
    internal class LanguageServerLoggerFilterOptions : IOptionsMonitor<LoggerFilterOptions>, IDisposable
    {
        private event Action<LoggerFilterOptions, string>? _onChange;

        public LanguageServerLoggerFilterOptions(IOptions<LoggerFilterOptions> options) => CurrentValue = options.Value;

        public LoggerFilterOptions CurrentValue { get; private set; }

        public LoggerFilterOptions Get(string _) => CurrentValue;

        public IDisposable OnChange(Action<LoggerFilterOptions, string> listener)
        {
            var disposable = new ChangeTrackerDisposable(this, listener);
            _onChange += disposable.OnChange;
            return disposable;
        }

        public void Dispose()
        {
        }

        internal void Set(LoggerFilterOptions options)
        {
            CurrentValue = options;
            _onChange?.Invoke(options, Options.DefaultName);
        }

        private class ChangeTrackerDisposable : IDisposable
        {
            private readonly Action<LoggerFilterOptions, string> _listener;
            private readonly LanguageServerLoggerFilterOptions _monitor;

            public ChangeTrackerDisposable(LanguageServerLoggerFilterOptions monitor, Action<LoggerFilterOptions, string> listener)
            {
                _listener = listener;
                _monitor = monitor;
            }

            public void OnChange(LoggerFilterOptions options, string name) => _listener.Invoke(options, name);

            public void Dispose() => _monitor._onChange -= OnChange;
        }
    }
}

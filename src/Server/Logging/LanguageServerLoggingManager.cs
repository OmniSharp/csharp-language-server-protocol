using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server.Logging
{
    internal class LanguageServerLoggingManager : ISetTraceHandler, IOptionsChangeTokenSource<LoggerFilterOptions>, IPostConfigureOptions<LoggerFilterOptions>
    {
        private ConfigurationReloadToken _changeToken;
        private InitializeTrace _currentInitializeTrace = InitializeTrace.Off;

        public LanguageServerLoggingManager()
        {
            _changeToken = new();
        }

        Task<Unit> IRequestHandler<SetTraceParams, Unit>.Handle(SetTraceParams request, CancellationToken cancellationToken)
        {
            SetTrace(request.Value);
            return Unit.Task;
        }

        public void SetTrace(InitializeTrace initializeTrace)
        {
            _currentInitializeTrace = initializeTrace;
            RaiseChanged();
        }

        public IChangeToken GetChangeToken() => _changeToken;

        public string Name { get; } = Options.DefaultName;

        public void PostConfigure(string name, LoggerFilterOptions options)
        {
            options.MinLevel = CalculateOptionsMinLevel();
        }

        private LogLevel CalculateOptionsMinLevel() =>
            _currentInitializeTrace switch {
                InitializeTrace.Off      => LogLevel.Warning,
                InitializeTrace.Messages => LogLevel.Information,
                InitializeTrace.Verbose  => LogLevel.Trace,
                _                        => LogLevel.Trace
            };

        private void RaiseChanged()
        {
            Interlocked.Exchange(ref _changeToken, new ConfigurationReloadToken()).OnReload();
        }
    }
}

using System;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class WorkDoneProgressReporter : IDisposable
    {
        private readonly ProgressManager _progressManager;
        private readonly ProgressToken _token;
        private readonly CancellationToken _cancellationToken;
        private ResultObserver<WorkDoneProgressReport> _reporter;

        internal WorkDoneProgressReporter(ProgressManager progressManager, ProgressToken token, CancellationToken cancellationToken)
        {
            _progressManager = progressManager;
            _token = token;
            _cancellationToken = cancellationToken;
        }

        public ResultObserver<WorkDoneProgressReport> Begin(WorkDoneProgressBegin begin, Func<WorkDoneProgressEnd> end = null)
        {
            end ??= (() => new WorkDoneProgressEnd());
            return _reporter ?? (_reporter = _progressManager.Create(_token, begin, end, _cancellationToken));
        }

        public void Dispose()
        {
            _reporter?.OnCompleted();
        }
    }
}

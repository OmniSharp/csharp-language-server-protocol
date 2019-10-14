using System;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class WorkDoneProgressReporter : IDisposable, IObserver<WorkDoneProgressReport>
    {
        private readonly ProgressManager _progressManager;
        private readonly IWorkDoneProgressParams _request;
        private readonly WorkDoneProgressBegin _begin;
        private readonly Func<WorkDoneProgressEnd> _end;
        private readonly CancellationToken _cancellationToken;
        private ResultObserver<WorkDoneProgressReport> _reporter;

        internal WorkDoneProgressReporter(ProgressManager progressManager, IWorkDoneProgressParams request, WorkDoneProgressBegin begin, Func<WorkDoneProgressEnd> end, CancellationToken cancellationToken)
        {
            _progressManager = progressManager;
            _request = request;
            _begin = begin;
            _end = end;
            _cancellationToken = cancellationToken;
        }

        public void Dispose()
        {
            _reporter?.OnCompleted();
        }

        public void OnNext(WorkDoneProgressReport value)
        {
            _reporter ??= _progressManager.Create(_request, _begin, _end, _cancellationToken);
            _reporter.OnNext(value);
        }

        public void OnError(Exception error)
        {
            _reporter ??= _progressManager.Create(_request, _begin, _end, _cancellationToken);
            _reporter.OnError(error);
        }

        public void OnCompleted()
        {
            _reporter ??= _progressManager.Create(_request, _begin, _end, _cancellationToken);
            _reporter.OnCompleted();
        }
    }
}

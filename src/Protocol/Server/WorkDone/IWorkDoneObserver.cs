using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone
{
    public interface IWorkDoneObserver : IObserver<WorkDoneProgress>, IDisposable
    {
        ProgressToken WorkDoneToken { get; }
        void OnNext(string message, int? percentage, bool? cancellable);
    }
}

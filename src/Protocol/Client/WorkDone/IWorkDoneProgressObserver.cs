using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone
{
    public interface IWorkDoneProgressObserver
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnReport(WorkDoneProgressReport report);
        void OnBegin(WorkDoneProgressBegin begin);
        void OnEnd(WorkDoneProgressEnd begin);
    }
}

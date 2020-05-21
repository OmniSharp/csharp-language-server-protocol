using System;
using DynamicData;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone
{
    public interface IClientWorkDoneManager
    {
        void Initialize(WindowClientCapabilities windowClientCapabilities);
        bool IsSupported { get; }
        IObservableCache<IProgressObservable<WorkDoneProgress>, ProgressToken> PendingWork { get; }
        IProgressObservable<WorkDoneProgress> Monitor(ProgressToken progressToken);
    }
}

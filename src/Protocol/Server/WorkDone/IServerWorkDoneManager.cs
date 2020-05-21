using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone
{
    public interface IServerWorkDoneManager
    {
        void Initialized(WindowClientCapabilities windowClientCapabilities);

        bool IsSupported { get; }

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        Task<IWorkDoneObserver> Create(ProgressToken progressToken, WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="IObserver{WorkDoneProgressReport}" /> that will send all of its progress information to the same source.
        /// The other side can cancel this, so the <see cref="CancellationToken" /> should be respected.
        /// </summary>
        Task<IWorkDoneObserver> Create(WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null, Func<WorkDoneProgressEnd> onComplete = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="IWorkDoneObserver" /> for a request where the client is already listening to work done.
        /// </summary>
        IWorkDoneObserver For(IWorkDoneProgressParams request, WorkDoneProgressBegin begin, Func<Exception, WorkDoneProgressEnd> onError = null,
            Func<WorkDoneProgressEnd> onComplete = null);
    }
}

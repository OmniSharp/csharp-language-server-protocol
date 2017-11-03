using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Processes
{
    using Logging;

    /// <summary>
    ///     A <see cref="ServerProcess"/> is responsible for launching or attaching to a language server, providing access to its input and output streams, and tracking its lifetime.
    /// </summary>
    public abstract class ServerProcess
        : IDisposable
    {
        /// <summary>
        ///     Create a new <see cref="ServerProcess"/>.
        /// </summary>
        /// <param name="logger">
        ///     The logger to use.
        /// </param>
        protected ServerProcess(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            Log = logger.ForSourceContext(
                source: GetType()
            );

            ServerStartCompletion = new TaskCompletionSource<object>();

            ServerExitCompletion = new TaskCompletionSource<object>();
            ServerExitCompletion.SetResult(null); // Start out as if the server has already exited.
        }

        /// <summary>
        ///     Finaliser for <see cref="ServerProcess"/>.
        /// </summary>
        ~ServerProcess()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Dispose of resources being used by the launcher.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Dispose of resources being used by the launcher.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        ///     The launcher's logger.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        ///     The <see cref="TaskCompletionSource{TResult}"/> used to signal server startup.
        /// </summary>
        protected TaskCompletionSource<object> ServerStartCompletion { get; set; }

        /// <summary>
        ///     The <see cref="TaskCompletionSource{TResult}"/> used to signal server exit.
        /// </summary>
        protected TaskCompletionSource<object> ServerExitCompletion { get; set; }

        /// <summary>
        ///     Event raised when the server has exited.
        /// </summary>
        public event EventHandler<EventArgs> Exited;

        /// <summary>
        ///     Is the server running?
        /// </summary>
        public abstract bool IsRunning { get; }

        /// <summary>
        ///     A <see cref="Task"/> that completes when the server has started.
        /// </summary>
        public Task HasStarted => ServerStartCompletion.Task;

        /// <summary>
        ///     A <see cref="Task"/> that completes when the server has exited.
        /// </summary>
        public Task HasExited => ServerExitCompletion.Task;

        /// <summary>
        ///     The server's input stream.
        /// </summary>
        /// <remarks>
        ///     The connection will write to the server's input stream, and read from its output stream.
        /// </remarks>
        public abstract Stream InputStream { get; }

        /// <summary>
        ///     The server's output stream.
        /// </summary>
        /// <remarks>
        ///     The connection will read from the server's output stream, and write to its input stream.
        /// </remarks>
        public abstract Stream OutputStream { get; }

        /// <summary>
        ///     Start or connect to the server.
        /// </summary>
        public abstract Task Start();

        /// <summary>
        ///     Stop or disconnect from the server.
        /// </summary>
        public abstract Task Stop();

        /// <summary>
        ///     Raise the <see cref="Exited"/> event.
        /// </summary>
        protected virtual void OnExited()
        {
            Exited?.Invoke(this, EventArgs.Empty);
        }
    }
}

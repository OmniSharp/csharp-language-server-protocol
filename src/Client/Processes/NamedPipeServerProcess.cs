using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.LanguageServer.Client.Processes
{
    /// <summary>
    ///     A <see cref="NamedPipeServerProcess"/> is a <see cref="ServerProcess"/> that creates named pipe streams to connect a language client to a language server in the same process.
    /// </summary>
    public class NamedPipeServerProcess
        : ServerProcess
    {
        /// <summary>
        ///     Create a new <see cref="NamedPipeServerProcess"/>.
        /// </summary>
        /// <param name="baseName">
        ///     The base name (prefix) used to create the named pipes.
        /// </param>
        /// <param name="loggerFactory">
        ///     The factory for loggers used by the process and its components.
        /// </param>
        public NamedPipeServerProcess(string baseName, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            BaseName = baseName;
        }

        /// <summary>
        ///     Dispose of resources being used by the <see cref="NamedPipeServerProcess"/>.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                CloseStreams();

            base.Dispose(disposing);
        }

        /// <summary>
        ///     The base name (prefix) used to create the named pipes.
        /// </summary>
        public string BaseName { get; }

        /// <summary>
        ///     Is the server running?
        /// </summary>
        public override bool IsRunning => ServerStartCompletion.Task.IsCompleted;

        /// <summary>
        ///     A <see cref="NamedPipeClientStream"/> that the client reads messages from.
        /// </summary>
        public NamedPipeClientStream ClientInputStream { get; protected set; }

        /// <summary>
        ///     A <see cref="NamedPipeClientStream"/> that the client writes messages to.
        /// </summary>
        public NamedPipeClientStream ClientOutputStream { get; protected set; }

        /// <summary>
        ///     A <see cref="NamedPipeServerStream"/> that the server reads messages from.
        /// </summary>
        public NamedPipeServerStream ServerInputStream { get; protected set; }

        /// <summary>
        ///     A <see cref="NamedPipeServerStream"/> that the server writes messages to.
        /// </summary>
        public NamedPipeServerStream ServerOutputStream { get; protected set; }

        /// <summary>
        ///     The server's input stream.
        /// </summary>
        public override Stream InputStream => ServerInputStream;

        /// <summary>
        ///     The server's output stream.
        /// </summary>
        public override Stream OutputStream => ServerOutputStream;

        /// <summary>
        ///     Start or connect to the server.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public override async Task Start()
        {
            ServerExitCompletion = new TaskCompletionSource<object>();

            ServerInputStream = new NamedPipeServerStream(BaseName + "_in", PipeDirection.Out, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, inBufferSize: 1024, outBufferSize: 1024);
            ServerOutputStream = new NamedPipeServerStream(BaseName + "_out", PipeDirection.In, 2, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, inBufferSize: 1024, outBufferSize: 1024);
            ClientInputStream = new NamedPipeClientStream(".", BaseName + "_out", PipeDirection.Out, PipeOptions.Asynchronous);
            ClientOutputStream = new NamedPipeClientStream(".", BaseName + "_in", PipeDirection.In, PipeOptions.Asynchronous);

            // Ensure all pipes are connected before proceeding.
            await Task.WhenAll(
                ServerInputStream.WaitForConnectionAsync(),
                ServerOutputStream.WaitForConnectionAsync(),
                ClientInputStream.ConnectAsync(),
                ClientOutputStream.ConnectAsync()
            );

            ServerStartCompletion.TrySetResult(null);
        }

        /// <summary>
        ///     Stop the server.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public override Task Stop()
        {
            ServerStartCompletion = new TaskCompletionSource<object>();

            CloseStreams();

            ServerExitCompletion.TrySetResult(null);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Close the underlying streams.
        /// </summary>
        void CloseStreams()
        {
            ClientInputStream?.Dispose();
            ClientInputStream = null;

            ClientOutputStream?.Dispose();
            ClientOutputStream = null;

            ServerInputStream?.Dispose();
            ServerInputStream = null;

            ServerOutputStream?.Dispose();
            ServerOutputStream = null;
        }
    }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    using Dispatcher;
    using Processes;
    using Protocol;

    /// <summary>
    ///     The base class for test suites that use a <see cref="PipeServerProcess"/>.
    /// </summary>
    public abstract class PipeServerTestBase
        : TestBase
    {
        /// <summary>
        ///     The <see cref="PipeServerProcess"/> used to connect client and server streams.
        /// </summary>
        readonly NamedPipeServerProcess _serverProcess;

        /// <summary>
        ///     Create a new <see cref="PipeServerTestBase"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        protected PipeServerTestBase(ITestOutputHelper testOutput)
            : base(testOutput)
        {
            _serverProcess = new NamedPipeServerProcess(Guid.NewGuid().ToString("N"), Log);
            Disposal.Add(_serverProcess);
        }

        /// <summary>
        ///     The workspace root path.
        /// </summary>
        protected virtual string WorkspaceRoot => Path.GetDirectoryName(GetType().Assembly.Location);

        /// <summary>
        ///     The client's output stream (server reads from this).
        /// </summary>
        protected Stream ClientOutputStream => _serverProcess.ClientOutputStream;

        /// <summary>
        ///     The client's input stream (server writes to this).
        /// </summary>
        protected Stream ClientInputStream => _serverProcess.ClientInputStream;

        /// <summary>
        ///     The server's output stream (client reads from this).
        /// </summary>
        protected Stream ServerOutputStream => _serverProcess.ServerOutputStream;

        /// <summary>
        ///     The server's input stream (client writes to this).
        /// </summary>
        protected Stream ServerInputStream => _serverProcess.ServerInputStream;

        /// <summary>
        ///     Create a <see cref="LanguageClient"/> connected to the test's <see cref="PipeServerProcess"/>.
        /// </summary>
        /// <param name="initialize">
        ///     Automatically initialise the client?
        ///     
        ///     Default is <c>true</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="LanguageClient"/>.
        /// </returns>
        protected async Task<LanguageClient> CreateClient(bool initialize = true)
        {
            if (!_serverProcess.IsRunning)
                await StartServer();

            await _serverProcess.HasStarted;

            LanguageClient client = new LanguageClient(Log, _serverProcess);
            Disposal.Add(client);

            if (initialize)
                await client.Initialize(WorkspaceRoot);

            return client;
        }

        /// <summary>
        ///     Create a <see cref="LspConnection"/> that uses the client ends of the the test's <see cref="PipeServerProcess"/> streams.
        /// </summary>
        /// <returns>
        ///     The <see cref="LspConnection"/>.
        /// </returns>
        protected async Task<LspConnection> CreateClientConnection()
        {
            if (!_serverProcess.IsRunning)
                await StartServer();

            await _serverProcess.HasStarted;

            LspConnection connection = new LspConnection(Log, input: ServerOutputStream, output: ServerInputStream);
            Disposal.Add(connection);

            return connection;
        }

        /// <summary>
        ///     Create a <see cref="LspConnection"/> that uses the server ends of the the test's <see cref="PipeServerProcess"/> streams.
        /// </summary>
        /// <returns>
        ///     The <see cref="LspConnection"/>.
        /// </returns>
        protected async Task<LspConnection> CreateServerConnection()
        {
            if (!_serverProcess.IsRunning)
                await StartServer();

            await _serverProcess.HasStarted;

            LspConnection connection = new LspConnection(Log, input: ClientOutputStream, output: ClientInputStream);
            Disposal.Add(connection);

            return connection;
        }

        /// <summary>
        ///     Called to start the server process.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        protected virtual Task StartServer() => _serverProcess.Start();

        /// <summary>
        ///     Called to stop the server process.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        protected virtual Task StopServer() => _serverProcess.Stop();
    }
}

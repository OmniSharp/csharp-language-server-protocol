using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests
{
    /// <summary>
    ///     The base class for test suites that use a <see cref="PipeServerProcess"/>.
    /// </summary>
    public abstract class PipeServerTestBase : TestBase, IAsyncLifetime
    {
        /// <summary>
        ///     The <see cref="PipeServerProcess"/> used to connect client and server streams.
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        private ILanguageClient _client;
        private ILanguageServer _server;

        /// <summary>
        ///     Create a new <see cref="PipeServerTestBase"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        protected PipeServerTestBase(ITestOutputHelper testOutput)
            : base(testOutput)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            if (!Debugger.IsAttached)
            {
                _cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
            }
        }

        /// <summary>
        ///     The workspace root path.
        /// </summary>
        protected string WorkspaceRoot => Path.GetDirectoryName(GetType().Assembly.Location);

        /// <summary>
        ///     Create a <see cref="LanguageClient"/> connected to the test's <see cref="PipeServerProcess"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="LanguageClient"/>.
        /// </returns>
        protected async Task<(ILanguageClient client, ILanguageServer server)> Initialize(
            Action<LanguageClientOptions> clientOptionsAction,
            Action<LanguageServerOptions> serverOptionsAction)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();
            _client = LanguageClient.PreInit(options => {
                options.Services.AddSingleton(LoggerFactory);
                options.WithInput(serverPipe.Reader).WithOutput(clientPipe.Writer);
                options.WithRootPath(WorkspaceRoot);
                clientOptionsAction(options);
            });
            Disposal.Add(_client);

            _server = OmniSharp.Extensions.LanguageServer.Server.LanguageServer.PreInit(options => {
                options.Services.AddSingleton(LoggerFactory);
                options.WithInput(clientPipe.Reader).WithOutput(serverPipe.Writer);
                serverOptionsAction(options);
            });
            Disposal.Add(_server);

            await Task.WhenAll(
                _client.Initialize(CancellationToken),
                _server.Initialize(CancellationToken)
            );

            return (_client, _server);
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync() => _client.Shutdown();
    }
}

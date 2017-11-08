using OmniSharp.Extensions.LanguageServerProtocol.Client.Dispatcher;
using OmniSharp.Extensions.LanguageServerProtocol.Client.Protocol;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    /// <summary>
    ///     Tests for <see cref="LspConnection"/>.
    /// </summary>
    public class ConnectionTests
        : PipeServerTestBase
    {
        /// <summary>
        ///     Create a new <see cref="LspConnection"/> test suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        public ConnectionTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        /// <summary>
        ///     Verify that a server <see cref="LspConnection"/> can handle an empty notification from a client <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Server connection can handle empty notification from client")]
        public async Task Client_HandleEmptyNotification_Success()
        {
            TaskCompletionSource<object> testCompletion = new TaskCompletionSource<object>();

            LspConnection serverConnection = await CreateServerConnection();
            LspConnection clientConnection = await CreateClientConnection();

            LspDispatcher serverDispatcher = new LspDispatcher();
            serverDispatcher.HandleEmptyNotification("test", () =>
            {
                Log.Information("Got notification.");

                testCompletion.SetResult(null);
            });
            serverConnection.Connect(serverDispatcher);

            clientConnection.Connect(new LspDispatcher());
            clientConnection.SendEmptyNotification("test");

            await testCompletion.Task;

            clientConnection.Disconnect(flushOutgoing: true);
            serverConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }

        /// <summary>
        ///     Verify that a client <see cref="LspConnection"/> can handle an empty notification from a server <see cref="LspConnection"/>.
        /// </summary>
        [Fact(DisplayName = "Client connection can handle empty notification from server")]
        public async Task Server_HandleEmptyNotification_Success()
        {
            TaskCompletionSource<object> testCompletion = new TaskCompletionSource<object>();

            LspConnection clientConnection = await CreateClientConnection();
            LspConnection serverConnection = await CreateServerConnection();

            LspDispatcher clientDispatcher = new LspDispatcher();
            clientDispatcher.HandleEmptyNotification("test", () =>
            {
                Log.Information("Got notification.");

                testCompletion.SetResult(null);
            });
            clientConnection.Connect(clientDispatcher);

            serverConnection.Connect(new LspDispatcher());
            serverConnection.SendEmptyNotification("test");

            await testCompletion.Task;

            serverConnection.Disconnect(flushOutgoing: true);
            clientConnection.Disconnect();

            await Task.WhenAll(clientConnection.HasHasDisconnected, serverConnection.HasHasDisconnected);
        }
    }
}

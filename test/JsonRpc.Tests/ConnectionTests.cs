using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Nerdbank.Streams;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class ConnectionTests
    {
        public void Test()
        {
            var streamIn = Substitute.For<TextReader>();
            var streamOut = Substitute.For<TextWriter>();

            //var connection = new Connection(
            //    streamIn,
            //    streamOut,
            //    new SerialRequestProcessIdentifier()
            //);
        }
    }

    public class JsonRpcServerTests : JsonRpcServerTestBase
    {
        public JsonRpcServerTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper)) { }

        private string _pipeName = Guid.NewGuid().ToString();

        [Fact]
        public async Task Can_Reconnect_To_A_Pipe()
        {
            {
                var serverPipe = new NamedPipeServerStream(
                    pipeName: _pipeName,
                    direction: PipeDirection.InOut,
                    maxNumberOfServerInstances: 1,
                    transmissionMode: PipeTransmissionMode.Byte,
                    options: PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous);
                var clientPipe = new NamedPipeClientStream(
                    ".",
                    _pipeName,
                    PipeDirection.InOut,
                    PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
                );

                var (client, server) = await Initialize(clientOptions => {
                    clientOptions
                        .WithInput(clientPipe)
                        .WithOutput(clientPipe);
                }, serverOptions => {
                    serverOptions
                        .WithInput(serverPipe)
                        .WithOutput(serverPipe);
                });

                await Task.WhenAll(clientPipe.ConnectAsync(CancellationToken), serverPipe.WaitForConnectionAsync(CancellationToken));

                client.Dispose();
                server.Dispose();
                //
                // serverPipe.Dispose();
                // clientPipe.Dispose();
            }

            {
                var serverPipe = new NamedPipeServerStream(
                    pipeName: _pipeName,
                    direction: PipeDirection.InOut,
                    maxNumberOfServerInstances: 1,
                    transmissionMode: PipeTransmissionMode.Byte,
                    options: PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous);
                var clientPipe = new NamedPipeClientStream(
                    ".",
                    _pipeName,
                    PipeDirection.InOut,
                    PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
                );

                var (client, server) = await Initialize(clientOptions => {
                    clientOptions
                        .WithInput(clientPipe)
                        .WithOutput(clientPipe);
                    // clientOptions.RegisterForDisposal(clientPipe);
                }, serverOptions => {
                    serverOptions
                        .WithInput(serverPipe)
                        .WithOutput(serverPipe);
                    // serverOptions.RegisterForDisposal(serverPipe);
                });

                await Task.WhenAll(clientPipe.ConnectAsync(CancellationToken), serverPipe.WaitForConnectionAsync(CancellationToken));
            }
        }

        [Fact]
        public async Task Can_Connect_To_A_Pipe()
        {
            var serverPipe = new NamedPipeServerStream(
                pipeName: _pipeName ,
                direction: PipeDirection.InOut,
                maxNumberOfServerInstances: 1,
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous);
            var clientPipe = new NamedPipeClientStream(
                ".",
                _pipeName ,
                PipeDirection.InOut,
                PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
            );

            var (client, server) = await Initialize(clientOptions => {
                clientOptions
                    .WithInput(clientPipe.UsePipeReader())
                    .WithOutput(clientPipe.UsePipeWriter());
            }, serverOptions => {
                serverOptions
                    .WithInput(serverPipe.UsePipeReader())
                    .WithOutput(serverPipe.UsePipeWriter());
            });

            await Task.WhenAll(clientPipe.ConnectAsync(CancellationToken), serverPipe.WaitForConnectionAsync(CancellationToken));
        }
    }
}

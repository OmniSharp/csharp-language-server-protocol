using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Nerdbank.Streams;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace JsonRpc.Tests
{
    public class JsonRpcServerTests : JsonRpcServerTestBase
    {
        public JsonRpcServerTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        private readonly string _pipeName = Guid.NewGuid().ToString();

        [Fact]
        public async Task Can_Connect_To_Stdio()
        {
            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions
                       .WithInput(Console.OpenStandardInput().UsePipeReader())
                       .WithOutput(Console.OpenStandardError().UsePipeWriter());
                }, serverOptions => {
                    serverOptions
                       .WithInput(Console.OpenStandardInput().UsePipeReader())
                       .WithOutput(Console.OpenStandardError().UsePipeWriter());
                }
            );
        }

        [Fact]
        public async Task Can_Connect_To_A_Named_Pipe()
        {
            var serverPipe = new NamedPipeServerStream(
                _pipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
            );
            var clientPipe = new NamedPipeClientStream(
                ".",
                _pipeName,
                PipeDirection.InOut,
                PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
            );

            var (client, server) = await Initialize(
                clientOptions => {
                    clientOptions
                       .WithInput(clientPipe)
                       .WithOutput(clientPipe);
                }, serverOptions => {
                    serverOptions
                       .WithInput(serverPipe)
                       .WithOutput(serverPipe);
                }
            );

            await Task.WhenAll(clientPipe.ConnectAsync(CancellationToken), serverPipe.WaitForConnectionAsync(CancellationToken));
        }

        [Fact]
        public async Task Can_Reconnect_To_A_Named_Pipe()
        {
            {
                var serverPipe = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
                );
                var clientPipe = new NamedPipeClientStream(
                    ".",
                    _pipeName,
                    PipeDirection.InOut,
                    PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
                );

                var (client, server) = await Initialize(
                    clientOptions => {
                        clientOptions
                           .WithInput(clientPipe)
                           .WithOutput(clientPipe);
                    }, serverOptions => {
                        serverOptions
                           .WithInput(serverPipe)
                           .WithOutput(serverPipe);
                    }
                );

                await Task.WhenAll(clientPipe.ConnectAsync(CancellationToken), serverPipe.WaitForConnectionAsync(CancellationToken));

                client.Dispose();
                server.Dispose();
                //
                // serverPipe.Dispose();
                // clientPipe.Dispose();
            }

            {
                var serverPipe = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
                );
                var clientPipe = new NamedPipeClientStream(
                    ".",
                    _pipeName,
                    PipeDirection.InOut,
                    PipeOptions.CurrentUserOnly | PipeOptions.Asynchronous
                );

                var (client, server) = await Initialize(
                    clientOptions => {
                        clientOptions
                           .WithInput(clientPipe)
                           .WithOutput(clientPipe);
                        // clientOptions.RegisterForDisposal(clientPipe);
                    }, serverOptions => {
                        serverOptions
                           .WithInput(serverPipe)
                           .WithOutput(serverPipe);
                        // serverOptions.RegisterForDisposal(serverPipe);
                    }
                );

                await Task.WhenAll(clientPipe.ConnectAsync(CancellationToken), serverPipe.WaitForConnectionAsync(CancellationToken));
            }
        }
    }
}

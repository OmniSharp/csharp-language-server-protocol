using System;
using System.IO.Pipelines;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    /// <summary>
    /// This is a test class that is designed to allow you configure an in memory lsp client and server to do testing of handlers or behaviors.
    /// </summary>
    public abstract class JsonRpcServerTestBase : JsonRpcTestBase
    {
        private IJsonRpcServer _client;
        private IJsonRpcServer _server;

        public JsonRpcServerTestBase(JsonRpcTestOptions testOptions) : base(testOptions)
        {
        }

        protected virtual void ConfigureClientInputOutput(PipeReader inMemoryReader, PipeWriter inMemoryWriter, JsonRpcServerOptions options)
        {
            options.WithInput(inMemoryReader).WithOutput(inMemoryWriter);
        }

        protected virtual void ConfigureServerInputOutput(PipeReader inMemoryReader, PipeWriter inMemoryWriter, JsonRpcServerOptions options)
        {
            options.WithInput(inMemoryReader).WithOutput(inMemoryWriter);
        }

        protected virtual async Task<(IJsonRpcServer client, IJsonRpcServer server)> Initialize(
            Action<JsonRpcServerOptions> clientOptionsAction,
            Action<JsonRpcServerOptions> serverOptionsAction)
        {
            var clientPipe = new Pipe();
            var serverPipe = new Pipe();

            var clientTask = JsonRpcServer.From(options => {
                options
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler)
                    .AddLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ClientLoggerFactory);
                    });
                ConfigureClientInputOutput(serverPipe.Reader, clientPipe.Writer, options);
                clientOptionsAction(options);
            });

            var serverTask = JsonRpcServer.From(options => {
                options
                    .Services
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(SettlePipeline<,>))
                    .AddSingleton(ServerEvents as IRequestSettler)
                    .AddLogging(x => {
                        x.SetMinimumLevel(LogLevel.Trace);
                        x.Services.AddSingleton(TestOptions.ServerLoggerFactory);
                    });
                ConfigureServerInputOutput(clientPipe.Reader, serverPipe.Writer, options);
                serverOptionsAction(options);
            });

            await Task.WhenAll(clientTask, serverTask);
            _client = clientTask.Result;
            _server = serverTask.Result;

            Disposable.Add(_client);
            Disposable.Add(_server);

            return (_client, _server);
        }
    }
}
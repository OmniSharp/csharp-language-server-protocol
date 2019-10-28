using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;

namespace SampleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            //Debugger.Launch();
            //while (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    await Task.Delay(100);
            //}

            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();

            Log.Logger.Information("This only goes file...");

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .ConfigureLogging(x => x
                        .AddSerilog()
                        .AddLanguageServer()
                        .SetMinimumLevel(LogLevel.Information))
                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<DidChangeWatchedFilesHandler>()
                    .WithHandler<FoldingRangeHandler>()
                    .WithServices(services => {
                        services.AddSingleton<Foo>(provider => {
                            var loggerFactory = provider.GetService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger<Foo>();

                            logger.LogInformation("Configuring");

                            return new Foo(logger);
                        });
                    }).OnInitialize((s, request) => {
                        var serviceProvider = s.Services;
                        var foo = serviceProvider.GetService<Foo>();

                        return Task.CompletedTask;
                    })
                );

            await server.WaitForExit;
        }
    }

    internal class Foo
    {
        private readonly ILogger<Foo> _logger;

        public Foo(ILogger<Foo> logger)
        {
            logger.LogInformation("inside ctor");
            _logger = logger;
        }

        public void SayFoo()
        {
            _logger.LogInformation("Fooooo!");
        }
    }
}

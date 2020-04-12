using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog;
using ILanguageServer = OmniSharp.Extensions.LanguageServer.Server.ILanguageServer;

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
            // Debugger.Launch();
            // while (!System.Diagnostics.Debugger.IsAttached)
            // {
            //     await Task.Delay(100);
            // }

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Verbose()
              .CreateLogger();

            Log.Logger.Information("This only goes file...");

            IObserver<WorkDoneProgressReport> workDone = null;

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .ConfigureLogging(x => x
                        .AddSerilog()
                        .AddLanguageServer()
                        .SetMinimumLevel(LogLevel.Debug))
                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<DidChangeWatchedFilesHandler>()
                    .WithHandler<FoldingRangeHandler>()
                    .WithHandler<MyWorkspaceSymbolsHandler>()
                    .WithHandler<MyDocumentSymbolHandler>()
                    .WithHandler<SemanticTokens>()
                    .WithServices(x => x.AddLogging(b => b.SetMinimumLevel(LogLevel.Trace)))
                    .WithServices(services => {
                        services.AddSingleton(provider => {
                            var loggerFactory = provider.GetService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger<Foo>();

                            logger.LogInformation("Configuring");

                            return new Foo(logger);
                        });
                        services.AddSingleton(new ConfigurationItem() {
                            Section = "typescript",
                        }).AddSingleton(new ConfigurationItem() {
                            Section = "terminal",
                        });
                    })
                    .OnInitialize(new InitializeDelegate(async (server, request) => {
                        var manager = server.ProgressManager.WorkDone(request, new WorkDoneProgressBegin() {
                            Title = "Server is starting...",
                            Percentage = 10,
                        });
                        workDone = manager;

                        await Task.Delay(2000);

                        manager.OnNext(new WorkDoneProgressReport() {
                            Percentage = 20,
                            Message = "loading in progress"
                        });
                    }))
                    .OnInitialized(new InitializedDelegate(async (server, request, response) => {
                        workDone.OnNext(new WorkDoneProgressReport() {
                            Percentage = 40,
                            Message = "loading almost done",
                        });

                        await Task.Delay(2000);

                        workDone.OnNext(new WorkDoneProgressReport() {
                            Message = "loading done",
                            Percentage = 100,
                        });
                    }))
                    .OnStarted(async (languageServer, result) => {
                        using var manager = await languageServer.ProgressManager.Create(new WorkDoneProgressBegin() { Title = "Doing some work..." });

                        manager.OnNext(new WorkDoneProgressReport() { Message = "doing things..." });
                        await Task.Delay(10000);
                        manager.OnNext(new WorkDoneProgressReport() { Message = "doing things... 1234" });
                        await Task.Delay(10000);
                        manager.OnNext(new WorkDoneProgressReport() { Message = "doing things... 56789" });

                        var logger = languageServer.Services.GetService<ILogger<Foo>>();
                        var configuration = await languageServer.Configuration.GetConfiguration(
                            new ConfigurationItem() {
                                Section = "typescript",
                            }, new ConfigurationItem() {
                                Section = "terminal",
                            });

                        var baseConfig = new JObject();
                        foreach (var config in languageServer.Configuration.AsEnumerable())
                        {
                            baseConfig.Add(config.Key, config.Value);
                        }

                        logger.LogInformation("Base Config: {Config}", baseConfig);

                        var scopedConfig = new JObject();
                        foreach (var config in configuration.AsEnumerable())
                        {
                            scopedConfig.Add(config.Key, config.Value);
                        }

                        logger.LogInformation("Scoped Config: {Config}", scopedConfig);
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

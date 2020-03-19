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
                    .WithServices(x => x.AddLogging(b => b.SetMinimumLevel(LogLevel.Trace)))
                    .WithServices(services =>
                    {
                        services.AddSingleton(provider =>
                        {
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
                    .OnStarted(async (languageServer, result) =>
                    {
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

using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
            // while (!System.Diagnostics.Debugger.IsAttached)
            // {
            //    await Task.Delay(100);
            // }

            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();

            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .ConfigureLogging(x => x.AddSerilog())
                    .AddDefaultLoggingProvider()
                    .WithHandler<TextDocumentHandler>()
                    .WithHandler<DidChangeWatchedFilesHandler>()
                    .WithHandler<FoldingRangeHandler>()
                );

            await server.WaitForExit;
        }
    }
}

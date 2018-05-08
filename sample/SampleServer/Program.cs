using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Server;

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
            //while (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    await Task.Delay(100);
            //}

            var server = LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory()));

            server.AddHandler(new TextDocumentHandler(server));

            await server.Initialize();
            await server.WaitForExit;
        }
    }
}

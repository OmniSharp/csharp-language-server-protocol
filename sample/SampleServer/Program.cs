using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer;

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

            var server = new LanguageServer(Console.OpenStandardInput(), Console.OpenStandardOutput(), new LoggerFactory());

            server.AddHandler(new TextDocumentHandler(server));

            await server.Initialize();
            await server.WasShutDown;
        }
    }
}

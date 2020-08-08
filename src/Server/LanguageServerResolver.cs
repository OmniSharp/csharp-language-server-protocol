using System;
using System.Collections.Concurrent;
using DryIoc;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LanguageServerResolver : IDisposable
    {
        private readonly IOptionsMonitor<LanguageServerOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, LanguageServer> _servers = new ConcurrentDictionary<string, LanguageServer>();

        public LanguageServerResolver(IOptionsMonitor<LanguageServerOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public LanguageServer Get(string name)
        {
            if (_servers.TryGetValue(name, out var server)) return server;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = LanguageServer.CreateContainer(options, _outerServiceProvider);
            server = container.Resolve<LanguageServer>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }
}
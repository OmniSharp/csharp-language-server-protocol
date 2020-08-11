using System;
using System.Collections.Concurrent;
using DryIoc;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.DebugAdapter.Server
{
    public class DebugAdapterServerResolver : IDisposable
    {
        private readonly IOptionsMonitor<DebugAdapterServerOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, DebugAdapterServer> _servers = new ConcurrentDictionary<string, DebugAdapterServer>();

        public DebugAdapterServerResolver(IOptionsMonitor<DebugAdapterServerOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public DebugAdapterServer Get(string name)
        {
            if (_servers.TryGetValue(name, out var server)) return server;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = DebugAdapterServer.CreateContainer(options, _outerServiceProvider);
            server = container.Resolve<DebugAdapterServer>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }
}

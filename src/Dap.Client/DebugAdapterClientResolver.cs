using System.Collections.Concurrent;
using DryIoc;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.DebugAdapter.Client
{
    public class DebugAdapterClientResolver : IDisposable
    {
        private readonly IOptionsMonitor<DebugAdapterClientOptions> _monitor;
        private readonly IServiceProvider? _outerServiceProvider;
        private readonly ConcurrentDictionary<string, DebugAdapterClient> _servers = new ConcurrentDictionary<string, DebugAdapterClient>();

        public DebugAdapterClientResolver(IOptionsMonitor<DebugAdapterClientOptions> monitor, IServiceProvider? outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public DebugAdapterClient Get(string name)
        {
            if (_servers.TryGetValue(name, out var server)) return server;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = DebugAdapterClient.CreateContainer(options, _outerServiceProvider);
            server = container.Resolve<DebugAdapterClient>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }
}

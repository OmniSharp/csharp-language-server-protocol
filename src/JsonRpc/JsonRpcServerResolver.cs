using System;
using System.Collections.Concurrent;
using DryIoc;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServerResolver : IDisposable
    {
        private readonly IOptionsMonitor<JsonRpcServerOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, JsonRpcServer> _servers = new ConcurrentDictionary<string, JsonRpcServer>();

        public JsonRpcServerResolver(IOptionsMonitor<JsonRpcServerOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public JsonRpcServer Get(string name)
        {
            if (_servers.TryGetValue(name, out var server)) return server;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = JsonRpcServer.CreateContainer(options, _outerServiceProvider);
            server = container.Resolve<JsonRpcServer>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }
}

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
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

            var serviceProvider = options.Services
                .AddJsonRpcServerInternals(options, _outerServiceProvider)
                .BuildServiceProvider();

            server = serviceProvider.GetRequiredService<JsonRpcServer>();
            _servers.TryAdd(name, server);

            return server;
        }

        public void Dispose()
        {
            foreach (var item in _servers.Values) item.Dispose();
        }
    }
}

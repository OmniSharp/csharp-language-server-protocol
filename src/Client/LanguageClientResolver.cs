using System;
using System.Collections.Concurrent;
using DryIoc;
using Microsoft.Extensions.Options;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LanguageClientResolver : IDisposable
    {
        private readonly IOptionsMonitor<LanguageClientOptions> _monitor;
        private readonly IServiceProvider _outerServiceProvider;
        private readonly ConcurrentDictionary<string, LanguageClient> _clients = new ConcurrentDictionary<string, LanguageClient>();

        public LanguageClientResolver(IOptionsMonitor<LanguageClientOptions> monitor, IServiceProvider outerServiceProvider)
        {
            _monitor = monitor;
            _outerServiceProvider = outerServiceProvider;
        }

        public LanguageClient Get(string name)
        {
            if (_clients.TryGetValue(name, out var client)) return client;

            var options = name == Options.DefaultName ? _monitor.CurrentValue : _monitor.Get(name);

            var container = LanguageClient.CreateContainer(options, _outerServiceProvider);
            client = container.Resolve<LanguageClient>();
            _clients.TryAdd(name, client);

            return client;
        }

        public void Dispose()
        {
            foreach (var item in _clients.Values) item.Dispose();
        }
    }
}
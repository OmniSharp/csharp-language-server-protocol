using System.IO.Pipelines;
using System.Reactive.Concurrency;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OmniSharp.Extensions.JsonRpc.Testing
{
    public sealed class JsonRpcTestOptions
    {
        public JsonRpcTestOptions()
        {
        }

        public JsonRpcTestOptions(ILoggerFactory loggerFactory) => ServerLoggerFactory = ClientLoggerFactory = loggerFactory;

        public JsonRpcTestOptions(ILoggerFactory clientLoggerFactory, ILoggerFactory serverLoggerFactory)
        {
            ClientLoggerFactory = clientLoggerFactory;
            ServerLoggerFactory = serverLoggerFactory;
        }

        public ILoggerFactory ClientLoggerFactory { get; internal set; } = NullLoggerFactory.Instance;
        public ILoggerFactory ServerLoggerFactory { get; internal set; } = NullLoggerFactory.Instance;
        public IScheduler ClientScheduler { get; internal set; } = TaskPoolScheduler.Default;
        public IScheduler ServerScheduler { get; internal set; } = TaskPoolScheduler.Default;
        public TimeSpan WaitTime { get; internal set; } = TimeSpan.FromMilliseconds(50);
        public TimeSpan Timeout { get; internal set; } = TimeSpan.FromMilliseconds(500);
        public TimeSpan CancellationTimeout { get; internal set; } = TimeSpan.FromSeconds(50);
        public PipeOptions DefaultPipeOptions { get; internal set; } = new PipeOptions();
        public IEnumerable<Assembly> Assemblies { get; set; } = Enumerable.Empty<Assembly>();
    }
}

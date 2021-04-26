using System.Reactive.Concurrency;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerOptionsExtensions
    {
        public static JsonRpcServerOptions WithSerializer(this JsonRpcServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }

        public static JsonRpcServerOptions WithAssemblyAttributeScanning(this JsonRpcServerOptions options, bool value)
        {
            options.UseAssemblyAttributeScanning = value;
            return options;
        }

        /// <summary>
        /// Sets both input and output schedulers to the same scheduler
        /// </summary>
        /// <param name="options"></param>
        /// <param name="inputScheduler"></param>
        /// <returns></returns>
        public static JsonRpcServerOptions WithScheduler(this JsonRpcServerOptions options, IScheduler inputScheduler)
        {
            options.DefaultScheduler = inputScheduler;
            return options;
        }
    }
}

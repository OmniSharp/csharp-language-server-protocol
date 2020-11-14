using System.Diagnostics.CodeAnalysis;
using OmniSharp.Extensions.JsonRpc.Serialization;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServerOptions : JsonRpcServerOptionsBase<JsonRpcServerOptions>
    {
        public JsonRpcServerOptions()
        {
            RequestProcessIdentifier = new ParallelRequestProcessIdentifier();
        }
        public ISerializer Serializer { get; set; } = new JsonRpcSerializer();
        [DisallowNull] public IReceiver? Receiver { get; set; } = null!;

        public JsonRpcServerOptions WithReceiver(IReceiver receiver)
        {
            Receiver = receiver;
            return this;
        }
    }
}

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
        public IReceiver Receiver { get; set; } = new Receiver();

        public JsonRpcServerOptions WithReceiver(IReceiver receiver)
        {
            Receiver = receiver;
            return this;
        }
    }
}

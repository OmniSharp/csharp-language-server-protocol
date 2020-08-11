using OmniSharp.Extensions.JsonRpc.Serialization;

namespace OmniSharp.Extensions.JsonRpc
{
    public class JsonRpcServerOptions : JsonRpcServerOptionsBase<JsonRpcServerOptions>
    {
        public ISerializer Serializer { get; set; } = new JsonRpcSerializer();
        public IReceiver Receiver { get; set; } = new Receiver();

        public JsonRpcServerOptions WithReceiver(IReceiver receiver)
        {
            Receiver = receiver;
            return this;
        }

        public override IRequestProcessIdentifier RequestProcessIdentifier { get; set; } = new ParallelRequestProcessIdentifier();
    }
}

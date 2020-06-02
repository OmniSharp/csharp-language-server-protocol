using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Reactive.Disposables;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;

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

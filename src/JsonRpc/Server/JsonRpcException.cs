using System;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class JsonRpcException : Exception
    {
        public JsonRpcException(ServerError error)
        {
            Error = error.Error;
            Id = error.Id;
            ProtocolVersion = error.ProtocolVersion;
        }

        public JToken Error { get; }
        public object Id { get; }
        public string ProtocolVersion { get; }
    }
}

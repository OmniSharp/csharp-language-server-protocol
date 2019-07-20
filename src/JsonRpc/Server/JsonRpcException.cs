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
        }

        public JToken Error { get; }
        public object Id { get; }
    }
}

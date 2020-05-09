using System;
using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class JsonRpcException : Exception
    {
        public JsonRpcException(ServerError error)
        {
            Error = error.Error;
            Id = error.Id;
        }

        public JsonElement Error { get; }
        public object Id { get; }
    }
}

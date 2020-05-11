using System;
using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Request : IMethodWithParams
    {
        internal Request(
            object id,
            string method,
            object @params)
        {
            Id = id;
            Method = method;
            Params = @params;
        }

        public object Id { get; }

        public string Method { get; }

        public object Params { get; }
    }
}

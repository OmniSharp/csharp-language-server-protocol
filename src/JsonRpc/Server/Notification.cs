using System;
using System.Text.Json;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    public class Notification : IMethodWithParams
    {
        internal Notification(string method, object @params)
        {
            Method = method;
            Params = @params;
        }

        public string Method { get; }

        public object Params { get; }
    }
}

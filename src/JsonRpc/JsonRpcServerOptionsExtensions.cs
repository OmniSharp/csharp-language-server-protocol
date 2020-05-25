using System;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerdbank.Streams;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public static class JsonRpcServerOptionsExtensions
    {
        public static JsonRpcServerOptions WithSerializer(this JsonRpcServerOptions options, ISerializer serializer)
        {
            options.Serializer = serializer;
            return options;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcServerOptions
    {
        PipeReader Input { get; set; }
        PipeWriter Output { get; set; }
        IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
        int? Concurrency { get; set; }
        Action<Exception> OnUnhandledException { get; set; }
        Func<ServerError, string, Exception> CreateResponseException { get; set; }
        bool SupportsContentModified { get; set; }
        TimeSpan MaximumRequestTimeout { get; set; }
        void RegisterForDisposal(IDisposable disposable);
        IDisposable RegisteredDisposables { get; }
        IEnumerable<Assembly> Assemblies { get; }
    }
}

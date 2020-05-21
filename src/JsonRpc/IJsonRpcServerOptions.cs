using System;
using System.IO.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc.Server;

namespace OmniSharp.Extensions.JsonRpc
{
    public interface IJsonRpcServerOptions
    {
        PipeReader Input { get; set; }
        PipeWriter Output { get; set; }
        IServiceCollection Services { get; set; }
        IRequestProcessIdentifier RequestProcessIdentifier { get; set; }
        int? Concurrency { get; set; }
        Func<ServerError, IHandlerDescriptor, Exception> OnServerError { get; set; }
        bool SupportsContentModified { get; set; }
    }
}

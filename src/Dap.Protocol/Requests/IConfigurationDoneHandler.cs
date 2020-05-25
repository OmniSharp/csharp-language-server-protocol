using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ConfigurationDone, Direction.ClientToServer)]
    public interface
        IConfigurationDoneHandler : IJsonRpcRequestHandler<ConfigurationDoneArguments, ConfigurationDoneResponse>
    {
    }

    public abstract class ConfigurationDoneHandler : IConfigurationDoneHandler
    {
        public abstract Task<ConfigurationDoneResponse> Handle(ConfigurationDoneArguments request,
            CancellationToken cancellationToken);
    }

    public static class ConfigurationDoneExtensions
    {
        public static IDebugAdapterServerRegistry OnConfigurationDone(this IDebugAdapterServerRegistry registry,
            Func<ConfigurationDoneArguments, CancellationToken, Task<ConfigurationDoneResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ConfigurationDone, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnConfigurationDone(this IDebugAdapterServerRegistry registry,
            Func<ConfigurationDoneArguments, Task<ConfigurationDoneResponse>> handler)
        {
            return registry.AddHandler(RequestNames.ConfigurationDone, RequestHandler.For(handler));
        }

        public static Task<ConfigurationDoneResponse> RequestConfigurationDone(this IDebugAdapterClient mediator, ConfigurationDoneArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}

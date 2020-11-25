using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Launch, Direction.ClientToServer)]
        [GenerateHandlerMethods(AllowDerivedRequests = true)]
        [GenerateRequestMethods]
        public interface ILaunchHandler<in T> : IJsonRpcRequestHandler<T, LaunchResponse> where T : LaunchRequestArguments
        {
        }

        public interface ILaunchHandler : ILaunchHandler<LaunchRequestArguments>
        {
        }

        public abstract class LaunchHandlerBase<T> : ILaunchHandler<T> where T : LaunchRequestArguments
        {
            public abstract Task<LaunchResponse> Handle(T request, CancellationToken cancellationToken);
        }

        public abstract class LaunchHandler : LaunchHandlerBase<LaunchRequestArguments>, ILaunchHandler
        {
        }
    }
}

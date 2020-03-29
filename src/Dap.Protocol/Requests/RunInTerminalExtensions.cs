using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public static class RunInTerminalExtensions
    {
        public static Task<RunInTerminalResponse> RunInTerminal(this IDebugAdapterClient mediator, RunInTerminalArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest<RunInTerminalArguments, RunInTerminalResponse>(RequestNames.RunInTerminal, @params, cancellationToken);
        }
    }

}

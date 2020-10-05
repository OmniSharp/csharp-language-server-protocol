using OmniSharp.Extensions.DebugAdapter.Client;

namespace Dap.Tests.Integration.Fixtures
{
    public sealed class DefaultClient : IConfigureDebugAdapterClientOptions
    {
        public void Configure(DebugAdapterClientOptions options)
        {
        }
    }
}

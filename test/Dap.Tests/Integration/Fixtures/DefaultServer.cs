using OmniSharp.Extensions.DebugAdapter.Server;

namespace Dap.Tests.Integration.Fixtures
{
    public sealed class DefaultServer : IConfigureDebugAdapterServerOptions
    {
        public DefaultServer()
        {
        }

        public void Configure(DebugAdapterServerOptions options)
        {
        }
    }
}
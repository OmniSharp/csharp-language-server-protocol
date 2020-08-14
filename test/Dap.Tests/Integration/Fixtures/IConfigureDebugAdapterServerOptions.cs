using OmniSharp.Extensions.DebugAdapter.Server;

namespace Dap.Tests.Integration.Fixtures
{
    public interface IConfigureDebugAdapterServerOptions
    {
        void Configure(DebugAdapterServerOptions options);
    }
}
using OmniSharp.Extensions.DebugAdapter.Client;

namespace Dap.Tests.Integration.Fixtures
{
    public interface IConfigureDebugAdapterClientOptions
    {
        void Configure(DebugAdapterClientOptions options);
    }
}
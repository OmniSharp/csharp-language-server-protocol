using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    internal class DebugAdapterSettingsBag : IDebugAdapterProtocolSettings
    {
        public InitializeRequestArguments ClientSettings { get; internal set; } = null!;

        public InitializeResponse ServerSettings { get; internal set; } = null!;
    }
}

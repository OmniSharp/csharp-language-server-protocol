using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    class DebugAdapterSettingsBag : IDebugAdapterProtocolSettings
    {
        public InitializeRequestArguments ClientSettings { get; internal set; }

        public InitializeResponse ServerSettings { get; internal set; }
    }
}

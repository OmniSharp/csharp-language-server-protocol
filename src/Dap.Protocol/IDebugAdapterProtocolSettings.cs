using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public interface IDebugAdapterProtocolSettings
    {
        InitializeRequestArguments ClientSettings { get; }

        InitializeResponse ServerSettings { get; }
    }
}

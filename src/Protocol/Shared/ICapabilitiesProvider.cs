using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Shared
{
    public interface ICapabilitiesProvider
    {
        T GetCapability<T>() where T : ICapability?;
        ICapability? GetCapability(Type type);
    }
}

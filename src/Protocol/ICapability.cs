namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ICapability<in TCapability>
    {
        void SetCapability(TCapability capability);
    }
}

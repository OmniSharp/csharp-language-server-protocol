namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ICapability<TCapability>
    {
        void SetCapability(TCapability capability);
    }
}
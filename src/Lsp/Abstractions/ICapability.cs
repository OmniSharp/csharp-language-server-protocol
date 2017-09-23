namespace OmniSharp.Extensions.LanguageServerProtocol.Abstractions
{
    public interface ICapability<TCapability>
    {
        void SetCapability(TCapability capability);
    }
}
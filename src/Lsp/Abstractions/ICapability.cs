namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    public interface ICapability<TCapability>
    {
        void SetCapability(TCapability capability);
    }
}
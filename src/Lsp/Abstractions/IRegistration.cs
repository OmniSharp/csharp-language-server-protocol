namespace OmniSharp.Extensions.LanguageServerProtocol.Abstractions
{
    public interface IRegistration<TOptions>
    {
        TOptions GetRegistrationOptions();
    }
}
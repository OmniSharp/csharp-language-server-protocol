namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    public interface IRegistration<TOptions>
    {
        TOptions GetRegistrationOptions();
    }
}
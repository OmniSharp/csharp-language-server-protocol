namespace OmniSharp.Extensions.LanguageServer.Abstractions
{
    public interface IRegistration<out TOptions>
    {
        TOptions GetRegistrationOptions();
    }
}

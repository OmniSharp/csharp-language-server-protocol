namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistration<out TOptions>
    {
        TOptions GetRegistrationOptions();
    }
}

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistration<out TOptions>
        where TOptions : class, new()
    {
        TOptions GetRegistrationOptions();
    }
}

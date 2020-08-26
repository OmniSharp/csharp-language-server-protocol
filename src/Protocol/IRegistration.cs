namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistration<out TOptions>
        where TOptions : class?, new()
    {
        TOptions GetRegistrationOptions();
    }

    /// <summary>
    /// Identifies a handler that does not participate in dynamic registration.
    /// </summary>
    public interface IDoesNotParticipateInRegistration
    {
    }
}

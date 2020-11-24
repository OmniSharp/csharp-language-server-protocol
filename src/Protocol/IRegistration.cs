using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistration<out TOptions, in TCapability>
        where TOptions : class
        where TCapability : ICapability
    {
        TOptions GetRegistrationOptions(TCapability capability);
    }

    public interface IRegistration<out TOptions>
        where TOptions : class
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

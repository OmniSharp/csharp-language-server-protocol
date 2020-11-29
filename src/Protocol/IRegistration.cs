using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IRegistration<out TOptions, in TCapability>
        where TOptions : class
        where TCapability : ICapability
    {
        TOptions GetRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities);
    }

    public interface IRegistration<out TOptions>
        where TOptions : class
    {
        TOptions GetRegistrationOptions(ClientCapabilities clientCapabilities);
    }

    public delegate TOptions RegistrationOptionsDelegate<out TOptions>(ClientCapabilities clientCapabilities)
        where TOptions : class;
    public delegate TOptions RegistrationOptionsDelegate<out TOptions, in TCapability>(TCapability capability, ClientCapabilities clientCapabilities)
        where TOptions : class
        where TCapability : ICapability;

    /// <summary>
    /// Identifies a handler that does not participate in dynamic registration.
    /// </summary>
    public interface IDoesNotParticipateInRegistration
    {
    }
}

using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class AbstractHandlers
    {
        public abstract class Base<TRegistrationOptions, TCapability> :
            IRegistration<TRegistrationOptions, TCapability>,
            ICapability<TCapability>
            where TRegistrationOptions : class, new()
            where TCapability : ICapability
        {
            protected TRegistrationOptions RegistrationOptions { get; private set; } = default!;
            protected TCapability Capability { get; private set; } = default!;
            protected ClientCapabilities ClientCapabilities { get; private set; } = default!;
            protected internal abstract TRegistrationOptions CreateRegistrationOptions(TCapability capability, ClientCapabilities clientCapabilities);

            TRegistrationOptions IRegistration<TRegistrationOptions, TCapability>.GetRegistrationOptions(
                TCapability capability, ClientCapabilities clientCapabilities
            )
            {
                // ReSharper disable twice ConditionIsAlwaysTrueOrFalse
                if (RegistrationOptions is not null && Capability is not null) return RegistrationOptions;
                Capability = capability;
                ClientCapabilities = clientCapabilities;
                return RegistrationOptions = CreateRegistrationOptions(capability, clientCapabilities);
            }

            void ICapability<TCapability>.SetCapability(TCapability capability, ClientCapabilities clientCapabilities)
            {
                ClientCapabilities = clientCapabilities;
                Capability = capability;
            }
        }

        public abstract class BaseCapability<TCapability> :
            ICapability<TCapability>
            where TCapability : ICapability
        {
            protected TCapability Capability { get; private set; } = default!;
            protected ClientCapabilities ClientCapabilities { get; private set; } = default!;

            void ICapability<TCapability>.SetCapability(TCapability capability, ClientCapabilities clientCapabilities)
            {
                ClientCapabilities = clientCapabilities;
                Capability = capability;
            }
        }

        public abstract class Base<TRegistrationOptions> :
            IRegistration<TRegistrationOptions>
            where TRegistrationOptions : class, new()
        {
            protected TRegistrationOptions RegistrationOptions { get; private set; } = default!;
            protected ClientCapabilities ClientCapabilities { get; private set; } = default!;
            protected abstract TRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities);

            TRegistrationOptions IRegistration<TRegistrationOptions>.GetRegistrationOptions(ClientCapabilities clientCapabilities)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (RegistrationOptions is not null) return RegistrationOptions;
                ClientCapabilities = clientCapabilities;
                return RegistrationOptions = CreateRegistrationOptions(clientCapabilities);
            }
        }
    }
}

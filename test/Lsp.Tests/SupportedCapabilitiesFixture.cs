using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace Lsp.Tests
{
    public class SupportedCapabilitiesFixture
    {
        public static readonly ISupportedCapabilities AlwaysTrue = new AlwaysTrueSupportedCapabilities();
        public static readonly ISupportedCapabilities AlwaysFalse = new AlwaysFalseSupportedCapabilities();

        private class AlwaysTrueSupportedCapabilities : SupportedCapabilitiesBase, ISupportedCapabilities
        {
            // ReSharper disable once UnusedParameter.Local
            // ReSharper disable once UnusedMember.Local
            public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor) => true;

            public bool AllowsDynamicRegistration(Type capabilityType) => true;

            protected override bool TryGetCapability(Type capabilityType, [NotNullWhen(true)] out object? capability)
            {
                capability = Activator.CreateInstance(capabilityType);
                return true;
            }
        }

        private class AlwaysFalseSupportedCapabilities : SupportedCapabilitiesBase, ISupportedCapabilities
        {
            // ReSharper disable once UnusedParameter.Local
            // ReSharper disable once UnusedMember.Local
            public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor) => false;

            public bool AllowsDynamicRegistration(Type capabilityType) => false;

            protected override bool TryGetCapability(Type capabilityType, [NotNullWhen(true)] out object? capability)
            {
                capability = null;
                return false;
            }
        }
    }
}

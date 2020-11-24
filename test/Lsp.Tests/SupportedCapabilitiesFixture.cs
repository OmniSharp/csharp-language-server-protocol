using System;
using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace Lsp.Tests
{
    public class SupportedCapabilitiesFixture
    {
        public static readonly ISupportedCapabilities AlwaysTrue = new AlwaysTrueSupportedCapabilities();
        public static readonly ISupportedCapabilities AlwaysFalse = new AlwaysFalseSupportedCapabilities();

        private class AlwaysTrueSupportedCapabilities : ISupportedCapabilities
        {
            // ReSharper disable once UnusedParameter.Local
            // ReSharper disable once UnusedMember.Local
            public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor) => true;

            public bool AllowsDynamicRegistration(Type capabilityType) => true;
            public void SetCapability(ILspHandlerTypeDescriptor descriptor, IJsonRpcHandler handler) {}

            public object? GetRegistrationOptions(ILspHandlerTypeDescriptor handlerTypeDescriptor, IJsonRpcHandler handler) => null;

            public void Add(IEnumerable<ISupports> supports)
            {
            }

            public void Add(ICapability capability) {}
        }

        private class AlwaysFalseSupportedCapabilities : ISupportedCapabilities
        {
            // ReSharper disable once UnusedParameter.Local
            // ReSharper disable once UnusedMember.Local
            public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor) => false;

            public bool AllowsDynamicRegistration(Type capabilityType) => false;
            public void SetCapability(ILspHandlerTypeDescriptor descriptor, IJsonRpcHandler handler) {}

            public object? GetRegistrationOptions(ILspHandlerTypeDescriptor handlerTypeDescriptor, IJsonRpcHandler handler) => null;

            public void Add(IEnumerable<ISupports> supports)
            {
            }

            public void Add(ICapability capability) {}
        }
    }
}

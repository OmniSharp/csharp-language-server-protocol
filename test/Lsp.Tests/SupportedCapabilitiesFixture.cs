using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;

namespace Lsp.Tests
{
    public class SupportedCapabilitiesFixture
    {
        public static readonly ISupportedCapabilities AlwaysTrue = new AlwaysTrueSupportedCapabilities();
        public static readonly ISupportedCapabilities AlwaysFalse = new AlwaysFalseSupportedCapabilities();
        class AlwaysTrueSupportedCapabilities : ISupportedCapabilities
        {
            public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor) => true;

            public bool AllowsDynamicRegistration(Type capabilityType) => true;

            public void SetCapability(ILspHandlerDescriptor descriptor, IJsonRpcHandler handler)
            {
            }
        }

        class AlwaysFalseSupportedCapabilities : ISupportedCapabilities
        {
            public bool AllowsDynamicRegistration(ILspHandlerDescriptor descriptor) => false;

            public bool AllowsDynamicRegistration(Type capabilityType) => false;

            public void SetCapability(ILspHandlerDescriptor descriptor, IJsonRpcHandler handler)
            {
            }
        }
    }
}

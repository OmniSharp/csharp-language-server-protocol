using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Lsp.Integration.Tests.Fixtures
{
    [Parallel]
    [Method("tests/discover", Direction.ClientToServer)]
    [GenerateHandler]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
    [GenerateRequestMethods(typeof(ILanguageClient))]
    [RegistrationOptions(typeof(UnitTestRegistrationOptions))]
    [Capability(typeof(UnitTestCapability))]
    public partial class DiscoverUnitTestsParams : IPartialItemsRequest<Container<UnitTest>, UnitTest>, IWorkDoneProgressParams
    {
    }

    [Parallel]
    [Method("tests/run", Direction.ClientToServer)]
    [GenerateHandler(Name = "RunUnitTest")]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
    [GenerateRequestMethods(typeof(ILanguageClient))]
    [RegistrationOptions(typeof(UnitTestRegistrationOptions))]
    [Capability(typeof(UnitTestCapability))]
    public partial class UnitTest : IJsonRpcRequest, IDoesNotParticipateInRegistration
    {
        public string Name { get; set; } = null!;
    }

    [CapabilityKey("workspace", "unitTests")]
    public partial class UnitTestCapability : DynamicCapability
    {
        public string Property { get; set; } = null!;
    }

    [GenerateRegistrationOptions("unitTestDiscovery")]
    [RegistrationName("tests")]
    public partial class UnitTestRegistrationOptions : IWorkDoneProgressOptions
    {
        [Optional] public bool SupportsDebugging { get; set; }
    }
}

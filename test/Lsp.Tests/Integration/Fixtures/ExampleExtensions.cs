using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method("tests/run", Direction.ClientToServer)]
    public class UnitTest : IRequest
    {
        public string Name { get; set; }
    }

    [CapabilityKey("workspace", "unitTests")]
    public  class UnitTestCapability : DynamicCapability
    {
        public string Property { get; set; }
    }

    public class UnitTestRegistrationOptions
    {

    }

    [Parallel, Method("tests/discover", Direction.ClientToServer)]
    public class DiscoverUnitTestsParams : IPartialItemsRequest<Container<UnitTest>, UnitTest>, IWorkDoneProgressParams
    {
        public ProgressToken PartialResultToken { get; set; }
        public ProgressToken WorkDoneToken { get; set; }
    }

    [Parallel, Method("tests/discover", Direction.ClientToServer)]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
    [GenerateRequestMethods(typeof(ILanguageClient))]
    public  interface IDiscoverUnitTestsHandler : IJsonRpcRequestHandler<DiscoverUnitTestsParams, Container<UnitTest>>, IRegistration<UnitTestRegistrationOptions>, ICapability<UnitTestCapability>
    {
    }

    [Parallel, Method("tests/run", Direction.ClientToServer)]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
    [GenerateRequestMethods(typeof(ILanguageClient))]
    public  interface IRunUnitTestHandler : IJsonRpcRequestHandler<UnitTest>, IRegistration<UnitTestRegistrationOptions>, ICapability<UnitTestCapability>
    {
    }
}

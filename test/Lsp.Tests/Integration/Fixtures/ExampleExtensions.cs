using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Lsp.Tests.Integration.Fixtures
{
    [Parallel, Method("tests/run", Direction.ClientToServer)]
    public class UnitTest : IRequest
    {
        public string Name { get; set; } = null!;
    }

    [CapabilityKey("workspace", "unitTests")]
    public class UnitTestCapability : DynamicCapability
    {
        public string Property { get; set; } = null!;
    }

    public class UnitTestRegistrationOptions : IWorkDoneProgressOptions, IRegistrationOptions
    {
        [Optional] public bool SupportsDebugging { get; set; }
        [Optional] public bool WorkDoneProgress { get; set; }

        public class StaticOptions : WorkDoneProgressOptions
        {
            [Optional] public bool SupportsDebugging { get; set; }
        }

        private class Converter : RegistrationOptionsConverterBase<UnitTestRegistrationOptions, StaticOptions>
        {
            public Converter() : base("unitTests")
            {
            }

            public override StaticOptions Convert(UnitTestRegistrationOptions source)
            {
                return new StaticOptions {
                    SupportsDebugging = source.SupportsDebugging,
                    WorkDoneProgress = source.WorkDoneProgress,
                };
            }
        }
    }

    [Parallel, Method("tests/discover", Direction.ClientToServer)]
    public class DiscoverUnitTestsParams : IPartialItemsRequest<Container<UnitTest>, UnitTest>, IWorkDoneProgressParams
    {
        public ProgressToken? PartialResultToken { get; set; } = null!;
        public ProgressToken? WorkDoneToken { get; set; } = null!;
    }

    [Parallel, Method("tests/discover", Direction.ClientToServer)]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
    [GenerateRequestMethods(typeof(ILanguageClient))]
    public interface IDiscoverUnitTestsHandler : IJsonRpcRequestHandler<DiscoverUnitTestsParams, Container<UnitTest>>, IRegistration<UnitTestRegistrationOptions>,
                                                 ICapability<UnitTestCapability>
    {
    }

    [Parallel, Method("tests/run", Direction.ClientToServer)]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
    [GenerateRequestMethods(typeof(ILanguageClient))]
    public interface IRunUnitTestHandler : IJsonRpcRequestHandler<UnitTest>, IRegistration<UnitTestRegistrationOptions>, ICapability<UnitTestCapability>
    {
    }
}

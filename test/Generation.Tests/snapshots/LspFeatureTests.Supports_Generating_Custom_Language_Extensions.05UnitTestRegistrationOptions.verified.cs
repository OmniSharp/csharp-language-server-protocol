//HintName: UnitTestRegistrationOptions.cs
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

#nullable enable
namespace Lsp.Tests.Integration.Fixtures
{
    [RegistrationOptionsKey("unitTestDiscovery")]
    [RegistrationOptionsConverterAttribute(typeof(UnitTestRegistrationOptionsConverter))]
    public partial class UnitTestRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        [Optional]
        public bool WorkDoneProgress { get; set; }

        class UnitTestRegistrationOptionsConverter : RegistrationOptionsConverterBase<UnitTestRegistrationOptions, StaticOptions>
        {
            public UnitTestRegistrationOptionsConverter()
            {
            }

            public override StaticOptions Convert(UnitTestRegistrationOptions source)
            {
                return new StaticOptions{SupportsDebugging = source.SupportsDebugging, WorkDoneProgress = source.WorkDoneProgress};
            }
        }

        [RegistrationOptionsKey("unitTestDiscovery")]
        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            [Optional]
            public bool SupportsDebugging { get; set; }

            [Optional]
            public bool WorkDoneProgress { get; set; }
        }
    }
}
#nullable restore

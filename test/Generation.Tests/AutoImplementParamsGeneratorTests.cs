using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
using TestingUtils;
using Xunit;

namespace Generation.Tests
{
    public class AutoImplementParamsGeneratorTests
    {
        [Fact]
        public async Task Auto_Magically_Implements_Properties()
        {
            var source = @"
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

#nullable enable
namespace Test
{
    [Method(""abcd"")]
    public partial class DeclarationParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<LocationOrLocationLinks, LocationOrLocationLink> { }
}
";
            var expected = @"
#nullable enable
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace Test
{
    public partial class DeclarationParams
    {
        [Optional]
        public ProgressToken? WorkDoneToken
        {
            get;
            init;
        }

        [Optional]
        public ProgressToken? PartialResultToken
        {
            get;
            init;
        }
    }
}
#nullable restore";
            CacheKeyHasher.Cache = true;
            await GenerationHelpers.AssertGeneratedAsExpected<AutoImplementParamsGenerator>(source, expected);
            await GenerationHelpers.AssertGeneratedAsExpected<AutoImplementParamsGenerator>(source, expected);
        }
    }
}

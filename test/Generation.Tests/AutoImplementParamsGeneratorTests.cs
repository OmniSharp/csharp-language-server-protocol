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
            await Verify(GenerationHelpers.GenerateAll(source));
        }
    }
}

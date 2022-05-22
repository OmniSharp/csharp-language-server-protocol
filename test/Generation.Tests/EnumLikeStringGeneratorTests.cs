using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using Xunit;

namespace Generation.Tests
{
    [UsesVerify]
    public class EnumLikeStringGeneratorTests
    {
        [Fact]
        public async Task Auto_Magically_Implements_IEnumLikeString()
        {
            var source = @"
using OmniSharp.Extensions.JsonRpc.Generation;
namespace Test {
    [StringEnum]
    public readonly partial struct ThreadEventReason
    {
        public static ThreadEventReason Started { get; } = new ThreadEventReason(""started"");
        public static ThreadEventReason Exited { get; } = new ThreadEventReason(""exited"");
    }
}
";
            await Verify(GenerationHelpers.Generate<EnumLikeStringGenerator>(source));
        }
    }
}

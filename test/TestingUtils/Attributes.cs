using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace TestingUtils
{
    public class SkipOnFactAttribute : FactAttribute
    {
        private readonly OSPlatform[] _platformsToSkip;

        public SkipOnFactAttribute(params OSPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string Skip { get => _platformsToSkip.Any(RuntimeInformation.IsOSPlatform) ? "Skipped on platform" : null; set { } }
    }

    public class SkipOnTheoryAttribute : TheoryAttribute
    {
        private readonly OSPlatform[] _platformsToSkip;

        public SkipOnTheoryAttribute(params OSPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string Skip { get => _platformsToSkip.Any(RuntimeInformation.IsOSPlatform) ? "Skipped on platform" : null; set { } }
    }
}

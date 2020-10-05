using System.Linq;
using Xunit;

namespace TestingUtils
{
    public class TheoryWithSkipOnAttribute : TheoryAttribute
    {
        private readonly SkipOnPlatform[] _platformsToSkip;
        private string? _skip;

        public TheoryWithSkipOnAttribute(params SkipOnPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string? Skip
        {
            get => /*!UnitTestDetector.IsCI() && */_platformsToSkip.Any(UnitTestDetector.PlatformToSkipPredicate)
                ? "Skipped on platform" + ( string.IsNullOrWhiteSpace(_skip) ? "" : " because " + _skip )
                : null;
            set => _skip = value;
        }
    }
}

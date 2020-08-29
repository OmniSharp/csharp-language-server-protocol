using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace TestingUtils
{
    public enum SkipOnPlatform
    {
        Linux,
        Mac,
        Windows,
    }

    public class FactWithSkipOnAttribute : FactAttribute
    {
        private readonly SkipOnPlatform[] _platformsToSkip;
        private string _skip;

        public FactWithSkipOnAttribute(params SkipOnPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string Skip
        {
            get => !UnitTestDetector.InUnitTestRunner() && _platformsToSkip.Any(UnitTestDetector.PlatformToSkipPredicate)
                ? "Skipped on platform" + ( string.IsNullOrWhiteSpace(_skip) ? "" : " because " + _skip )
                : null;
            set => _skip = value;
        }
    }

    public class TheoryWithSkipOnAttribute : TheoryAttribute
    {
        private readonly SkipOnPlatform[] _platformsToSkip;
        private string _skip;

        public TheoryWithSkipOnAttribute(params SkipOnPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string Skip
        {
            get => !UnitTestDetector.InUnitTestRunner() && _platformsToSkip.Any(UnitTestDetector.PlatformToSkipPredicate)
                ? "Skipped on platform" + ( string.IsNullOrWhiteSpace(_skip) ? "" : " because " + _skip )
                : null;
            set => _skip = value;
        }
    }

    class UnitTestDetector
    {
        private static bool? _inUnitTestRunner;

        public static bool InUnitTestRunner()
        {
            if (_inUnitTestRunner.HasValue) return _inUnitTestRunner.Value;

            var testAssemblies = new[] {
                "CSUNIT",
                "NUNIT",
                "XUNIT",
                "MBUNIT",
                "NBEHAVE",
                "VISUALSTUDIO.QUALITYTOOLS",
                "VISUALSTUDIO.TESTPLATFORM",
                "FIXIE",
                "NCRUNCH",
            };

            try
            {
                _inUnitTestRunner = SearchForAssembly(testAssemblies);
            }
            catch (Exception e)
            {
                _inUnitTestRunner = false;
            }

            return _inUnitTestRunner.Value;
        }

        public static bool PlatformToSkipPredicate(SkipOnPlatform platform) =>
            RuntimeInformation.IsOSPlatform(
                platform switch {
                    SkipOnPlatform.Linux   => OSPlatform.Linux,
                    SkipOnPlatform.Mac     => OSPlatform.OSX,
                    SkipOnPlatform.Windows => OSPlatform.Windows,
                    _                      => OSPlatform.Create("Unknown")
                }
            );

        private static bool SearchForAssembly(IEnumerable<string> assemblyList)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .Select(x => x.FullName.ToUpperInvariant())
                            .Any(x => assemblyList.Any(name => x.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) != -1));
        }
    }
}

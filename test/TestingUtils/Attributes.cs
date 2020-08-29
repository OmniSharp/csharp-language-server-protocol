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

        public FactWithSkipOnAttribute(params SkipOnPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string Skip
        {
            get => !UnitTestDetector.InUnitTestRunner() && _platformsToSkip.Any(
                platform => RuntimeInformation.IsOSPlatform(
                    platform switch {
                        SkipOnPlatform.Linux   => OSPlatform.Linux,
                        SkipOnPlatform.Mac     => OSPlatform.OSX,
                        SkipOnPlatform.Windows => OSPlatform.Windows,
                        _                      => OSPlatform.Create("Unknown")
                    }
                )
            )
                ? "Skipped on platform"
                : null;
            set { }
        }
    }

    public class TheoryWithSkipOnAttribute : TheoryAttribute
    {
        private readonly SkipOnPlatform[] _platformsToSkip;

        public TheoryWithSkipOnAttribute(params SkipOnPlatform[] platformsToSkip)
        {
            _platformsToSkip = platformsToSkip;
        }

        public override string Skip
        {
            get => !UnitTestDetector.InUnitTestRunner() && _platformsToSkip.Any(
                platform => RuntimeInformation.IsOSPlatform(
                    platform switch {
                        SkipOnPlatform.Linux   => OSPlatform.Linux,
                        SkipOnPlatform.Mac     => OSPlatform.OSX,
                        SkipOnPlatform.Windows => OSPlatform.Windows,
                        _                      => OSPlatform.Create("Unknown")
                    }
                )
            )
                ? "Skipped on platform"
                : null;
            set { }
        }
    }

    class UnitTestDetector
    {
        private static bool? _inUnitTestRunner;
        public static bool InUnitTestRunner()
        {
            if (_inUnitTestRunner.HasValue) return _inUnitTestRunner.Value;

            var testAssemblies = new[]
            {
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
                _inUnitTestRunner =  SearchForAssembly(testAssemblies);
            }
            catch (Exception e)
            {
                _inUnitTestRunner = false;
            }
            return _inUnitTestRunner.Value;
        }

        private static bool SearchForAssembly(IEnumerable<string> assemblyList)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .Select(x => x.FullName.ToUpperInvariant())
                            .Any(x => assemblyList.Any(name => x.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) != -1));
        }
    }
}

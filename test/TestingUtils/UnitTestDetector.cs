using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestingUtils
{
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

        public static bool IsCI() => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI"))
                                  && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TF_BUILD"));

        public static bool PlatformToSkipPredicate(SkipOnPlatform platform) =>
            RuntimeInformation.IsOSPlatform(
                platform switch {
                    {} v when v.HasFlag(SkipOnPlatform.Linux)   => OSPlatform.Linux,
                    {} v when v.HasFlag(SkipOnPlatform.Mac)     => OSPlatform.OSX,
                    {} v when v.HasFlag(SkipOnPlatform.Windows) => OSPlatform.Windows,
                    _                                                        => OSPlatform.Create("Unknown")
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

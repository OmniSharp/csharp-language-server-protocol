using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestingUtils
{
    internal class UnitTestDetector
    {
        // ReSharper disable once InconsistentNaming
        public static bool IsCI() => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CI"))
                                  || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TF_BUILD"));

        public static bool PlatformToSkipPredicate(SkipOnPlatform platform)
        {
            if (platform == SkipOnPlatform.All) return true;
            if (platform == SkipOnPlatform.None) return false;
            return Enum.GetValues(typeof(SkipOnPlatform))
                                .OfType<SkipOnPlatform>()
                                .Where(z => z != SkipOnPlatform.All && z != SkipOnPlatform.None)
                                .Where(z => ( platform & z ) == z)
                                .Any(
                                     z => RuntimeInformation.IsOSPlatform(
                                         platform switch {
                                             SkipOnPlatform.Linux   => OSPlatform.Linux,
                                             SkipOnPlatform.Mac     => OSPlatform.OSX,
                                             SkipOnPlatform.Windows => OSPlatform.Windows,
                                             _                      => OSPlatform.Create("Unknown")
                                         }
                                     )
                                 );
        }
    }
}

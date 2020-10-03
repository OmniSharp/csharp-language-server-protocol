using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestingUtils
{
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
            get => !UnitTestDetector.IsCI() && _platformsToSkip.Any(UnitTestDetector.PlatformToSkipPredicate)
                ? "Skipped on platform" + ( string.IsNullOrWhiteSpace(_skip) ? "" : " because " + _skip )
                : null;
            set => _skip = value;
        }
    }

    public static class TestHelper
    {
        public static async Task DelayUntil<T>(Func<T> valueFunc, Func<T, bool> func, CancellationToken cancellationToken, TimeSpan? delay = null)
        {
            while (true)
            {
                if (func(valueFunc())) return;
                await Task.Delay(delay ?? TimeSpan.FromMilliseconds(100), cancellationToken);
            }
        }

        public static async Task DelayUntil(Func<bool> func, CancellationToken cancellationToken, TimeSpan? delay = null)
        {
            while (true)
            {
                if (func()) return;
                await Task.Delay(delay ?? TimeSpan.FromMilliseconds(100), cancellationToken);
            }
        }

        public static Task DelayUntil<T>(this T value, Func<T, bool> func, CancellationToken cancellationToken, TimeSpan? delay = null)
        {
            return DelayUntil(() => value, func, cancellationToken, delay);
        }

        public static Task DelayUntilCount<T>(this T value, int count, CancellationToken cancellationToken, TimeSpan? delay = null) where T : IEnumerable
        {
            return DelayUntil(() => value.OfType<object>().Count() >= count, cancellationToken, delay);
        }
    }
}

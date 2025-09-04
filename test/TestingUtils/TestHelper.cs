namespace TestingUtils
{
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

        public static Task DelayUntilCount<T>(this IEnumerable<T> value, int count, CancellationToken cancellationToken, TimeSpan? delay = null)
        {
            return DelayUntil(() => {
                try
                {
                    return value.Count() >= count;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }, cancellationToken, delay);
        }

        public static Task DelayUntilCount<T>(this ICollection<T> value, int count, CancellationToken cancellationToken, TimeSpan? delay = null)
        {
            return DelayUntil(() => value.Count >= count, cancellationToken, delay);
        }

        public static Task DelayUntilCount<T>(this T[] value, int count, CancellationToken cancellationToken, TimeSpan? delay = null)
        {
            return DelayUntil(() => value.Length >= count, cancellationToken, delay);
        }
    }
}
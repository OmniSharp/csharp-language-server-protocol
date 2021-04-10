using System.Reactive.Concurrency;
using System.Reactive.PlatformServices;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TestingUtils
{
    class XUnitPlatformEnlightenmentProvider : IPlatformEnlightenmentProvider
    {
        private readonly IPlatformEnlightenmentProvider _defaultEnlightenmentProvider;
//        private AsyncLocal<SynchronizationContextScheduler> _localScheduler = new AsyncLocal<SynchronizationContextScheduler>();

//        [ModuleInitializer]
        internal static void Initializer()
        {
            if (PlatformEnlightenmentProvider.Current is XUnitPlatformEnlightenmentProvider) return;
            PlatformEnlightenmentProvider.Current = new XUnitPlatformEnlightenmentProvider(PlatformEnlightenmentProvider.Current);
        }

        public XUnitPlatformEnlightenmentProvider(IPlatformEnlightenmentProvider defaultEnlightenmentProvider)
        {
            _defaultEnlightenmentProvider = defaultEnlightenmentProvider;
        }
        public T GetService<T>(params object[]? args) where T : class
        {
            if (typeof(T) == typeof(IScheduler) && args != null)
            {
//                _localScheduler.Value ??= new SynchronizationContextScheduler(SynchronizationContext.Current, true);
                return (T)(object)new SynchronizationContextScheduler(SynchronizationContext.Current);
            }

            return _defaultEnlightenmentProvider.GetService<T>(args);
        }
    }
}

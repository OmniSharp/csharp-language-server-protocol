namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static partial class LanguageProtocolDelegatingHandlers
    {
        public sealed class TypedPartialObserver<T, TR> : IObserver<IEnumerable<T>>
        {
            private readonly IObserver<IEnumerable<TR>> _results;
            private readonly Func<T, TR> _factory;

            public TypedPartialObserver(IObserver<IEnumerable<TR>> results, Func<T, TR> factory)
            {
                _results = results;
                _factory = factory;
            }

            public void OnCompleted() => _results.OnCompleted();

            public void OnError(Exception error) => _results.OnError(error);

            public void OnNext(IEnumerable<T> value) => _results.OnNext(value.Select(_factory));
        }
    }
}

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    public class CacheContainer<T> where T : SyntaxNode
    {
        private ImmutableDictionary<string, ImmutableArray<SourceTextCache>> _sourceTexts;
        private ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>> _cacheDiagnostics;

        public CacheContainer()
        {
            _sourceTexts = ImmutableDictionary<string, ImmutableArray<SourceTextCache>>.Empty;
            _cacheDiagnostics = ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>>.Empty;
        }

        public ImmutableDictionary<string, ImmutableArray<SourceTextCache>> SourceTexts => _sourceTexts;
        public ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>> Diagnostics => _cacheDiagnostics;

        public void Swap(
            ImmutableDictionary<string, (T syntaxNode, ImmutableArray<SourceTextCache>.Builder sources)>.Builder foundCache,
            ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>.Builder>.Builder diagnosticFactories
        )
        {
            Interlocked.Exchange(
                ref _sourceTexts,
                foundCache.ToImmutableDictionary(z => z.Key, z => z.Value.sources.ToImmutable())
            );
            Interlocked.Exchange(
                ref _cacheDiagnostics,
                diagnosticFactories.ToImmutableDictionary(z => z.Key, z => z.Value.ToImmutable())
            );
        }
    }
}

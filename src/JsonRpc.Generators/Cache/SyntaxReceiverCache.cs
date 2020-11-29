using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    public abstract class SyntaxReceiverCache<T> : IReceiverCache<T>, ISyntaxReceiver
        where T : SyntaxNode
    {
        private readonly CacheContainer<T> _cache;
        private readonly ImmutableDictionary<string, (T syntaxNode, ImmutableArray<SourceTextCache>.Builder sources)>.Builder _foundSourceTexts;
        private readonly ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>.Builder>.Builder _foundDiagnosticFactories;
        private readonly List<SourceTextCache> _cachedSources = new();
        private readonly List<Diagnostic> _cachedDiagnostics = new();

        protected SyntaxReceiverCache(CacheContainer<T> cache)
        {
            _cache = cache;
            _foundSourceTexts = ImmutableDictionary<string, (T syntaxNode, ImmutableArray<SourceTextCache>.Builder sources)>.Empty.ToBuilder();
            _foundDiagnosticFactories = ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>.Builder>.Empty.ToBuilder();
        }

        public abstract string? GetKey(T syntax);

        public void Start(GeneratorExecutionContext context)
        {
            // TODO: Check if options disable cache
            try
            {
                // check stuff
                _cache.Swap(_foundSourceTexts, _foundDiagnosticFactories);
            }
            catch
            {
                _cachedSources.Clear();
                _cachedDiagnostics.Clear();
                foreach (var found in _foundSourceTexts.Values)
                {
                    OnVisitNode(found.Item1);
                }
            }
        }

        public void Finish(GeneratorExecutionContext context)
        {
            // TODO: Check if options disable cache
            try
            {
                // check stuff
                _cache.Swap(_foundSourceTexts, _foundDiagnosticFactories);
            }
            catch
            {
                _cache.Swap(
                    ImmutableDictionary<string, (T syntaxNode, ImmutableArray<SourceTextCache>.Builder sources)>.Empty.ToBuilder(),
                    ImmutableDictionary<string, ImmutableArray<CacheDiagnosticFactory<T>>.Builder>.Empty.ToBuilder()
                );
            }
        }

        public IEnumerable<SourceTextCache> CachedSources => _cachedSources;
        public IEnumerable<Diagnostic> CachedDiagnostics => _cachedDiagnostics;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not T v) return;
            if (GetKey(v) is { } key)
            {
                if (_cache.SourceTexts.TryGetValue(key, out var cacheValue))
                {
                    _foundSourceTexts.Add(key, ( v, cacheValue.ToBuilder() ));
                    _cachedSources.AddRange(cacheValue);
                }

                if (_cache.Diagnostics.TryGetValue(key, out var diagnostics))
                {
                    _foundDiagnosticFactories.Add(key, diagnostics.ToBuilder());
                    _cachedDiagnostics.AddRange(diagnostics.Select(f => f(v)));
                    return;
                }

                if (_foundSourceTexts.ContainsKey(key)) return;
            }

            OnVisitNode(v);
        }

        public void AddCacheSource(string hintName, T syntaxNode, SourceText sourceText)
        {
            if (GetKey(syntaxNode) is not { } key) return;
            if (!_foundSourceTexts.TryGetValue(key, out var data))
            {
                var array = ImmutableArray.Create(new SourceTextCache(hintName, sourceText)).ToBuilder();
                _foundSourceTexts.Add(key, ( syntaxNode, array ));
            }
            else
            {
                data.sources.Add(new SourceTextCache(hintName, sourceText));
            }
        }

        public void ReportCacheDiagnostic(T syntaxNode, CacheDiagnosticFactory<T> diagnostic)
        {
            if (GetKey(syntaxNode) is not { } key) return;
            if (!_foundDiagnosticFactories.TryGetValue(key, out var array))
            {
                array = ImmutableArray.Create(diagnostic).ToBuilder();
                _foundDiagnosticFactories.Add(key, array);
            }
            else
            {
                array.Add(diagnostic);
            }
        }

        public abstract void OnVisitNode(T syntaxNode);
    }
}

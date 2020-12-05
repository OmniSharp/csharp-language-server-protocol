using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    public record SourceTextCache(string Name, SourceText SourceText);
    public record DiagnosticCache<T>(ImmutableArray<CacheDiagnosticFactory<T>> Diagnostics) where T : SyntaxNode;
}

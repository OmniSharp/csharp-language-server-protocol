using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    public interface IReceiverCache<T>
        where T : SyntaxNode
    {
        string? GetKey(T syntax);
        void Start(GeneratorExecutionContext context);
        void Finish(GeneratorExecutionContext context);
        IEnumerable<SourceTextCache> CachedSources { get; }
        IEnumerable<Diagnostic> CachedDiagnostics { get; }
        void AddCacheSource(string hintName, T syntaxNode, SourceText sourceText);
        void ReportCacheDiagnostic(T syntaxNode, CacheDiagnosticFactory<T> diagnostic);
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    public delegate void AddCacheSource<in T>(string hintName, T syntaxNode, SourceText sourceText) where T : SyntaxNode;

    public delegate void ReportCacheDiagnostic<T>(T syntaxNode, CacheDiagnosticFactory<T> diagnostic) where T : SyntaxNode;

    public delegate Diagnostic CacheDiagnosticFactory<in T>(T syntaxNode) where T : SyntaxNode;

    public delegate Location LocationFactory<in T>(T syntaxNode) where T : SyntaxNode;
}

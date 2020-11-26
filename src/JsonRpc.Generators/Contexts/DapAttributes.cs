using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record DapAttributes()
    {
        public static DapAttributes? Parse(
            GeneratorExecutionContext context,
            AddCacheSource<TypeDeclarationSyntax> addCacheSource,
            ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic,
            TypeDeclarationSyntax syntax,
            INamedTypeSymbol symbol
            )
        {
            return null;
        }
    }
}

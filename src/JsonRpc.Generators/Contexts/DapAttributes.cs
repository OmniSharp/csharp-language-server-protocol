using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record DapAttributes
    {
        public static DapAttributes? Parse(
            Compilation compilation,
            TypeDeclarationSyntax candidateClass,
            SemanticModel model,
            INamedTypeSymbol symbol
            )
        {
            return null;
        }
    }
}

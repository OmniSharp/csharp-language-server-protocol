using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record SyntaxSymbol(TypeSyntax Syntax, INamedTypeSymbol Symbol);
}
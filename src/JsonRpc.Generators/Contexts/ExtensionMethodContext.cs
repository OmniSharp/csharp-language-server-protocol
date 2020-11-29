using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record ExtensionMethodContext(
        AttributeData AttributeData,
        TypeDeclarationSyntax TypeDeclaration,
        INamedTypeSymbol TypeSymbol,
        TypeSyntax Item,
        ImmutableArray<TypeSyntax> RelatedItems,
        GeneratorExecutionContext Context
    )
    {
        public bool IsProxy { get; init; }
        public bool IsRegistry { get; init; }
    }
}

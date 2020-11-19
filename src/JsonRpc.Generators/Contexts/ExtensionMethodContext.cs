using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record ExtensionMethodContext(
        AttributeData AttributeData,
        TypeDeclarationSyntax TypeDeclaration,
        INamedTypeSymbol TypeSymbol,
        NameSyntax Item,
        ImmutableArray<NameSyntax> RelatedItems,
        GeneratorExecutionContext Context
    )
    {
        public string GetSendMethodName() => Helpers.GetSendMethodName(TypeSymbol, AttributeData);
        public string GetOnMethodName() => Helpers.GetOnMethodName(TypeSymbol, AttributeData);
        public bool IsProxy { get; init; }
        public bool IsRegistry { get; init; }
    }
}

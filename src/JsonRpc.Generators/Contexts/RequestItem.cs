using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record RequestItem(
        TypeDeclarationSyntax TypeDeclaration,
        INamedTypeSymbol TypeSymbol,
        JsonRpcAttributes JsonRpcAttributes,
        LspAttributes? LspAttributes,
        DapAttributes? DapAttributes,
        SyntaxSymbol Request,
        SyntaxSymbol Response,
        bool IsUnit,
        SyntaxSymbol? Capability,
        SyntaxSymbol? RegistrationOptions,
        SyntaxSymbol? PartialItem,
        SyntaxSymbol? PartialItems,
        HashSet<string> AdditionalUsings,
        List<AttributeArgumentSyntax> AssemblyJsonRpcHandlersAttributeArguments,
        SemanticModel Model,
        Compilation Compilation
    ) : GeneratorData(
        TypeDeclaration, TypeSymbol,
        JsonRpcAttributes, LspAttributes, DapAttributes, Request, Capability, RegistrationOptions,
        AdditionalUsings, AssemblyJsonRpcHandlersAttributeArguments, Model, Compilation
    );

//    record PartialItem(TypeSyntax Syntax, INamedTypeSymbol Symbol, SyntaxSymbol Item) : SyntaxSymbol(Syntax, Symbol);
}

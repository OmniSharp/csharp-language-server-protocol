using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;

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
        SemanticModel Model,
        GeneratorExecutionContext Context
    ) : GeneratorData(
        TypeDeclaration, TypeSymbol,
        JsonRpcAttributes, LspAttributes, DapAttributes, Request, Capability, RegistrationOptions,
        AdditionalUsings, Model, Context
    );

//    record PartialItem(TypeSyntax Syntax, INamedTypeSymbol Symbol, SyntaxSymbol Item) : SyntaxSymbol(Syntax, Symbol);
}

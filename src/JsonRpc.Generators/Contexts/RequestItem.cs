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
        SyntaxSymbol? Capability,
        SyntaxSymbol? RegistrationOptions,
        SyntaxSymbol? PartialItem,
        SyntaxSymbol? PartialItems,
            HashSet<string> AdditionalUsings,
            SemanticModel Model,
        GeneratorExecutionContext Context
    ) : ExtensionMethodData(
        TypeDeclaration, TypeSymbol,
        JsonRpcAttributes, LspAttributes, DapAttributes, Request, Capability, RegistrationOptions,
        AdditionalUsings, Model, Context
    );
}

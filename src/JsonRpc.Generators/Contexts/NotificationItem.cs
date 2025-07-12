using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record NotificationItem(
        TypeDeclarationSyntax TypeDeclaration,
        INamedTypeSymbol TypeSymbol,
        JsonRpcAttributes JsonRpcAttributes,
        LspAttributes? LspAttributes,
        DapAttributes? DapAttributes,
        SyntaxSymbol Request,
        SyntaxSymbol? Capability,
        SyntaxSymbol? RegistrationOptions,
        HashSet<string> AdditionalUsings,
        List<AttributeArgumentSyntax> AssemblyJsonRpcHandlersAttributeArguments,
        SemanticModel Model,
        Compilation Compilation
    ) : GeneratorData(
        TypeDeclaration, TypeSymbol, JsonRpcAttributes, LspAttributes,
        DapAttributes, Request, Capability, RegistrationOptions,
        AdditionalUsings, AssemblyJsonRpcHandlersAttributeArguments, Model, Compilation
    );
}

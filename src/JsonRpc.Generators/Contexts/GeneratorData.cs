using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    internal abstract record GeneratorData(
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
    )
    {
        public static GeneratorData? Create(
            Compilation compilation,
            TypeDeclarationSyntax candidateClass,
            SemanticModel model,
            HashSet<string> additionalUsings
        )
        {
            var symbol = model.GetDeclaredSymbol(candidateClass) is { } nts ? nts : null;
            if (symbol == null) return null;
            var requestType = GetRequestType(candidateClass, symbol);
            if (requestType == null) return null;
            var jsonRpcAttributes = JsonRpcAttributes.Parse(compilation, candidateClass, model, symbol, additionalUsings);
            var lspAttributes = LspAttributes.Parse(compilation, candidateClass, model, symbol);
            var dapAttributes = DapAttributes.Parse(compilation, candidateClass, model, symbol);

            additionalUsings.Add(jsonRpcAttributes.HandlerNamespace);
            additionalUsings.Add(jsonRpcAttributes.ModelNamespace);

            if (IsRequest(candidateClass))
            {
                var responseType = GetResponseType(candidateClass, symbol);
                return new RequestItem(
                    candidateClass,
                    symbol,
                    jsonRpcAttributes,
                    lspAttributes,
                    dapAttributes,
                    requestType,
                    responseType,
                    responseType.Syntax.GetSyntaxName() == "Unit",
                    GetCapability(candidateClass, symbol, lspAttributes),
                    GetRegistrationOptions(candidateClass, symbol, lspAttributes),
                    GetPartialItem(candidateClass, symbol, requestType),
                    GetPartialItems(candidateClass, symbol, requestType),
                    symbol.AllInterfaces.Concat(requestType.Symbol.AllInterfaces).Any(z => z.Name.EndsWith("WithInitialValue", StringComparison.Ordinal)),
                    additionalUsings,
                    new List<AttributeArgumentSyntax>(),
                    model, compilation
                );
            }

            if (IsNotification(candidateClass))
            {
                return new NotificationItem(
                    candidateClass,
                    symbol,
                    jsonRpcAttributes,
                    lspAttributes,
                    dapAttributes,
                    requestType,
                    GetCapability(candidateClass, symbol, lspAttributes),
                    GetRegistrationOptions(candidateClass, symbol, lspAttributes),
                    additionalUsings,
                    new List<AttributeArgumentSyntax>(),
                    model, compilation
                );
            }

            return null;
        }

        public static RequestItem? CreateForResolver(GeneratorData parent)
        {
            if (parent.LspAttributes?.Resolver?.Symbol.DeclaringSyntaxReferences
                      .FirstOrDefault(
                           z => z.GetSyntax() is TypeDeclarationSyntax { AttributeLists: { Count: > 0 } } tds
                             && tds.AttributeLists.ContainsAttribute("GenerateHandler")
                       )?.GetSyntax() is TypeDeclarationSyntax declarationSyntax)
            {
                return Create(
                    parent.Compilation, declarationSyntax, parent.Compilation.GetSemanticModel(declarationSyntax.SyntaxTree), parent.AdditionalUsings
                ) as RequestItem;
            }

            return null;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    abstract record GeneratorData(
        TypeDeclarationSyntax TypeDeclaration,
        INamedTypeSymbol TypeSymbol,
        JsonRpcAttributes JsonRpcAttributes,
        LspAttributes? LspAttributes,
        DapAttributes? DapAttributes,
        SyntaxSymbol Request,
        SyntaxSymbol? Capability,
        SyntaxSymbol? RegistrationOptions,
        HashSet<string> AdditionalUsings,
        SemanticModel Model,
        GeneratorExecutionContext Context
    )
    {
        public static GeneratorData? Create(GeneratorExecutionContext context, TypeDeclarationSyntax candidateClass, HashSet<string> additionalUsings)
        {
            var model = context.Compilation.GetSemanticModel(candidateClass.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(candidateClass);
            if (symbol == null) return null;
            var requestType = GetRequestType(candidateClass, symbol);
            if (requestType == null) return null;
            var jsonRpcAttributes = JsonRpcAttributes.Parse(context, candidateClass, symbol, additionalUsings);
            var lspAttributes = LspAttributes.Parse(context, candidateClass, symbol);
            var dapAttributes = DapAttributes.Parse(context, candidateClass, symbol);

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
                    responseType.Symbol.Name == "Unit",
                    GetCapability(candidateClass, symbol, lspAttributes),
                    GetRegistrationOptions(candidateClass, symbol, lspAttributes),
                    GetPartialItem(candidateClass, symbol, requestType),
                    GetPartialItems(candidateClass, symbol, requestType),
                    additionalUsings,
                    model,
                    context
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
                    model,
                    context
                );
            }

            return null;
        }

        public static RequestItem? CreateForResolver(GeneratorData parent)
        {
            if (parent.LspAttributes?.Resolver?.Symbol.DeclaringSyntaxReferences
                      .FirstOrDefault(
                           z => z.GetSyntax() is TypeDeclarationSyntax { AttributeLists: { Count: > 0 } } tds
                             && tds.AttributeLists.SelectMany(z => z.Attributes).Any(z => z.Name.ToFullString().Contains("GenerateHandler"))
                       )?.GetSyntax() is TypeDeclarationSyntax declarationSyntax)
            {
                return Create(parent.Context, declarationSyntax, parent.AdditionalUsings) as RequestItem;
            }

            return null;
        }
    }
}

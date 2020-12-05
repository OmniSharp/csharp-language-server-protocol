using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
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
        List<AttributeArgumentSyntax> AssemblyJsonRpcHandlersAttributeArguments,
        SemanticModel Model,
        GeneratorExecutionContext Context
    )
    {
        private AddCacheSource<TypeDeclarationSyntax> AddCacheSourceDelegate { get; init; }
        private ReportCacheDiagnostic<TypeDeclarationSyntax> CacheDiagnosticDelegate { get; init; }

        public void AddSource(string hintName, SourceText sourceText)
        {
            AddCacheSourceDelegate(hintName, TypeDeclaration, sourceText);
        }

        public void ReportDiagnostic(CacheDiagnosticFactory<TypeDeclarationSyntax> diagnostic)
        {
            CacheDiagnosticDelegate(TypeDeclaration, diagnostic);
        }

        public static GeneratorData? Create(
            GeneratorExecutionContext context,
            TypeDeclarationSyntax candidateClass,
            AddCacheSource<TypeDeclarationSyntax> addCacheSource,
            ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic,
            HashSet<string> additionalUsings
        )
        {
            var model = context.Compilation.GetSemanticModel(candidateClass.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(candidateClass);
            if (symbol == null) return null;
            var requestType = GetRequestType(candidateClass, symbol);
            if (requestType == null) return null;
            var jsonRpcAttributes = JsonRpcAttributes.Parse(context, addCacheSource, cacheDiagnostic, candidateClass, symbol, additionalUsings);
            var lspAttributes = LspAttributes.Parse(context, addCacheSource, cacheDiagnostic, candidateClass, symbol);
            var dapAttributes = DapAttributes.Parse(context, addCacheSource, cacheDiagnostic, candidateClass, symbol);

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
                    additionalUsings,
                    new List<AttributeArgumentSyntax>(),
                    model,
                    context
                ) { CacheDiagnosticDelegate = cacheDiagnostic, AddCacheSourceDelegate = addCacheSource };
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
                    model,
                    context
                ) { CacheDiagnosticDelegate = cacheDiagnostic, AddCacheSourceDelegate = addCacheSource };
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
                return Create(parent.Context, declarationSyntax, parent.AddCacheSourceDelegate, parent.CacheDiagnosticDelegate, parent.AdditionalUsings) as RequestItem;
            }

            return null;
        }
    }
}

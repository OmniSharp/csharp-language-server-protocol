using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class WarnIfResponseRouterIsNotProvidedStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (extensionMethodContext is not { IsProxy: true }) yield break;

            var generateRequestMethods = item.JsonRpcAttributes.GenerateRequestMethods;
            if (generateRequestMethods != null && (
                    generateRequestMethods.Data.ConstructorArguments.Length == 0 ||
                    ( generateRequestMethods.Data.ConstructorArguments[0].Kind != TypedConstantKind.Array
                   && generateRequestMethods.Data.ConstructorArguments[0].Value == null )
                 || ( generateRequestMethods.Data.ConstructorArguments[0].Kind == TypedConstantKind.Array
                   && generateRequestMethods.Data.ConstructorArguments[0].Values.Length == 0 ) ) && !extensionMethodContext.TypeSymbol.ContainingNamespace
                   .ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.NoResponseRouterProvided, item.TypeDeclaration.Identifier.GetLocation(), item.TypeDeclaration.Identifier.Text
                    )
                );
            }
        }
    }
}

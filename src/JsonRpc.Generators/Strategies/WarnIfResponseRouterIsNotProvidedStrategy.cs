using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class WarnIfResponseRouterIsNotProvidedStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, ExtensionMethodData item)
        {
            if (extensionMethodContext is not { IsProxy: true }) yield break;

            var generateRequestMethods = item.JsonRpcAttributes.GenerateRequestMethods;
            if (generateRequestMethods != null && (
                                                      generateRequestMethods.ConstructorArguments.Length == 0 ||
                                                      generateRequestMethods.ConstructorArguments[0].Kind != TypedConstantKind.Array
                                                   && generateRequestMethods.ConstructorArguments[0].Value == null
                                                   || generateRequestMethods.ConstructorArguments[0].Kind == TypedConstantKind.Array
                                                   && generateRequestMethods.ConstructorArguments[0].Values.Length == 0 )
                                               && !extensionMethodContext.TypeSymbol.ContainingNamespace.ToDisplayString().StartsWith("OmniSharp.Extensions.DebugAdapter.Protocol"))
            {
                item.Context.ReportDiagnostic(
                    Diagnostic.Create(
                        GeneratorDiagnostics.NoResponseRouterProvided, extensionMethodContext.TypeDeclaration.Identifier.GetLocation(), extensionMethodContext.TypeSymbol.Name,
                        string.Join(", ", item.JsonRpcAttributes.HandlerRegistries.Select(z => z.ToFullString()))
                    )
                );
            }
        }
    }
}

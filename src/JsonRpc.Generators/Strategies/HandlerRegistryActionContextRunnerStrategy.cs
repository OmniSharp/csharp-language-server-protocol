using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class HandlerRegistryActionContextRunnerStrategy : IExtensionMethodGeneratorStrategy
    {
        private readonly ImmutableArray<IExtensionMethodContextGeneratorStrategy> _strategies;

        public HandlerRegistryActionContextRunnerStrategy(ImmutableArray<IExtensionMethodContextGeneratorStrategy> strategies)
        {
            _strategies = strategies;
        }
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, GeneratorData item)
        {
            foreach (var diagnostic in item.JsonRpcAttributes.HandlerRegistryDiagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
            return item.JsonRpcAttributes.HandlerRegistries
                       .Select(
                            registry => new ExtensionMethodContext(
                                item.JsonRpcAttributes.GenerateHandlerMethods!.Data, item.TypeDeclaration, item.TypeSymbol, registry, item.JsonRpcAttributes.HandlerRegistries
                            ) { IsRegistry = true }
                        )
                       .SelectMany(_ => _strategies, (actionContext, strategy) => new { actionContext, strategy })
                       .SelectMany(@t => @t.strategy.Apply(context, @t.actionContext, item));
        }
    }
}

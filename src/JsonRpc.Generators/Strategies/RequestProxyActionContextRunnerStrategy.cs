using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class RequestProxyActionContextRunnerStrategy : IExtensionMethodGeneratorStrategy
    {
        private readonly ImmutableArray<IExtensionMethodContextGeneratorStrategy> _strategies;

        public RequestProxyActionContextRunnerStrategy(ImmutableArray<IExtensionMethodContextGeneratorStrategy> strategies)
        {
            _strategies = strategies;
        }
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, GeneratorData item)
        {
            foreach (var diagnostic in item.JsonRpcAttributes.RequestProxyDiagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
            return item.JsonRpcAttributes.RequestProxies
                       .Select(
                            registry => new ExtensionMethodContext(
                                item.JsonRpcAttributes.GenerateRequestMethods!.Data, item.TypeDeclaration, item.TypeSymbol, registry, item.JsonRpcAttributes.RequestProxies
                            ) { IsProxy = true }
                        )
                       .SelectMany(_ => _strategies, (actionContext, strategy) => new { actionContext, strategy })
                       .SelectMany(t => t.strategy.Apply(context, t.actionContext, item));
        }
    }
}

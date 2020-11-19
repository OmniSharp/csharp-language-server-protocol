using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodData item)
        {
            return item.JsonRpcAttributes.RequestProxies
                       .Select(
                            registry => new ExtensionMethodContext(
                                item.JsonRpcAttributes.GenerateRequestMethods!, item.TypeDeclaration, item.TypeSymbol, registry, item.JsonRpcAttributes.RequestProxies,
                                item.Context
                            ) { IsProxy = true }
                        )
                       .SelectMany(_ => _strategies, (actionContext, strategy) => new { actionContext, strategy })
                       .SelectMany(t => t.strategy.Apply(t.actionContext, item));
        }
    }
}

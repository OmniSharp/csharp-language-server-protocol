using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodData item)
        {
            return item.JsonRpcAttributes.HandlerRegistries
                       .Select(
                            registry => new ExtensionMethodContext(
                                item.JsonRpcAttributes.GenerateHandlerMethods!, item.TypeDeclaration, item.TypeSymbol, registry, item.JsonRpcAttributes.HandlerRegistries,
                                item.Context
                            ) { IsRegistry = true }
                        )
                       .SelectMany(actionContext => _strategies, (actionContext, strategy) => new { actionContext, strategy })
                       .SelectMany(@t => @t.strategy.Apply(@t.actionContext, item));
        }
    }
}

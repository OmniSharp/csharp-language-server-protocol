using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class EnsureNamespaceStrategy : IExtensionMethodGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, GeneratorData item)
        {
            if (item.Capability != null) item.AdditionalUsings.Add(item.Capability.Symbol.ContainingNamespace.ToDisplayString());
            if (item.RegistrationOptions != null) item.AdditionalUsings.Add(item.RegistrationOptions.Symbol.ContainingNamespace.ToDisplayString());
            if (item is RequestItem requestItem)
            {
                if (requestItem.PartialItem != null) item.AdditionalUsings.Add(requestItem.PartialItem.Symbol.ContainingNamespace.ToDisplayString());
                if (requestItem.PartialItems != null) item.AdditionalUsings.Add(requestItem.PartialItems.Symbol.ContainingNamespace.ToDisplayString());
            }

            yield break;
        }
    }
}

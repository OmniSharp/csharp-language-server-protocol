using System.Linq;
using Microsoft.CodeAnalysis;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    static class SymbolExtensions
    {
        public static AttributeData? GetAttribute(this INamedTypeSymbol symbol, INamedTypeSymbol? attributeSymbol)
        {
            if (attributeSymbol is null) return null;
            return symbol?.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, attributeSymbol));
        }
    }
}
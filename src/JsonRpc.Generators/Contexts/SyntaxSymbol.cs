using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Contexts
{
    record SyntaxSymbol(TypeSyntax Syntax, ITypeSymbol Symbol);

    record SyntaxAttributeData(AttributeSyntax Syntax, AttributeData Data)
    {
        public static SyntaxAttributeData? Parse(AttributeData? data)
        {
            var syntax = data?.ApplicationSyntaxReference?.GetSyntax() ;
            if (syntax is AttributeSyntax attributeSyntax)
                return new SyntaxAttributeData(attributeSyntax, data!);

            return null;
        }
    }
}

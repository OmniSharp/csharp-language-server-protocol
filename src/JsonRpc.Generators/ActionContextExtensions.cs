using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    static class ActionContextExtensions
    {
        public static ParameterListSyntax GetRegistryParameterList(this ExtensionMethodContext extensionMethodContext)
        {
            return SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    new[] {
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("registry"))
                                     .WithType(extensionMethodContext.Item)
                                     .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                    }
                )
            );
        }
    }
}

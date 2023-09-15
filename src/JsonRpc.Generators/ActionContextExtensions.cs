using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    static class ActionContextExtensions
    {
        public static ParameterListSyntax GetRegistryParameterList(this ExtensionMethodContext extensionMethodContext)
        {
            return ParameterList(
                SeparatedList(
                    new[] {
                        Parameter(Identifier("registry"))
                                     .WithType(extensionMethodContext.Item)
                                     .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                    }
                )
            );
        }
    }
}

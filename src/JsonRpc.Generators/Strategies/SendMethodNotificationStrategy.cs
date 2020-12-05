using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class SendMethodNotificationStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not NotificationItem notification) yield break;
            if (extensionMethodContext is not { IsProxy: true }) yield break;

            var method = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), item.JsonRpcAttributes.RequestMethodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithExpressionBody(Helpers.GetNotificationInvokeExpression())
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            yield return method
                        .WithParameterList(
                             ParameterList(
                                 SeparatedList(
                                     new[] {
                                         Parameter(Identifier("mediator"))
                                            .WithType(extensionMethodContext.Item)
                                            .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                                         Parameter(Identifier("request"))
                                            .WithType(notification.Request.Syntax)
                                     }
                                 )
                             )
                         );
        }
    }
}

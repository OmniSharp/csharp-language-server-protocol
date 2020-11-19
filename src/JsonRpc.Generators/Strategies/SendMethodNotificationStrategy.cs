using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class SendMethodNotificationStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, ExtensionMethodData item)
        {
            if (item is not NotificationItem notification) yield break;
            if (extensionMethodContext is not { IsProxy: true }) yield break;

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), extensionMethodContext.GetSendMethodName())
                                      .WithModifiers(
                                           SyntaxFactory.TokenList(
                                               SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                               SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                           )
                                       )
                                      .WithExpressionBody(Helpers.GetNotificationInvokeExpression())
                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            yield return method
                        .WithParameterList(
                             SyntaxFactory.ParameterList(
                                 SyntaxFactory.SeparatedList(
                                     new[] {
                                         SyntaxFactory.Parameter(SyntaxFactory.Identifier("mediator"))
                                                      .WithType(extensionMethodContext.Item)
                                                      .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword))),
                                         SyntaxFactory.Parameter(SyntaxFactory.Identifier("request"))
                                                      .WithType(notification.Request.Syntax)
                                     }
                                 )
                             )
                         )
                        .NormalizeWhitespace();
        }
    }
}

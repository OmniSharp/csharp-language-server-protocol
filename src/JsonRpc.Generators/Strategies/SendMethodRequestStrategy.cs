using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class SendMethodRequestStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsProxy: true }) yield break;

            var parameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    new[] {
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("mediator"))
                                     .WithType(extensionMethodContext.Item)
                                     .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword))),
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("request"))
                                     .WithType(request.Request.Syntax),
                        SyntaxFactory.Parameter(SyntaxFactory.Identifier("cancellationToken"))
                                     .WithType(SyntaxFactory.IdentifierName("CancellationToken"))
                                     .WithDefault(
                                          SyntaxFactory.EqualsValueClause(
                                              SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxFactory.Token(SyntaxKind.DefaultKeyword))
                                          )
                                      )
                    }
                )
            );

            if (request.PartialItem is not null)
            {
                request.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
                yield return SyntaxFactory.MethodDeclaration(
                                               SyntaxFactory.GenericName(
                                                                 SyntaxFactory.Identifier("IRequestProgressObservable")
                                                             )
                                                            .WithTypeArgumentList(
                                                                 SyntaxFactory.TypeArgumentList(
                                                                     SyntaxFactory.SeparatedList(
                                                                         new[] {
                                                                             request.PartialItem.Syntax,
                                                                             request.Response!.Syntax
                                                                         }
                                                                     )
                                                                 )
                                                             ),
                                               SyntaxFactory.Identifier(item.JsonRpcAttributes.RequestMethodName)
                                           )
                                          .WithModifiers(
                                               SyntaxFactory.TokenList(
                                                   SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                   SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                               )
                                           )
                                          .WithParameterList(parameterList)
                                          .WithExpressionBody(Helpers.GetPartialInvokeExpression(request.Response.Syntax))
                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                          .NormalizeWhitespace();
                yield break;
            }

            if (request.PartialItems is not null)
            {
                request.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
                var partialItemsSyntax = SyntaxFactory.GenericName("IEnumerable").WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(new[] { request.PartialItems!.Syntax })));
                yield return SyntaxFactory.MethodDeclaration(
                                               SyntaxFactory.GenericName(
                                                                 SyntaxFactory.Identifier("IRequestProgressObservable")
                                                             )
                                                            .WithTypeArgumentList(
                                                                 SyntaxFactory.TypeArgumentList(
                                                                     SyntaxFactory.SeparatedList(
                                                                         new TypeSyntax[] {
                                                                             partialItemsSyntax,
                                                                             request.Response!.Syntax
                                                                         }
                                                                     )
                                                                 )
                                                             ),
                                               SyntaxFactory.Identifier(item.JsonRpcAttributes.RequestMethodName)
                                           )
                                          .WithModifiers(
                                               SyntaxFactory.TokenList(
                                                   SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                                   SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                               )
                                           )
                                          .WithParameterList(parameterList)
                                          .WithExpressionBody(Helpers.GetPartialInvokeExpression(request.Response.Syntax))
                                          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                          .NormalizeWhitespace();
                ;
                yield break;
            }


            var responseSyntax = request.Response!.Symbol.Name.EndsWith("Unit")
                ? SyntaxFactory.IdentifierName("Task") as NameSyntax
                : SyntaxFactory.GenericName("Task").WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(new[] { request.Response.Syntax })));
            yield return SyntaxFactory.MethodDeclaration(responseSyntax, item.JsonRpcAttributes.RequestMethodName)
                                      .WithModifiers(
                                           SyntaxFactory.TokenList(
                                               SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                               SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                           )
                                       )
                                      .WithParameterList(parameterList)
                                      .WithExpressionBody(Helpers.GetRequestInvokeExpression())
                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                                      .NormalizeWhitespace();
        }
    }
}

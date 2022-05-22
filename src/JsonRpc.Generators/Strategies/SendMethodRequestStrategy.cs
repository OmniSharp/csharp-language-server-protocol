using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class SendMethodRequestStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsProxy: true }) yield break;

            var parameterList = ParameterList(
                SeparatedList(
                    new[]
                    {
                        Parameter(Identifier("mediator"))
                           .WithType(extensionMethodContext.Item)
                           .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                        Parameter(Identifier("request"))
                           .WithType(request.Request.Syntax),
                        Parameter(Identifier("cancellationToken"))
                           .WithType(IdentifierName("CancellationToken"))
                           .WithDefault(
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
                                )
                            )
                    }
                )
            );

            if (request.PartialItem is not null)
            {
                request.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
                yield return MethodDeclaration(
                                 GenericName(
                                         Identifier("IRequestProgressObservable")
                                     )
                                    .WithTypeArgumentList(
                                         TypeArgumentList(
                                             SeparatedList(
                                                 new[]
                                                 {
                                                     request.PartialItem.Syntax,
                                                     request.Response!.Syntax
                                                 }
                                             )
                                         )
                                     ),
                                 Identifier(item.JsonRpcAttributes.RequestMethodName)
                             )
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithParameterList(parameterList)
                            .WithExpressionBody(
                                 Helpers.GetPartialInvokeExpression(
                                     request.Response.Syntax,
                                     request.PartialHasInitialValue ? null : request.PartialItem.Syntax,
                                     request.PartialItemInheritsFromSelf
                                 )
                             )
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                yield break;
            }

            if (request.PartialItems is not null)
            {
                request.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
                var partialItemsSyntax =
                    GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { request.PartialItems!.Syntax })));
                yield return MethodDeclaration(
                                 GenericName(
                                         Identifier("IRequestProgressObservable")
                                     )
                                    .WithTypeArgumentList(
                                         TypeArgumentList(
                                             SeparatedList(
                                                 new[]
                                                 {
                                                     partialItemsSyntax,
                                                     request.Response!.Syntax
                                                 }
                                             )
                                         )
                                     ),
                                 Identifier(item.JsonRpcAttributes.RequestMethodName)
                             )
                            .WithModifiers(
                                 TokenList(
                                     Token(SyntaxKind.PublicKeyword),
                                     Token(SyntaxKind.StaticKeyword)
                                 )
                             )
                            .WithParameterList(parameterList)
                            .WithExpressionBody(Helpers.GetPartialInvokeExpression(request.Response.Syntax, default, false))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
                yield break;
            }


            var responseSyntax = request.Response.Syntax.GetSyntaxName() == "Unit"
                ? IdentifierName("Task") as NameSyntax
                : GenericName("Task").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { request.Response.Syntax })));
            yield return MethodDeclaration(responseSyntax, item.JsonRpcAttributes.RequestMethodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithTypeParameterList(
                             item is RequestItem { Response: { Symbol: ITypeParameterSymbol } } ri
                                 ? TypeParameterList(SingletonSeparatedList(TypeParameter(ri.Response.Symbol.Name)))
                                 : null
                         )
                        .WithParameterList(parameterList)
                        .WithExpressionBody(Helpers.GetRequestInvokeExpression())
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }
    }
}

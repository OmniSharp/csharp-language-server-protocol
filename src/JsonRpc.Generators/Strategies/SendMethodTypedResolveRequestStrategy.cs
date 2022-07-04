using System.Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies;

internal class SendMethodTypedResolveRequestStrategy : IExtensionMethodContextGeneratorStrategy
{
    public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, ExtensionMethodContext extensionMethodContext, GeneratorData item)
    {
        if (item is not RequestItem request) yield break;
        if (extensionMethodContext is not { IsProxy: true }) yield break;
        if (item.JsonRpcAttributes.AllowDerivedRequests) yield break;
        if (item is not { LspAttributes: { Resolver: { } } }) yield break;
        if (request.Response.Syntax.GetSyntaxName() == "Unit") yield break;
//        if (request.Request.Symbol.Name != "OutlayHintParams") yield break;

        TypeSyntax requestType = item.Request.Syntax;
        TypeSyntax responseType = GenericName(Identifier(request.Response.Symbol.Name))
           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));

        responseType = request.Response.Syntax is NullableTypeSyntax ? NullableType(responseType) : responseType;

        var parameterList = ParameterList(
            SeparatedList(
                new[]
                {
                    Parameter(Identifier("mediator"))
                       .WithType(extensionMethodContext.Item)
                       .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword))),
                    Parameter(Identifier("request"))
                       .WithType(requestType),
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
//
//        if (request.PartialItem is not null)
//        {
//            request.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
//            yield return MethodDeclaration(
//                                           GenericName(
//                                                             Identifier("IRequestProgressObservable")
//                                                         )
//                                                        .WithTypeArgumentList(
//                                                             TypeArgumentList(
//                                                                 SeparatedList(
//                                                                     new[]
//                                                                     {
//                                                                         request.PartialItem.Syntax,
//                                                                         responseType
//                                                                     }
//                                                                 )
//                                                             )
//                                                         ),
//                                           Identifier(item.JsonRpcAttributes.RequestMethodName)
//                                       )
//                                      .WithModifiers(
//                                           TokenList(
//                                               Token(SyntaxKind.PublicKeyword),
//                                               Token(SyntaxKind.StaticKeyword)
//                                           )
//                                       )
//                                      .WithParameterList(parameterList)
//                                      .WithExpressionBody(
//                                           Helpers.GetPartialInvokeExpression(
//                                               request.Response.Syntax,
//                                               request.PartialHasInitialValue ? null : request.PartialItem.Syntax,
//                                               request.PartialItemInheritsFromSelf
//                                           )
//                                       )
//                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
//            yield break;
//        }
//
//        if (request.PartialItems is not null)
//        {
//            request.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Progress");
//            var partialItemsSyntax =
//                GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { request.PartialItems!.Syntax })));
//            yield return MethodDeclaration(
//                                           GenericName(
//                                                             Identifier("IRequestProgressObservable")
//                                                         )
//                                                        .WithTypeArgumentList(
//                                                             TypeArgumentList(
//                                                                 SeparatedList(
//                                                                     new[]
//                                                                     {
//                                                                         partialItemsSyntax,
//                                                                         responseType
//                                                                     }
//                                                                 )
//                                                             )
//                                                         ),
//                                           Identifier(item.JsonRpcAttributes.RequestMethodName)
//                                       )
//                                      .WithModifiers(
//                                           TokenList(
//                                               Token(SyntaxKind.PublicKeyword),
//                                               Token(SyntaxKind.StaticKeyword)
//                                           )
//                                       )
//                                      .WithParameterList(parameterList)
//                                      .WithExpressionBody(Helpers.GetPartialInvokeExpression(responseType, default, false))
//                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
//            yield break;
//        }


        var responseSyntax = GenericName("Task").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { responseType })));
        yield return MethodDeclaration(responseSyntax, item.JsonRpcAttributes.RequestMethodName)
                    .WithModifiers(
                         TokenList(
                             Token(SyntaxKind.PublicKeyword),
                             Token(SyntaxKind.StaticKeyword)
                         )
                     )
                    .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter("T"))))
                    .WithParameterList(parameterList)
                    .WithExpressionBody(Helpers.GetRequestReturningInvokeExpression(Helpers.GetJsonRpcMethodName(item.TypeDeclaration), responseType))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }
}

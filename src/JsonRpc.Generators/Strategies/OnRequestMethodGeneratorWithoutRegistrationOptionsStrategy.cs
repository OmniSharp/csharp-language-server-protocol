using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnRequestMethodGeneratorWithoutRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, ExtensionMethodData item)
        {
            if (item is { RegistrationOptions: {} }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var allowDerivedRequests = extensionMethodContext.AttributeData.NamedArguments
                                                    .Where(z => z.Key == "AllowDerivedRequests")
                                                    .Select(z => z.Value.Value)
                                                    .FirstOrDefault() is bool b && b;

            var method = SyntaxFactory.MethodDeclaration(extensionMethodContext.Item, extensionMethodContext.GetOnMethodName())
                                      .WithModifiers(
                                           SyntaxFactory.TokenList(
                                               SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                               SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                           )
                                       )
                                      .WithExpressionBody(Helpers.GetRequestHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration)))
                                      .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            var factory = DelegateHelpers.MakeMethodFactory(method, extensionMethodContext.GetRegistryParameterList());
            {

                yield return factory(DelegateHelpers.CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax));
                yield return factory(DelegateHelpers.CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax));
            }

            if (allowDerivedRequests)
            {
                static Func<TypeSyntax, MethodDeclarationSyntax> MakeGenericFactory(Func<TypeSyntax, MethodDeclarationSyntax> factory, TypeSyntax constraint)
                {
                    return syntax => factory(syntax)
                                    .WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.TypeParameter(SyntaxFactory.Identifier("T")))))
                                    .WithConstraintClauses(
                                         SyntaxFactory.SingletonList(
                                             SyntaxFactory.TypeParameterConstraintClause(SyntaxFactory.IdentifierName("T"))
                                                          .WithConstraints(SyntaxFactory.SingletonSeparatedList<TypeParameterConstraintSyntax>(SyntaxFactory.TypeConstraint(constraint)))
                                         )
                                     )
                                    .NormalizeWhitespace();
                }

                var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);

                yield return genericFactory( DelegateHelpers.CreateGenericAsyncFunc(false, request.Response.Syntax));
                yield return genericFactory( DelegateHelpers.CreateGenericAsyncFunc(true, request.Response.Syntax));
            }

            if (request.PartialItems is {} partialItems)
            {
                var partialItemsSyntax = SyntaxFactory.GenericName("IEnumerable").WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { partialItems.Syntax })));

                factory = DelegateHelpers.MakeMethodFactory(method.WithExpressionBody(Helpers.GetPartialResultsHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItems.Syntax, request.Response.Syntax)), extensionMethodContext.GetRegistryParameterList());

                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true));
                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItemsSyntax, false));
                if (request.Capability is {} capability)
                {
                    factory = DelegateHelpers.MakeMethodFactory(method.WithExpressionBody(
                                                                    Helpers.GetPartialResultsCapabilityHandlerExpression(
                                                                        Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax,
                                                                        partialItemsSyntax, capability.Syntax
                                                                    )
                                                                ), extensionMethodContext.GetRegistryParameterList());
                    yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItemsSyntax, capability.Syntax));
                }
            }

            if (request.PartialItem is {} partialItem)
            {
                factory = DelegateHelpers.MakeMethodFactory(method.WithExpressionBody(Helpers.GetPartialResultHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax)), extensionMethodContext.GetRegistryParameterList());
                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true));
                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItem.Syntax, false));

                if (request.Capability is {} capability)
                {
                    factory = DelegateHelpers.MakeMethodFactory(method.WithExpressionBody(
                                                                    Helpers.GetPartialResultCapabilityHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax, capability.Syntax)
                                                                ), extensionMethodContext.GetRegistryParameterList());
                    yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItem.Syntax, capability.Syntax));
                }
            }

            {
                if (request.Capability is { } capability)
                {
                        factory = DelegateHelpers.MakeMethodFactory(method.WithExpressionBody(
                                                                        Helpers.GetRequestCapabilityHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, capability.Syntax)
                                                                    ), extensionMethodContext.GetRegistryParameterList());

                        yield return factory(DelegateHelpers.CreateAsyncFunc(request.Response.Syntax, request.Request.Syntax, capability.Syntax));
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnRequestMethodGeneratorWithRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, ExtensionMethodData item)
        {
            if (item is { RegistrationOptions: null }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var allowDerivedRequests = extensionMethodContext.AttributeData.NamedArguments
                                                    .Where(z => z.Key == "AllowDerivedRequests")
                                                    .Select(z => z.Value.Value)
                                                    .FirstOrDefault() is bool b && b;

            var registrationOptions = request.RegistrationOptions!;

            var method = SyntaxFactory.MethodDeclaration(extensionMethodContext.Item, extensionMethodContext.GetOnMethodName())
                                      .WithModifiers(
                                           SyntaxFactory.TokenList(
                                               SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                               SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                                           )
                                       )
                                      .WithBody(Helpers.GetRequestRegistrationHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, registrationOptions.Syntax));
            if (request.Response.Syntax.ToFullString().EndsWith("Unit"))
            {
                method = method.WithBody(Helpers.GetVoidRequestRegistrationHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, registrationOptions.Syntax));
            }

            var factory = MakeFactory(method, extensionMethodContext.GetRegistryParameterList(), SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(item.RegistrationOptions.Syntax)));

            yield return factory(DelegateHelpers. CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax));
            yield return factory(DelegateHelpers. CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax));

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
                yield return genericFactory(DelegateHelpers.CreateGenericAsyncFunc(false, request.Response.Syntax));
                yield return genericFactory(DelegateHelpers.CreateGenericAsyncFunc(true, request.Response.Syntax));
            }

            if (request.PartialItems is {} partialItems)
            {
                var partialItemsSyntax = SyntaxFactory.GenericName("IEnumerable").WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { partialItems.Syntax })));
                factory = DelegateHelpers.MakeMethodFactory(method.WithBody(
                                                                Helpers.GetPartialResultsRegistrationHandlerExpression(
                                                                    Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, partialItems.Syntax,
                                                                    registrationOptions.Syntax
                                                                )
                                                            ), extensionMethodContext.GetRegistryParameterList());

                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItemsSyntax, false));
                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true));
                if (request.Capability is {} capability)
                {
                    factory = DelegateHelpers.MakeMethodFactory(method.WithBody(
                                                                    Helpers.GetPartialResultsRegistrationHandlerExpression(
                                                                        Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, partialItems.Syntax,
                                                                        registrationOptions.Syntax, capability.Syntax
                                                                    )
                                                                ), extensionMethodContext.GetRegistryParameterList());
                    yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItemsSyntax, capability.Syntax));
                }
            }

            if (request.PartialItem is {} partialItem)
            {

                factory = DelegateHelpers.MakeMethodFactory(method.WithBody(
                                                                Helpers.GetPartialResultRegistrationHandlerExpression(Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax, registrationOptions.Syntax)
                                                            ), extensionMethodContext.GetRegistryParameterList());

                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItem.Syntax, false));
                yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true));
                if (request.Capability is {} capability)
                {
                    factory = DelegateHelpers.MakeMethodFactory(method.WithBody(
                                                                    Helpers.GetPartialResultRegistrationHandlerExpression(
                                                                        Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax, registrationOptions.Syntax,
                                                                        capability.Syntax
                                                                    )
                                                                ), extensionMethodContext.GetRegistryParameterList());
                    yield return factory(Helpers.CreatePartialAction(request.Request.Syntax, partialItem.Syntax, capability.Syntax));
                }
            }

            {
                if (request.Capability is { } capability)
                {
                        factory = DelegateHelpers.MakeMethodFactory(method.WithBody(
                                                                        Helpers.GetRequestRegistrationHandlerExpression(
                                                                            Helpers.GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, registrationOptions.Syntax, capability.Syntax
                                                                        )
                                                                    ), extensionMethodContext.GetRegistryParameterList());

                    yield return factory(DelegateHelpers.CreateAsyncFunc(request.Response.Syntax, request.Request.Syntax, capability.Syntax));
                }
            }
        }

        private static Func<TypeSyntax, MethodDeclarationSyntax> MakeFactory(MethodDeclarationSyntax method, ParameterListSyntax preParameterList, TypeArgumentListSyntax typeArguments)
        {
            return DelegateHelpers.MakeMethodFactory(
                method, preParameterList, SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(
                            SyntaxFactory.Identifier("registrationOptionsFactory")
                        ).WithType(
                            SyntaxFactory.GenericName(
                                              SyntaxFactory.Identifier("Func")
                                          )
                                         .WithTypeArgumentList(typeArguments)
                        )
                    )
                )
            );
        }
    }
}

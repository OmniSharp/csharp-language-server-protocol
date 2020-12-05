using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;
using static OmniSharp.Extensions.JsonRpc.Generators.DelegateHelpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnRequestTypedResolveMethodGeneratorWithoutRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {

        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is { RegistrationOptions: { } }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;
            if (item is not { LspAttributes: { Resolver: {} } })  yield break;

            var resolver = GeneratorData.CreateForResolver(item)!;

            TypeSyntax requestType = request.Request.Syntax;
            TypeSyntax resolveType = GenericName(Identifier(resolver.Request.Symbol.Name))
               .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("T")
                        )
                    )
                );
            TypeSyntax responseType = GenericName(Identifier(request.Response.Symbol.Name))
               .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));

            // Special case... because the spec is awesome
            if (request.Response.Symbol.Name == "CommandOrCodeActionContainer")
            {
                responseType = GenericName(Identifier("CodeActionContainer"))
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
            }
            responseType = request.Response.Syntax is NullableTypeSyntax ? NullableType(responseType) : responseType;

            var method = MethodDeclaration(extensionMethodContext.Item, item.JsonRpcAttributes.HandlerMethodName)
                        .WithHandlerIdentityConstraint(true)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithExpressionBody(GetRequestHandlerExpression(request, GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration)))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            var methodFactory = MakeFactory(extensionMethodContext.GetRegistryParameterList());

            var factory = methodFactory(method);
            yield return factory(
                CreateAsyncFunc(responseType, false, requestType),
                CreateAsyncFunc(resolveType, false, resolveType)
            );
            yield return factory(
                CreateAsyncFunc(responseType, true, requestType),
                CreateAsyncFunc(resolveType, false, resolveType)
            );

            {
                if (request.Capability is { } capability)
                {
                    if (request.IsUnit)
                    {
                        factory = methodFactory(
                            method.WithExpressionBody(
                                GetVoidRequestCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), requestType, capability.Syntax
                                )
                            )
                        );
                    }
                    else
                    {
                        factory = methodFactory(
                            method.WithExpressionBody(
                                GetRequestCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), requestType, responseType, capability.Syntax
                                )
                            )
                        );
                    }

                    yield return factory(
                        CreateAsyncFunc(responseType, true, requestType, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax)
                    );
                }
            }

            if (request.PartialItems is { } partialItems)
            {
                var observerType = GenericName("IObserver")
                   .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                GenericName("IEnumerable")
                                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)))
                            )
                        )
                    );

                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithExpressionBody(
                            GetPartialResultsHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), requestType, partialItems.Syntax, responseType
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(requestType, observerType, false),
                    CreateAsyncFunc(resolveType, false, resolveType)
                );
                yield return factory(
                    CreatePartialAction(requestType, observerType, true),
                    CreateAsyncFunc(resolveType, true, resolveType)
                );

                if (request.Capability is { } capability)
                {
                    factory = methodFactory(
                        method
                           .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                           .WithExpressionBody(
                                GetPartialResultsCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), requestType, responseType,
                                    resolveType, capability.Syntax
                                )
                            )
                    );
                    yield return factory(
                        CreatePartialAction(requestType, observerType, true, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax)
                    );
                }
            }

            if (request.PartialItem is { })
            {
                var observerType = GenericName("IObserver").WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)));
                factory = methodFactory(
                    method.WithExpressionBody(
                        GetPartialResultHandlerExpression(
                            GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), requestType, observerType, responseType
                        )
                    )
                );
                yield return factory(
                    CreatePartialAction(requestType, observerType, false),
                    CreateAsyncFunc(resolveType, false, resolveType)
                );
                yield return factory(
                    CreatePartialAction(requestType, observerType, true),
                    CreateAsyncFunc(resolveType, true, resolveType)
                );

                if (request.Capability is { } capability)
                {
                    factory = methodFactory(
                        method.WithExpressionBody(
                            GetPartialResultCapabilityHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), requestType, observerType, responseType, capability.Syntax
                            )
                        )
                    );
                    yield return factory(
                        CreatePartialAction(requestType, observerType, true, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax)
                    );
                }
            }
        }

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, TypeSyntax?, MethodDeclarationSyntax>> MakeFactory(
            ParameterListSyntax preParameterList
        )
        {
            return method => (syntax, resolveSyntax) => GenerateMethods(method, syntax, resolveSyntax);

            MethodDeclarationSyntax MethodFactory(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax)
            {
                return method
                      .WithParameterList(
                           preParameterList
                              .AddParameters(Parameter(Identifier("handler")).WithType(syntax))
                              .AddParameters(
                                   resolveSyntax is { }
                                       ? new[] {
                                           Parameter(Identifier("resolveHandler"))
                                              .WithType(resolveSyntax)
                                       }
                                       : new ParameterSyntax[] { }
                               )
                       );
            }

            MethodDeclarationSyntax GenerateMethods(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax) => MethodFactory(method, syntax, resolveSyntax);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;
using static OmniSharp.Extensions.JsonRpc.Generators.DelegateHelpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnRequestMethodGeneratorWithoutRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is { RegistrationOptions: {}}) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var allowDerivedRequests = item.JsonRpcAttributes.AllowDerivedRequests;

            var method = MethodDeclaration(extensionMethodContext.Item, item.JsonRpcAttributes.HandlerMethodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithExpressionBody(GetRequestHandlerExpression(GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration)))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            var factory = MakeMethodFactory(method, extensionMethodContext.GetRegistryParameterList());
            {
                yield return factory(CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax));
                yield return factory(CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax));
            }

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);

                yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, false, IdentifierName("T")));
                yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, true, IdentifierName("T")));
            }

            if (request.PartialItems is { } partialItems)
            {
                var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { partialItems.Syntax })));

                factory = MakeMethodFactory(
                    method.WithExpressionBody(
                        GetPartialResultsHandlerExpression(
                            GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItems.Syntax, request.Response.Syntax
                        )
                    ), extensionMethodContext.GetRegistryParameterList()
                );

                yield return factory(CreatePartialAction(request.Request.Syntax, partialItemsSyntax, false));
                yield return factory(CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true));
                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, false));
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, true));
                }

                if (request.Capability is { } capability)
                {
                    factory = MakeMethodFactory(
                        method.WithExpressionBody(
                            GetPartialResultsCapabilityHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax,
                                partialItemsSyntax, capability.Syntax
                            )
                        ), extensionMethodContext.GetRegistryParameterList()
                    );
                    yield return factory(CreatePartialAction(request.Request.Syntax, partialItemsSyntax, capability.Syntax));

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, true, capability.Syntax));
                    }
                }
            }

            if (request.PartialItem is { } partialItem)
            {
                factory = MakeMethodFactory(
                    method.WithExpressionBody(
                        GetPartialResultHandlerExpression(
                            GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax
                        )
                    ), extensionMethodContext.GetRegistryParameterList()
                );
                yield return factory(CreatePartialAction(request.Request.Syntax, partialItem.Syntax, false));
                yield return factory(CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true));

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, false));
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true));
                }

                if (request.Capability is { } capability)
                {
                    factory = MakeMethodFactory(
                        method.WithExpressionBody(
                            GetPartialResultCapabilityHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax, capability.Syntax
                            )
                        ), extensionMethodContext.GetRegistryParameterList()
                    );
                    yield return factory(CreatePartialAction(request.Request.Syntax, partialItem.Syntax, capability.Syntax));

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true, capability.Syntax));
                    }
                }
            }

            {
                if (request.Capability is { } capability)
                {
                    if (request.IsUnit)
                    {
                        factory = MakeMethodFactory(
                            method.WithExpressionBody(
                                GetVoidRequestCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, capability.Syntax
                                )
                            ), extensionMethodContext.GetRegistryParameterList()
                        );
                    }
                    else
                    {
                        factory = MakeMethodFactory(
                            method.WithExpressionBody(
                                GetRequestCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, capability.Syntax
                                )
                            ), extensionMethodContext.GetRegistryParameterList()
                        );
                    }

                    yield return factory(CreateAsyncFunc(request.Response.Syntax, request.Request.Syntax, capability.Syntax));

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, IdentifierName("T"), capability.Syntax));
                    }
                }
            }
        }
    }
}

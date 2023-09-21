using System;
using System.Collections.Generic;
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
        private readonly bool _doResolve;

        public OnRequestMethodGeneratorWithoutRegistrationOptionsStrategy(bool doResolve)
        {
            _doResolve = doResolve;
        }

        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is { RegistrationOptions: { } }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var resolve = GeneratorData.CreateForResolver(item);
            if (!_doResolve) resolve = null;
            if (_doResolve && resolve is null) yield break;

            var allowDerivedRequests = item.JsonRpcAttributes.AllowDerivedRequests && !_doResolve;

            var method = MethodDeclaration(extensionMethodContext.Item, item.JsonRpcAttributes.HandlerMethodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithExpressionBody(GetRequestHandlerExpression(request, GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration)))
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            if (item is RequestItem { Response: { Symbol: ITypeParameterSymbol } } ri)
            {
                method = method.AddTypeParameterListParameters(TypeParameter(ri.Response.Symbol.Name));
            }

            var methodFactory = MakeFactory(extensionMethodContext.GetRegistryParameterList(), request.PartialHasInitialValue);

            var factory = methodFactory(method);
            yield return factory(
                CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax),
                resolve.ReturnIfNotNull(static r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax)),
                null
            );
            yield return factory(
                CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax),
                resolve.ReturnIfNotNull(static r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax)),
                null
            );

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);

                yield return genericFactory(
                    CreateAsyncFunc(request.Response.Syntax, false, IdentifierName("T")), null,
                    null
                );
                yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, true, IdentifierName("T")), null,
                                            null);
            }

            {
                if (request.Capability is { } capability)
                {
                    if (request.IsUnit)
                    {
                        factory = methodFactory(
                            method.WithExpressionBody(
                                GetVoidRequestCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, capability.Syntax
                                )
                            )
                        );
                    }
                    else
                    {
                        factory = methodFactory(
                            method.WithExpressionBody(
                                GetRequestCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, capability.Syntax
                                )
                            )
                        );
                    }

                    yield return factory(
                        CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax, capability.Syntax),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax)),
                        null
                    );

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(
                            CreateAsyncFunc(request.Response.Syntax, true, IdentifierName("T"), capability.Syntax),
                            resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax)),
                            null
                        );
                    }
                }
            }

            if (request.PartialItems is { } partialItems)
            {
                var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { partialItems.Syntax })));

                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithExpressionBody(
                            GetPartialResultsHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItems.Syntax, request.Response.Syntax, request.PartialHasInitialValue
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItemsSyntax, false),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax)),
                    CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax)
                );
                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax)),
                    CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)
                );
                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    yield return genericFactory(
                        CreatePartialAction(IdentifierName("T"), partialItemsSyntax, false),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax)),
                        CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax)
                    );
                    yield return genericFactory(
                        CreatePartialAction(IdentifierName("T"), partialItemsSyntax, true),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax)),
                        CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)
                    );
                }

                if (request.Capability is { } capability)
                {
                    factory = methodFactory(
                        method
                           .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                           .WithExpressionBody(
                                GetPartialResultsCapabilityHandlerExpression(
                                    GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax,
                                    partialItems.Syntax, capability.Syntax, request.PartialHasInitialValue
                                )
                            )
                    );
                    yield return factory(
                        CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true, capability.Syntax),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax)),
                        CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)
                    );

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(
                            CreatePartialAction(IdentifierName("T"), partialItemsSyntax, true, capability.Syntax),
                            resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax)),
                            CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)
                        );
                    }
                }
            }

            if (request.PartialItem is { } partialItem)
            {
                factory = methodFactory(
                    method.WithExpressionBody(
                        GetPartialResultHandlerExpression(
                            GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax, request.PartialHasInitialValue
                        )
                    )
                );
                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItem.Syntax, false),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax)),
                    CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax)
                );
                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax)),
                    CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)
                );

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, false), null,
                                                CreateAsyncFunc(request.Response.Syntax, false, request.Response.Syntax, partialItem.Syntax));
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true), null,
                                                CreateAsyncFunc(request.Response.Syntax, true, request.Response.Syntax, partialItem.Syntax));
                }

                if (request.Capability is { } capability)
                {
                    factory = methodFactory(
                        method.WithExpressionBody(
                            GetPartialResultCapabilityHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax, capability.Syntax, request.PartialHasInitialValue
                            )
                        )
                    );
                    yield return factory(
                        CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true, capability.Syntax),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax)),
                        CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)
                    );

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true, capability.Syntax), null,
                                                    CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax));
                    }
                }
            }
        }

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, TypeSyntax?, TypeSyntax?, MethodDeclarationSyntax>> MakeFactory(
            ParameterListSyntax preParameterList, bool partialHasInitialValue
        )
        {
            return method => (syntax, resolveSyntax, initialHandlerSyntax) => generateMethods(method, syntax, resolveSyntax, initialHandlerSyntax);

            MethodDeclarationSyntax methodFactory(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax, TypeSyntax? initialHandlerSyntax)
            {
                return method
                      .WithParameterList(
                           preParameterList
                              .AddParameters(
                                   partialHasInitialValue && initialHandlerSyntax is {}
                                       ? new[] {
                                           Parameter(Identifier("initialHandler"))
                                              .WithType(initialHandlerSyntax)
                                       }
                                       : Array.Empty<ParameterSyntax>()
                               )
                              .AddParameters(Parameter(Identifier("handler")).WithType(syntax))
                              .AddParameters(
                                   resolveSyntax is { }
                                       ? new[] {
                                           Parameter(Identifier("resolveHandler"))
                                              .WithType(resolveSyntax)
                                       }
                                       : Array.Empty<ParameterSyntax>()
                               )
                       );
            }

            MethodDeclarationSyntax generateMethods(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax, TypeSyntax? initialHandlerSyntax) => methodFactory(method, syntax, resolveSyntax, initialHandlerSyntax);
        }
    }
}

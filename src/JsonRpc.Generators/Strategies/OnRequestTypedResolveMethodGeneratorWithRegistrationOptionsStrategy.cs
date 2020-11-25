using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    internal class OnRequestTypedResolveMethodGeneratorWithRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not { RegistrationOptions: { } registrationOptions }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;
            if (item is not { LspAttributes: { Resolver: { } } }) yield break;
            if (request.IsUnit) yield break;

            var resolver = GeneratorData.CreateForResolver(item)!;

            TypeSyntax requestType = item.Request.Syntax;
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
                        .WithHandlerIdentityConstraint()
                        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                        .WithBody(
                             GetRequestHandlerExpression(
                                 request,
                                 requestType,
                                 responseType,
                                 resolveType,
                                 registrationOptions.Syntax,
                                 item.Capability?.Syntax
                             )
                         );

            var methodFactory = MakeFactory(
                extensionMethodContext.GetRegistryParameterList(),
                registrationOptions.Syntax,
                item.Capability?.Syntax
            );
            var factory = methodFactory(method);

            yield return factory(
                CreateAsyncFunc(responseType, false, requestType),
                CreateAsyncFunc(resolveType, false, resolveType)
            );
            yield return factory(
                CreateAsyncFunc(responseType, true, requestType),
                CreateAsyncFunc(resolveType, true, resolveType)
            );

            {
                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreateAsyncFunc(responseType, true, requestType, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax)
                    );
                }
            }

            if (request.PartialItems is { } partialItems)
            {
                var enumerableType = GenericName("IEnumerable")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)));
                var observerType = GenericName("IObserver")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(enumerableType)));
                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialHandlerExpression(
                                request,
                                requestType,
                                resolveType,
                                registrationOptions.Syntax,
                                item.Capability?.Syntax
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(requestType, enumerableType, false),
                    CreateAsyncFunc(resolveType, false, resolveType)
                );
                yield return factory(
                    CreatePartialAction(requestType, enumerableType, true),
                    CreateAsyncFunc(resolveType, true, resolveType)
                );

                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreatePartialAction(requestType, enumerableType, true, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax)
                    );
                }
            }

            if (request.PartialItem is { } partialItem )
            {
                var observerType = GenericName("IObserver").WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)));

                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialHandlerExpression(
                                request,
                                requestType,
                                resolveType,
                                registrationOptions.Syntax,
                                item.Capability?.Syntax
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(requestType, resolveType, false),
                    CreateAsyncFunc(resolveType, false, resolveType)
                );
                yield return factory(
                    CreatePartialAction(requestType, resolveType, true),
                    CreateAsyncFunc(resolveType, true, resolveType)
                );

                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreatePartialAction(requestType, resolveType, true, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax)
                    );
                }
            }
        }

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, TypeSyntax?, MethodDeclarationSyntax>> MakeFactory(
            ParameterListSyntax preParameterList, TypeSyntax registrationOptions, TypeSyntax? capabilityName
        )
        {
            return method => (syntax, resolveSyntax) => GenerateMethod(method, syntax, resolveSyntax);

            MethodDeclarationSyntax MethodFactory(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax, ParameterSyntax registrationParameter)
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
                              .AddParameters(registrationParameter)
                       )
                      .NormalizeWhitespace();
            }

            MethodDeclarationSyntax GenerateMethod(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax)
            {
                if (capabilityName is { })
                {
                    return MethodFactory(
                        method, syntax, resolveSyntax,
                        Parameter(Identifier("registrationOptions"))
                           .WithType(
                                GenericName(Identifier("Func")).WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList(new[] { capabilityName, registrationOptions }))
                                )
                            )
                    );
                }

                return MethodFactory(
                    method, syntax, resolveSyntax,
                    Parameter(Identifier("registrationOptions"))
                       .WithType(
                            GenericName(Identifier("Func"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(SingletonSeparatedList(registrationOptions))
                                )
                        )
                );
            }
        }

        private static BlockSyntax GetRequestHandlerExpression(
            RequestItem request,
            TypeSyntax requestSyntax,
            TypeSyntax responseSyntax,
            TypeSyntax resolveSyntax,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var typeArgs = ImmutableArray.Create(registrationOptions);
            var handlerAdapterArgs = ImmutableArray.Create(requestSyntax, responseSyntax);
            var resolveAdapterArgs = ImmutableArray.Create(resolveSyntax, resolveSyntax);
            if (capabilityName is { })
            {
                typeArgs = typeArgs.Add(capabilityName);
            }

            return Block(ReturnStatement(
                             AddHandler(
                                 Argument(
                                     CreateHandlerArgument($"Delegating{request.JsonRpcAttributes.HandlerName}Handler", IdentifierName("T"))
                                        .WithArgumentList(
                                             HandlerArgumentList(
                                                 IdentifierName("registrationOptions"),
                                                 registrationOptions,
                                                 capabilityName,
                                                 TypeArgumentList(SeparatedList(handlerAdapterArgs)),
                                                 TypeArgumentList(SeparatedList(resolveAdapterArgs)),
                                                 TypeArgumentList(SeparatedList(typeArgs.ToArray()))
                                             )
                                         )
                                 )
                             )
                         ));
        }

        private static BlockSyntax GetPartialHandlerExpression(
            RequestItem request,
            TypeSyntax requestSyntax,
            TypeSyntax resolveSyntax,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var typeArgs = ImmutableArray.Create(registrationOptions);
            var handlerAdapterArgs = ImmutableArray.Create(requestSyntax, resolveSyntax);
            var resolveAdapterArgs = ImmutableArray.Create(resolveSyntax, resolveSyntax);
            if (capabilityName is { })
            {
                typeArgs = typeArgs.Add(capabilityName);
            }

            return Block(ReturnStatement(
                             AddHandler(
                                 Argument(
                                     SimpleLambdaExpression(
                                         Parameter(
                                             Identifier("_")
                                         ),
                                     CreateHandlerArgument($"Delegating{request.JsonRpcAttributes.HandlerName}PartialHandler", IdentifierName("T"))
                                        .WithArgumentList(
                                             PartialHandlerArgumentList(
                                                 IdentifierName("registrationOptions"),
                                                 registrationOptions,
                                                 capabilityName,
                                                 TypeArgumentList(SeparatedList(handlerAdapterArgs)),
                                                 TypeArgumentList(SeparatedList(resolveAdapterArgs)),
                                                 TypeArgumentList(SeparatedList(typeArgs.ToArray()))
                                             )
                                         )
                                    )
                                 )
                             )
                         ));
        }

        public static ArgumentListSyntax HandlerArgumentList(
            TypeSyntax registrationOptionsName,
            TypeSyntax registrationType,
            TypeSyntax? capabilityName,
            TypeArgumentListSyntax handlerAdapterArgs,
            TypeArgumentListSyntax resolveAdapterArgs,
            TypeArgumentListSyntax registrationAdapterArgs
        ) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityName)),
                        GetHandlerAdapterArgument(
                            handlerAdapterArgs,
                            HandlerArgument,
                            capabilityName,
                            false
                        ),
                        GetHandlerAdapterArgument(
                            resolveAdapterArgs,
                            ResolveHandlerArgument,
                            capabilityName,
                            false
                        )
                    }
                )
            );

        public static ArgumentListSyntax PartialHandlerArgumentList(
            TypeSyntax registrationOptionsName,
            TypeSyntax registrationType,
            TypeSyntax? capabilityName,
            TypeArgumentListSyntax handlerAdapterArgs,
            TypeArgumentListSyntax resolveAdapterArgs,
            TypeArgumentListSyntax registrationAdapterArgs
        ) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("_"),
                                    GenericName(Identifier("GetService"))
                                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IProgressManager"))))
                                )
                            )
                        ),
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityName)),
                        GetHandlerAdapterArgument(
                            handlerAdapterArgs,
                            HandlerArgument,
                            capabilityName,
                            true
                        ),
                        GetHandlerAdapterArgument(
                            resolveAdapterArgs,
                            ResolveHandlerArgument,
                            capabilityName,
                            false
                        )
                    }
                )
            );
    }
}

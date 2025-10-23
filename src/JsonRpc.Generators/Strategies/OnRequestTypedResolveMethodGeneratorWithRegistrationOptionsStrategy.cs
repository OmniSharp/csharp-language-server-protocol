using System.Collections.Immutable;
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
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, ExtensionMethodContext extensionMethodContext, GeneratorData item)
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
                        .WithHandlerIdentityConstraint(true)
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

            if (item is RequestItem { Response: { Symbol: ITypeParameterSymbol } } ri)
            {
                method = method.AddTypeParameterListParameters(TypeParameter(ri.Response.Symbol.Name));
            }

            var methodFactory = MakeFactory(
                extensionMethodContext.GetRegistryParameterList(),
                registrationOptions.Syntax,
                item.Capability?.Syntax,
                request.PartialHasInitialValue
            );
            var factory = methodFactory(method);

            yield return factory(
                CreateAsyncFunc(responseType, false, requestType),
                CreateAsyncFunc(resolveType, false, resolveType),
                null
            );
            yield return factory(
                CreateAsyncFunc(responseType, true, requestType),
                CreateAsyncFunc(resolveType, true, resolveType),
                null
            );

            {
                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreateAsyncFunc(responseType, true, requestType, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax),
                        null
                    );
                }
            }

            if (request.PartialItems is { })
            {
                var enumerableType = GenericName("IEnumerable")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)));
                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialHandlerExpression(
                                request,
                                requestType,
                                resolveType,
                                responseType,
                                registrationOptions.Syntax,
                                item.Capability?.Syntax
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(requestType, enumerableType, false),
                    CreateAsyncFunc(resolveType, false, resolveType),
                    CreateAsyncFunc(responseType, false, requestType)
                );
                yield return factory(
                    CreatePartialAction(requestType, enumerableType, true),
                    CreateAsyncFunc(resolveType, true, resolveType),
                    CreateAsyncFunc(responseType, true, requestType)
                );

                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreatePartialAction(requestType, enumerableType, true, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax),
                        CreateAsyncFunc(responseType, true, requestType)
                    );
                }
            }

            if (request.PartialItem is { })
            {
                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialHandlerExpression(
                                request,
                                requestType,
                                resolveType,
                                responseType,
                                registrationOptions.Syntax,
                                item.Capability?.Syntax
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(requestType, resolveType, false),
                    CreateAsyncFunc(resolveType, false, resolveType),
                    CreateAsyncFunc(responseType, false, requestType)
                );
                yield return factory(
                    CreatePartialAction(requestType, resolveType, true),
                    CreateAsyncFunc(resolveType, true, resolveType),
                    CreateAsyncFunc(responseType, true, requestType)
                );

                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreatePartialAction(requestType, resolveType, true, capability.Syntax),
                        CreateAsyncFunc(resolveType, true, resolveType, capability.Syntax),
                        CreateAsyncFunc(responseType, true, requestType)
                    );
                }
            }
        }

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, TypeSyntax?, TypeSyntax?, MethodDeclarationSyntax>> MakeFactory(
            ParameterListSyntax preParameterList, TypeSyntax registrationOptions, TypeSyntax? capabilityName, bool partialHasInitialValue
        )
        {
            return method => (syntax, resolveSyntax, initialValueSyntax) => GenerateMethod(method, syntax, resolveSyntax, initialValueSyntax);

            MethodDeclarationSyntax MethodFactory(
                MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax, TypeSyntax? initialHandlerSyntax, ParameterSyntax registrationParameter
            )
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
                                    ? new[]
                                    {
                                        Parameter(Identifier("resolveHandler"))
                                           .WithType(resolveSyntax)
                                    }
                                    : new ParameterSyntax[] { }
                            )
                           .AddParameters(registrationParameter)
                    );
            }

            MethodDeclarationSyntax GenerateMethod(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax, TypeSyntax? initialHandlerSyntax)
            {
                if (capabilityName is { })
                {
                    return MethodFactory(
                        method, syntax, resolveSyntax, initialHandlerSyntax,
                        Parameter(Identifier("registrationOptions"))
                           .WithType(
                                GenericName(Identifier("RegistrationOptionsDelegate")).WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList(new[] { registrationOptions, capabilityName }))
                                )
                            )
                    );
                }

                return MethodFactory(
                    method, syntax, resolveSyntax, initialHandlerSyntax,
                    Parameter(Identifier("registrationOptions"))
                       .WithType(
                            GenericName(Identifier("RegistrationOptionsDelegate"))
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

            return Block(
                ReturnStatement(
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
                )
            );
        }

        private static BlockSyntax GetPartialHandlerExpression(
            RequestItem request,
            TypeSyntax requestSyntax,
            TypeSyntax resolveSyntax,
            TypeSyntax responseSyntax,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var typeArgs = ImmutableArray.Create(registrationOptions);
            var handlerAdapterArgs = ImmutableArray.Create(requestSyntax, resolveSyntax);
            var resolveAdapterArgs = ImmutableArray.Create(resolveSyntax, resolveSyntax);
            var initialValueAdapterArgs = ImmutableArray.Create(requestSyntax, responseSyntax);
            if (capabilityName is { })
            {
                typeArgs = typeArgs.Add(capabilityName);
            }

            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(
                            SimpleLambdaExpression(
                                Parameter(Identifier("_")),
                                CreateHandlerArgument($"Delegating{request.JsonRpcAttributes.HandlerName}PartialHandler", IdentifierName("T"))
                                   .WithArgumentList(
                                        PartialHandlerArgumentList(
                                            IdentifierName("registrationOptions"),
                                            registrationOptions,
                                            capabilityName,
                                            TypeArgumentList(SeparatedList(handlerAdapterArgs)),
                                            TypeArgumentList(SeparatedList(resolveAdapterArgs)),
                                            TypeArgumentList(SeparatedList(typeArgs.ToArray()))
                                        ).AddArguments(
                                            request.PartialHasInitialValue
                                                ? new[]
                                                {
                                                    GetHandlerAdapterArgument(
                                                        TypeArgumentList(SeparatedList(initialValueAdapterArgs)),
                                                        InitialHandlerArgument,
                                                        capabilityName,
                                                        false
                                                    )
                                                }
                                                : Array.Empty<ArgumentSyntax>()
                                        )
                                    )
                            )
                        )
                    )
                )
            );
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
                    new[]
                    {
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
                    new[]
                    {
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

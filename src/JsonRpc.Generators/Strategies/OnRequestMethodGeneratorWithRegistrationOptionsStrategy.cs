using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;
using static OmniSharp.Extensions.JsonRpc.Generators.DelegateHelpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class OnRequestMethodGeneratorWithRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        private readonly bool _doResolve;

        public OnRequestMethodGeneratorWithRegistrationOptionsStrategy(bool doResolve)
        {
            _doResolve = doResolve;
        }

        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not { RegistrationOptions: { } registrationOptions }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var resolve = GeneratorData.CreateForResolver(item);
            if (!_doResolve) resolve = null;
            if (_doResolve && resolve is null) yield break;

            var allowDerivedRequests = item.JsonRpcAttributes.AllowDerivedRequests && !_doResolve;

            var method = MethodDeclaration(extensionMethodContext.Item, item.JsonRpcAttributes.HandlerMethodName)
                        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                        .WithBody(
                             GetRequestHandlerExpression(
                                 request,
                                 registrationOptions.Syntax,
                                 item.Capability?.Syntax,
                                 resolve
                             )
                         );
            if (request.IsUnit)
            {
                method = method
                   .WithBody(
                        GetVoidRequestHandlerExpression(
                            request,
                            registrationOptions.Syntax,
                            item.Capability?.Syntax,
                            resolve
                        )
                    );
            }

            var methodFactory = MakeFactory(
                extensionMethodContext.GetRegistryParameterList(),
                registrationOptions.Syntax,
                item.Capability?.Syntax
            );
            var factory = methodFactory(method);

            yield return factory(
                CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax),
                resolve.ReturnIfNotNull(static r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax))
            );
            yield return factory(
                CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax),
                resolve.ReturnIfNotNull(static r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax))
            );

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, false, IdentifierName("T")), null);
                yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, true, IdentifierName("T")), null);
            }

            {
                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax, capability.Syntax),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax))
                    );

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(CreateAsyncFunc(request.Response.Syntax, true, IdentifierName("T"), capability.Syntax), null);
                    }
                }
            }

            if (request.PartialItems is { } partialItems)
            {
                var partialItemsSyntax = GenericName("IEnumerable").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { partialItems.Syntax })));
                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialResultsHandlerExpression(
                                request,
                                partialItems.Syntax,
                                registrationOptions.Syntax,
                                item.Capability?.Syntax,
                                resolve
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItemsSyntax, false),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax))
                );
                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax))
                );
                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, false), null);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, true), null);
                }

                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true, capability.Syntax),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax))
                    );
                }
            }

            if (request.PartialItem is { } partialItem)
            {
                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialResultHandlerExpression(
                                request,
                                partialItem.Syntax,
                                registrationOptions.Syntax,
                                item.Capability?.Syntax,
                                resolve
                            )
                        )
                );

                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItem.Syntax, false),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, false, r.Request.Syntax))
                );
                yield return factory(
                    CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true),
                    resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax))
                );

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, false), null);
                    yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true), null);
                }

                if (request.Capability is { } capability)
                {
                    yield return factory(
                        CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true, capability.Syntax),
                        resolve.ReturnIfNotNull(r => CreateAsyncFunc(r.Response.Syntax, true, r.Request.Syntax, capability.Syntax))
                    );

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        yield return genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true, capability.Syntax), null);
                    }
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
                       );
            }

            MethodDeclarationSyntax GenerateMethod(MethodDeclarationSyntax method, TypeSyntax syntax, TypeSyntax? resolveSyntax)
            {
                if (capabilityName is { })
                {
                    return MethodFactory(
                        method, syntax, resolveSyntax,
                        Parameter(Identifier("registrationOptions"))
                           .WithType(
                                GenericName(Identifier("RegistrationOptionsDelegate")).WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList(new[] { registrationOptions, capabilityName }))
                                )
                            )
                    );
                }

                return MethodFactory(
                    method, syntax, resolveSyntax,
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
            RequestItem item,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName,
            RequestItem? resolve
        )
        {
            var args = ImmutableArray.Create(item.Request.Syntax, item.Response.Syntax, registrationOptions);
            var adapterArgs = ImmutableArray.Create(item.Request.Syntax, item.Response.Syntax);
            var statements = ImmutableArray.Create<StatementSyntax>();
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
            }

            var returnStatement = ReturnStatement(
                AddHandler(
                    Argument(GetJsonRpcMethodName(item.TypeDeclaration)),
                    Argument(
                        CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), capabilityName is { } ? "Request" : "RequestRegistration", args.ToArray())
                           .WithArgumentList(
                                GetRegistrationHandlerArgumentList(
                                    IdentifierName("registrationOptions"),
                                    registrationOptions,
                                    GetHandlerAdapterArgument(
                                        TypeArgumentList(SeparatedList(adapterArgs.ToArray())),
                                        HandlerArgument,
                                        capabilityName,
                                        false
                                    ),
                                    capabilityName,
                                    resolve is not null
                                )
                            )
                    )
                )
            );

            InsertResolveHandler(ref statements, ref returnStatement, resolve, registrationOptions, capabilityName);

            return Block(statements.Add(returnStatement));
        }

        private static BlockSyntax GetVoidRequestHandlerExpression(
            RequestItem item,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName,
            RequestItem? resolve
        )
        {
            var args = ImmutableArray.Create(item.Request.Syntax, registrationOptions);
            var adapterArgs = ImmutableArray.Create(item.Request.Syntax);
            var statements = ImmutableArray.Create<StatementSyntax>();
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
            }

            var returnStatement = ReturnStatement(
                AddHandler(
                    Argument(GetJsonRpcMethodName(item.TypeDeclaration)),
                    Argument(
                        CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), capabilityName is { } ? "Request" : "RequestRegistration", args.ToArray())
                           .WithArgumentList(
                                GetRegistrationHandlerArgumentList(
                                    IdentifierName("registrationOptions"),
                                    registrationOptions,
                                    GetHandlerAdapterArgument(
                                        TypeArgumentList(SeparatedList(adapterArgs.ToArray())),
                                        HandlerArgument,
                                        capabilityName,
                                        false
                                    ),
                                    capabilityName,
                                    resolve is not null
                                )
                            )
                    )
                )
            );

            InsertResolveHandler(ref statements, ref returnStatement, resolve, registrationOptions, capabilityName);

            return Block(statements.Add(returnStatement));
        }

        private static BlockSyntax GetPartialResultsHandlerExpression(
            RequestItem item,
            TypeSyntax itemName,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName,
            RequestItem? resolve
        )
        {
            var args = ImmutableArray.Create(item.Request.Syntax, item.Response.Syntax, itemName, registrationOptions);
            var adapterArgs = ImmutableArray.Create(item.Request.Syntax, itemName);
            var statements = ImmutableArray.Create<StatementSyntax>();
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
            }

            var returnStatement = ReturnStatement(
                AddHandler(
                    Argument(GetJsonRpcMethodName(item.TypeDeclaration)),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")
                            ),
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResults",
                                    args.ToArray()
                                )
                               .WithArgumentList(
                                    GetPartialItemsArgumentList(
                                        IdentifierName("registrationOptions"),
                                        registrationOptions,
                                        item.Response.Syntax,
                                        GetHandlerAdapterArgument(
                                            TypeArgumentList(SeparatedList(adapterArgs.ToArray())),
                                            HandlerArgument,
                                            capabilityName,
                                            true
                                        ),
                                        capabilityName
                                    )
                                )
                        )
                    )
                )
            );

            InsertResolveHandler(ref statements, ref returnStatement, resolve, registrationOptions, capabilityName);

            return Block(statements.Add(returnStatement));
        }

        private static BlockSyntax GetPartialResultHandlerExpression(
            RequestItem item,
            TypeSyntax itemType,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName,
            RequestItem? resolve
        )
        {
            var args = ImmutableArray.Create(item.Request.Syntax, item.Response.Syntax, itemType, registrationOptions);
            var adapterArgs = ImmutableArray.Create(item.Request.Syntax, itemType);
            var statements = ImmutableArray.Create<StatementSyntax>();
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
            }

            var returnStatement = ReturnStatement(
                AddHandler(
                    Argument(GetJsonRpcMethodName(item.TypeDeclaration)),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")
                            ),
                            CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), "PartialResult", args.ToArray())
                               .WithArgumentList(
                                    GetPartialResultArgumentList(
                                        IdentifierName("registrationOptions"),
                                        registrationOptions,
                                        item.Response.Syntax,
                                        GetHandlerAdapterArgument(
                                            TypeArgumentList(SeparatedList(adapterArgs.ToArray())),
                                            HandlerArgument,
                                            capabilityName,
                                            true
                                        ),
                                        capabilityName
                                    )
                                )
                        )
                    )
                )
            );

            InsertResolveHandler(ref statements, ref returnStatement, resolve, registrationOptions, capabilityName);

            return Block(statements.Add(returnStatement));
        }

        private static void InsertResolveHandler(
            ref ImmutableArray<StatementSyntax> statements,
            ref ReturnStatementSyntax returnStatement,
            RequestItem? resolve,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            if (resolve is null) return;
            var args = ImmutableArray.Create(resolve.Request.Syntax, resolve.Response.Syntax, registrationOptions);
            var adapterArgs = ImmutableArray.Create(resolve.Request.Syntax, resolve.Response.Syntax);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
            }

            statements = statements.Add(NewGuid);
            returnStatement = returnStatement.WithExpression(
                AddHandler(
                    returnStatement.Expression!,
                    Argument(GetJsonRpcMethodName(resolve.TypeDeclaration)),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"), capabilityName is { } ? "Request" : "RequestRegistration",
                                args.ToArray()
                            )
                           .WithArgumentList(
                                GetRegistrationHandlerArgumentList(
                                    IdentifierName("registrationOptions"),
                                    registrationOptions,
                                    GetHandlerAdapterArgument(
                                        TypeArgumentList(SeparatedList(adapterArgs.ToArray())),
                                        ResolveHandlerArgument,
                                        capabilityName,
                                        false
                                    ),
                                    capabilityName,
                                    true
                                )
                            )
                    )
                )
            );
        }

        private static ArgumentListSyntax GetPartialItemsArgumentList(
            TypeSyntax registrationOptionsName, TypeSyntax registrationType, TypeSyntax responseName, ArgumentSyntax handlerArgument, TypeSyntax? capabilityType
        ) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        handlerArgument,
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityType)),
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
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                responseName is NullableTypeSyntax nts ? nts.ElementType : responseName,
                                IdentifierName("From")
                            )
                        )
                    }
                )
            );

        private static ArgumentListSyntax GetPartialResultArgumentList(
            TypeSyntax registrationOptionsName, TypeSyntax registrationType, TypeSyntax responseName, ArgumentSyntax handlerArgument, TypeSyntax? capabilityType
        ) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        handlerArgument,
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityType)),
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
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                responseName.EnsureNotNullable(),
                                IdentifierName("From")
                            )
                        )
                    }
                )
            );
    }
}

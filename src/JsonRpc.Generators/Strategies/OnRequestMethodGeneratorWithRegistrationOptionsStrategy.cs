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
    internal class OnRequestMethodGeneratorWithRegistrationOptionsStrategy : IExtensionMethodContextGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(ExtensionMethodContext extensionMethodContext, GeneratorData item)
        {
            if (item is not { RegistrationOptions: { } registrationOptions }) yield break;
            if (item is not RequestItem request) yield break;
            if (extensionMethodContext is not { IsRegistry: true }) yield break;

            var methods = new List<MethodDeclarationSyntax>();

            var allowDerivedRequests = item.JsonRpcAttributes.AllowDerivedRequests;

            var method = MethodDeclaration(extensionMethodContext.Item, item.JsonRpcAttributes.HandlerMethodName)
                        .WithModifiers(
                             TokenList(
                                 Token(SyntaxKind.PublicKeyword),
                                 Token(SyntaxKind.StaticKeyword)
                             )
                         )
                        .WithBody(
                             GetRequestHandlerExpression(
                                 GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, registrationOptions.Syntax,
                                 item.Capability?.Syntax
                             )
                         );
            if (request.IsUnit)
            {
                method = method
                   .WithBody(
                        GetVoidRequestHandlerExpression(
                            GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, registrationOptions.Syntax, item.Capability?.Syntax
                        )
                    );
            }

            var methodFactory = MakeFactory(extensionMethodContext.GetRegistryParameterList(), registrationOptions.Syntax, item.Capability?.Syntax);
            var factory = methodFactory(method);

            methods.AddRange(factory(CreateAsyncFunc(request.Response.Syntax, false, request.Request.Syntax)));
            methods.AddRange(factory(CreateAsyncFunc(request.Response.Syntax, true, request.Request.Syntax)));

            if (allowDerivedRequests)
            {
                var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                methods.AddRange(genericFactory(CreateAsyncFunc(request.Response.Syntax, false, IdentifierName("T"))));
                methods.AddRange( genericFactory(CreateAsyncFunc(request.Response.Syntax, true, IdentifierName("T"))));
            }

            {
                if (request.Capability is { } capability)
                {
                    methods.AddRange(factory(CreateAsyncFunc(request.Response.Syntax, request.Request.Syntax, capability.Syntax)));

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        methods.AddRange(genericFactory(CreateAsyncFunc(request.Response.Syntax, IdentifierName("T"), capability.Syntax)));
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
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, request.Response.Syntax, partialItems.Syntax,
                                registrationOptions.Syntax, item.Capability?.Syntax
                            )
                        )
                );

                methods.AddRange(factory(CreatePartialAction(request.Request.Syntax, partialItemsSyntax, false)));
                methods.AddRange(factory(CreatePartialAction(request.Request.Syntax, partialItemsSyntax, true)));
                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    methods.AddRange(genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, false)));
                    methods.AddRange(genericFactory(CreatePartialAction(IdentifierName("T"), partialItemsSyntax, true)));
                }

                if (request.Capability is { } capability)
                {
                    methods.AddRange( factory(CreatePartialAction(request.Request.Syntax, partialItemsSyntax, capability.Syntax)));
                }
            }

            if (request.PartialItem is { } partialItem)
            {
                factory = methodFactory(
                    method
                       .WithIdentifier(Identifier(item.JsonRpcAttributes.PartialHandlerMethodName))
                       .WithBody(
                            GetPartialResultHandlerExpression(
                                GetJsonRpcMethodName(extensionMethodContext.TypeDeclaration), request.Request.Syntax, partialItem.Syntax, request.Response.Syntax,
                                registrationOptions.Syntax, item.Capability?.Syntax
                            )
                        )
                );

                methods.AddRange( factory(CreatePartialAction(request.Request.Syntax, partialItem.Syntax, false)));
                methods.AddRange( factory(CreatePartialAction(request.Request.Syntax, partialItem.Syntax, true)));

                if (allowDerivedRequests)
                {
                    var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                    methods.AddRange( genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, false)));
                    methods.AddRange( genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true)));
                }

                if (request.Capability is { } capability)
                {
                    methods.AddRange( factory(CreatePartialAction(request.Request.Syntax, partialItem.Syntax, capability.Syntax)));

                    if (allowDerivedRequests)
                    {
                        var genericFactory = MakeGenericFactory(factory, request.Request.Syntax);
                        methods.AddRange( genericFactory(CreatePartialAction(IdentifierName("T"), partialItem.Syntax, true, capability.Syntax)));
                    }
                }
            }

            foreach (var m in methods) yield return m;
        }

        private static Func<MethodDeclarationSyntax, Func<TypeSyntax, IEnumerable<MethodDeclarationSyntax>>> MakeFactory(
            ParameterListSyntax preParameterList, TypeSyntax registrationOptions, TypeSyntax? capabilityName
        )
        {
            return method => syntax => GenerateMethods(method, syntax);

            MethodDeclarationSyntax MethodFactory(MethodDeclarationSyntax method, TypeSyntax syntax, ParameterSyntax registrationParameter)
            {
                return method
                      .WithParameterList(
                           preParameterList.AddParameters(Parameter(Identifier("handler")).WithType(syntax))
                                           .AddParameters(registrationParameter)
                       )
                      .NormalizeWhitespace();
            }

            IEnumerable<MethodDeclarationSyntax> GenerateMethods(MethodDeclarationSyntax method, TypeSyntax syntax)
            {
                if (capabilityName is { })
                {
                    yield return MethodFactory(
                        method, syntax,
                        Parameter(Identifier("registrationOptions"))
                           .WithType(
                                GenericName(Identifier("Func")).WithTypeArgumentList(
                                    TypeArgumentList(SeparatedList(new[] { capabilityName, registrationOptions }))
                                )
                            )
                    );
                }

                yield return MethodFactory(
                    method, syntax,
                    Parameter(Identifier("registrationOptions"))
                       .WithType(
                            GenericName(Identifier("Func"))
                               .WithTypeArgumentList(
                                    TypeArgumentList(SingletonSeparatedList(registrationOptions))
                                )
                        )
                );
                yield return MethodFactory(
                    method, syntax,
                    Parameter(Identifier("registrationOptions")).WithType(registrationOptions)
                );
            }
        }

        private static BlockSyntax GetRequestHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var args = ImmutableArray.Create(requestType, responseType, registrationOptions);
            var typeArgs = ImmutableArray.Create(registrationOptions);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
                typeArgs = typeArgs.Add(capabilityName);
            }
            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), capabilityName is { } ? "Request" : "RequestRegistration", args.ToArray())
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"), TypeArgumentList(SeparatedList(typeArgs.ToArray()))))
                        )
                    )
                )
            );
        }

        private static BlockSyntax GetVoidRequestHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var args = ImmutableArray.Create(requestType, registrationOptions);
            var typeArgs = ImmutableArray.Create(registrationOptions);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
                typeArgs = typeArgs.Add(capabilityName);
            }
            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), capabilityName is { } ? "Request" : "RequestRegistration", args.ToArray())
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"), TypeArgumentList(SeparatedList(typeArgs.ToArray()))))
                        )
                    )
                )
            );
        }

        private static BlockSyntax GetPartialResultsHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType,
            TypeSyntax itemName, TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var args = ImmutableArray.Create(requestType, responseType, itemName, registrationOptions);
            var typeArgs = ImmutableArray.Create(registrationOptions);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
                typeArgs = typeArgs.Add(capabilityName);
            }
            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
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
                                   .WithArgumentList(GetPartialItemsArgumentList(IdentifierName("registrationOptions"), responseType, TypeArgumentList(SeparatedList(typeArgs.ToArray()))))
                            )
                        )
                    )
                )
            );
        }

        private static BlockSyntax GetPartialResultHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax itemType, TypeSyntax responseType,
            TypeSyntax registrationOptions,
            TypeSyntax? capabilityName
        )
        {
            var args = ImmutableArray.Create(requestType, responseType, itemType, registrationOptions);
            var typeArgs = ImmutableArray.Create(registrationOptions);
            if (capabilityName is { })
            {
                args = args.Add(capabilityName);
                typeArgs = typeArgs.Add(capabilityName);
            }
            return Block(
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            SimpleLambdaExpression(
                                Parameter(
                                    Identifier("_")
                                ),
                                CreateHandlerArgument(IdentifierName("LanguageProtocolDelegatingHandlers"), "PartialResult", args.ToArray())
                                   .WithArgumentList(GetPartialResultArgumentList(IdentifierName("registrationOptions"), responseType, TypeArgumentList(SeparatedList(typeArgs.ToArray()))))
                            )
                        )
                    )
                )
            );
        }

        private static ArgumentListSyntax GetPartialItemsArgumentList(TypeSyntax registrationOptionsName, TypeSyntax responseName, TypeArgumentListSyntax typeArgumentListSyntax) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, typeArgumentListSyntax)),
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
                            SimpleLambdaExpression(
                                Parameter(Identifier("values")),
                                ObjectCreationExpression(responseName is NullableTypeSyntax nts ? nts.ElementType : responseName)
                                   .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))
                            )
                        )
                    }
                )
            );

        private static ArgumentListSyntax GetPartialResultArgumentList(TypeSyntax registrationOptionsName, TypeSyntax responseName, TypeArgumentListSyntax typeArgumentListSyntax) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, typeArgumentListSyntax)),
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
                            SimpleLambdaExpression(
                                Parameter(Identifier("values")),
                                ObjectCreationExpression(responseName is NullableTypeSyntax nts ? nts.ElementType : responseName)
                                   .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))
                            )
                        )
                    }
                )
            );
    }
}

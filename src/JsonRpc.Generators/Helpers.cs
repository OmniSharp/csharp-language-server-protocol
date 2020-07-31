#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    static class Helpers
    {
        public static bool IsNotification(INamedTypeSymbol symbol)
        {
            return symbol.AllInterfaces.Any(z => z.Name == "IJsonRpcNotificationHandler");
        }

        public static bool IsRequest(INamedTypeSymbol symbol)
        {
            return symbol.AllInterfaces.Any(z => z.Name == "IJsonRpcRequestHandler");
        }

        public static ExpressionSyntax GetMethodName(InterfaceDeclarationSyntax interfaceSyntax)
        {
            var methodAttribute = interfaceSyntax.AttributeLists
                .SelectMany(z => z.Attributes)
                .First(z => z.Name.ToString() == "MethodAttribute" || z.Name.ToString() == "Method");

            return methodAttribute.ArgumentList.Arguments[0].Expression;
        }

        public static INamedTypeSymbol GetRequestType(INamedTypeSymbol symbol)
        {
            var handlerInterface = symbol.AllInterfaces.First(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            return handlerInterface.TypeArguments[0] as INamedTypeSymbol;
        }

        public static INamedTypeSymbol GetResponseType(INamedTypeSymbol symbol)
        {
            var handlerInterface = symbol.AllInterfaces.First(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            return handlerInterface.TypeArguments[1] as INamedTypeSymbol;
        }

        public static INamedTypeSymbol? GetCapability(INamedTypeSymbol symbol)
        {
            var handlerInterface = symbol.AllInterfaces
                .FirstOrDefault(z => z.Name == "ICapability" && z.TypeArguments.Length == 1);
            return handlerInterface?.TypeArguments[0] as INamedTypeSymbol;
        }

        public static INamedTypeSymbol? GetRegistrationOptions(INamedTypeSymbol symbol)
        {
            var handlerInterface = symbol.AllInterfaces
                .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 1);
            return handlerInterface?.TypeArguments[0] as INamedTypeSymbol;
        }

        public static INamedTypeSymbol? GetPartialItems(INamedTypeSymbol symbol)
        {
            var handlerInterface = symbol.AllInterfaces
                .FirstOrDefault(z => z.Name == "IPartialItems" && z.TypeArguments.Length == 1);
            return handlerInterface?.TypeArguments[0] as INamedTypeSymbol;
        }

        public static INamedTypeSymbol? GetPartialItem(INamedTypeSymbol symbol)
        {
            var handlerInterface = symbol.AllInterfaces
                .FirstOrDefault(z => z.Name == "IPartialItem" && z.TypeArguments.Length == 1);
            return handlerInterface?.TypeArguments[0] as INamedTypeSymbol;
        }

        public static GenericNameSyntax CreateAction(bool withCancellationToken, params ITypeSymbol[] types)
        {
            var typeArguments = types.Select(ResolveTypeName).ToList();
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            return GenericName(Identifier("Action"))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(typeArguments)));
        }

        public static NameSyntax ResolveTypeName(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType)
                {
                    // TODO: Fix for generic types
                    return ParseName(namedTypeSymbol.ToString());
                }

                // Assume that we're adding the correct namespaces.
                return IdentifierName(namedTypeSymbol.Name);
            }

            return IdentifierName(symbol.Name);
        }

        public static GenericNameSyntax CreateAction(params ITypeSymbol[] types) => CreateAction(true, types);

        public static GenericNameSyntax CreateAsyncAction(params ITypeSymbol[] types) => CreateAsyncFunc(null, true, types);

        public static GenericNameSyntax CreateAsyncAction(bool withCancellationToken, params ITypeSymbol[] types) => CreateAsyncFunc(null, withCancellationToken, types);

        public static GenericNameSyntax CreateAsyncFunc(ITypeSymbol? responseType, params ITypeSymbol[] types) => CreateAsyncFunc(responseType, true, types);

        public static GenericNameSyntax CreateAsyncFunc(ITypeSymbol? responseType, bool withCancellationToken, params ITypeSymbol[] types)
        {
            var typeArguments = types.Select(ResolveTypeName).ToList();
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            if (responseType == null || responseType.Name == "Unit")
            {
                typeArguments.Add(IdentifierName("Task"));
            }
            else
            {
                typeArguments.Add(GenericName(Identifier("Task"), TypeArgumentList(SeparatedList(new TypeSyntax[] {
                    ResolveTypeName(responseType)
                }))));
            }

            return GenericName(Identifier("Func"))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(typeArguments)));
        }

        public static GenericNameSyntax CreatePartialAction(ITypeSymbol requestType, NameSyntax partialType, bool withCancellationToken, params ITypeSymbol[] types)
        {
            var typeArguments = new List<NameSyntax>() {
                ResolveTypeName(requestType),
                GenericName("IObserver").WithTypeArgumentList(TypeArgumentList(SeparatedList(new TypeSyntax[] {partialType}))),
            };
            typeArguments.AddRange(types.Select(ResolveTypeName));
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            return GenericName(Identifier("Action"))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList<TypeSyntax>(typeArguments)));
        }

        public static GenericNameSyntax CreatePartialAction(ITypeSymbol requestType, NameSyntax partialType, params ITypeSymbol[] types) =>
            CreatePartialAction(requestType, partialType, true, types);

        private static ExpressionStatementSyntax EnsureRegistrationOptionsIsSet(NameSyntax registrationOptionsName, TypeSyntax registrationOptionsType)
        {
            return ExpressionStatement(AssignmentExpression(
                SyntaxKind.CoalesceAssignmentExpression,
                registrationOptionsName,
                ObjectCreationExpression(registrationOptionsType)
                    .WithArgumentList(ArgumentList())));
        }

        private static InvocationExpressionSyntax AddHandler(params ArgumentSyntax[] arguments) => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("registry"),
                    IdentifierName("AddHandler")))
            .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        private static ArgumentListSyntax GetHandlerArgumentList() =>
            ArgumentList(SeparatedList(new[] {
                Argument(IdentifierName("handler"))
            }));

        private static ArgumentListSyntax GetRegistrationHandlerArgumentList(NameSyntax registrationOptionsName) =>
            ArgumentList(SeparatedList(new[] {
                Argument(IdentifierName("handler")),
                Argument(registrationOptionsName)
            }));

        private static ArgumentListSyntax GetPartialResultArgumentList(NameSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("_"),
                            GenericName(Identifier("GetService"))
                                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IProgressManager"))))
                        ))),
                        Argument(SimpleLambdaExpression(Parameter(Identifier("values")),
                            ObjectCreationExpression(responseName)
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))))
                    }));

        private static ArgumentListSyntax GetPartialResultRegistrationArgumentList(NameSyntax registrationOptionsName, NameSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(registrationOptionsName),
                        Argument(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("_"),
                            GenericName(Identifier("GetService"))
                                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IProgressManager"))))
                        ))),
                        Argument(SimpleLambdaExpression(Parameter(Identifier("values")),
                            ObjectCreationExpression(responseName)
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))))
                    }));

        private static ArgumentListSyntax GetPartialItemsArgumentList(NameSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("_"),
                            GenericName(Identifier("GetService"))
                                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IProgressManager"))))
                        ))),
                        Argument(SimpleLambdaExpression(Parameter(Identifier("values")),
                            ObjectCreationExpression(responseName)
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))))
                    }));

        private static ArgumentListSyntax GetPartialItemsRegistrationArgumentList(NameSyntax registrationOptionsName, NameSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(registrationOptionsName),
                        Argument(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("_"),
                            GenericName(Identifier("GetService"))
                                .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("IProgressManager"))))
                        ))),
                        Argument(SimpleLambdaExpression(Parameter(Identifier("values")),
                            ObjectCreationExpression(responseName)
                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))))
                    }));

        private static ObjectCreationExpressionSyntax CreateHandlerArgument(NameSyntax className, string innerClassName, params TypeSyntax[] genericArguments)
        {
            return ObjectCreationExpression(
                QualifiedName(
                    className,
                    GenericName(innerClassName).WithTypeArgumentList(TypeArgumentList(SeparatedList(genericArguments)))
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetNotificationCapabilityHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var capabilityName = ResolveTypeName(capability);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "NotificationCapability",
                                requestName,
                                capabilityName
                            )
                            .WithArgumentList(GetHandlerArgumentList()))
                )
            );
        }

        public static BlockSyntax GetNotificationRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol registrationOptions)
        {
            var requestName = ResolveTypeName(requestType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Notification",
                                    requestName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"))))
                    )
                )
            );
        }

        public static BlockSyntax GetNotificationRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol registrationOptions,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            var capabilityName = ResolveTypeName(capability);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Notification",
                                    requestName,
                                    capabilityName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"))))
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetRequestCapabilityHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var capabilityName = ResolveTypeName(capability);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "RequestCapability",
                                requestName,
                                responseName,
                                capabilityName
                            )
                            .WithArgumentList(GetHandlerArgumentList()))
                )
            );
        }

        public static BlockSyntax GetRequestRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            ITypeSymbol registrationOptions)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "RequestRegistration",
                                    requestName,
                                    responseName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"))))
                    )
                )
            );
        }

        public static BlockSyntax GetVoidRequestRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol registrationOptions)
        {
            var requestName = ResolveTypeName(requestType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "RequestRegistration",
                                    requestName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"))))
                    )
                )
            );
        }

        public static BlockSyntax GetRequestRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            ITypeSymbol registrationOptions,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            var capabilityName = ResolveTypeName(capability);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Request",
                                    requestName,
                                    responseName,
                                    capabilityName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"))))
                    )
                )
            );
        }

        public static BlockSyntax GetVoidRequestRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol registrationOptions,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            var capabilityName = ResolveTypeName(capability);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Request",
                                    requestName,
                                    capabilityName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptions"))))
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetRequestHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "Request",
                                requestName,
                                responseName
                            )
                            .WithArgumentList(GetHandlerArgumentList()))
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetRequestHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType)
        {
            var requestName = ResolveTypeName(requestType);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "Request",
                                requestName
                            )
                            .WithArgumentList(GetHandlerArgumentList()))
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultCapabilityHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var capabilityName = ResolveTypeName(capability);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "PartialResultCapability",
                                requestName,
                                responseName,
                                capabilityName
                            )
                            .WithArgumentList(GetPartialResultArgumentList(responseName)))
                )
            );
        }

        public static BlockSyntax GetPartialResultRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            ITypeSymbol registrationOptions)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResult",
                                    requestName,
                                    responseName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetPartialResultRegistrationArgumentList(IdentifierName("registrationOptions"), responseName)))
                    )
                )
            );
        }

        public static BlockSyntax GetPartialResultRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            ITypeSymbol registrationOptions,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            var capabilityName = ResolveTypeName(capability);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResult",
                                    requestName,
                                    responseName,
                                    capabilityName,
                                    registrationOptionsName
                                )
                                .WithArgumentList(GetPartialResultRegistrationArgumentList(IdentifierName("registrationOptions"), responseName)))
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "PartialResult",
                                requestName,
                                responseName
                            )
                            .WithArgumentList(GetPartialResultArgumentList(responseName)))
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultsCapabilityHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            NameSyntax itemName, ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var capabilityName = ResolveTypeName(capability);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")),
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResultsCapability",
                                    requestName,
                                    responseName,
                                    itemName,
                                    capabilityName
                                )
                                .WithArgumentList(GetPartialItemsArgumentList(responseName)))
                    )
                )
            );
        }

        public static BlockSyntax GetPartialResultsRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            NameSyntax itemName, ITypeSymbol registrationOptions)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            SimpleLambdaExpression(
                                Parameter(
                                    Identifier("_")),
                                CreateHandlerArgument(
                                        IdentifierName("LanguageProtocolDelegatingHandlers"),
                                        "PartialResults",
                                        requestName,
                                        responseName,
                                        itemName,
                                        registrationOptionsName
                                    )
                                    .WithArgumentList(GetPartialItemsRegistrationArgumentList(IdentifierName("registrationOptions"), responseName)))
                        )
                    ))
            );
        }

        public static BlockSyntax GetPartialResultsRegistrationHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, ITypeSymbol responseType,
            NameSyntax itemName, ITypeSymbol registrationOptions,
            ITypeSymbol capability)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            var registrationOptionsName = ResolveTypeName(registrationOptions);
            var capabilityName = ResolveTypeName(capability);
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptions"), registrationOptionsName),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            SimpleLambdaExpression(
                                Parameter(
                                    Identifier("_")),
                                CreateHandlerArgument(
                                        IdentifierName("LanguageProtocolDelegatingHandlers"),
                                        "PartialResults",
                                        requestName,
                                        responseName,
                                        itemName,
                                        capabilityName,
                                        registrationOptionsName
                                    )
                                    .WithArgumentList(GetPartialItemsRegistrationArgumentList(IdentifierName("registrationOptions"), responseName)))
                        )
                    ))
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultsHandlerExpression(ExpressionSyntax nameExpression, ITypeSymbol requestType, NameSyntax itemName,
            ITypeSymbol responseType)
        {
            var requestName = ResolveTypeName(requestType);
            var responseName = ResolveTypeName(responseType);
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")),
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResults",
                                    requestName,
                                    responseName,
                                    itemName
                                )
                                .WithArgumentList(GetPartialItemsArgumentList(responseName)))
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetNotificationHandlerExpression(ExpressionSyntax nameExpression)
        {
            return ArrowExpressionClause(
                InvocationExpression(MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("registry"),
                        IdentifierName("AddHandler")
                    ))
                    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] {
                        Argument(nameExpression),
                        Token(SyntaxKind.CommaToken),
                        Argument(InvocationExpression(MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("NotificationHandler"),
                                IdentifierName("For")
                            ))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("handler"))))))
                    })))
            );
        }

        public static ArrowExpressionClauseSyntax GetRequestHandlerExpression(ExpressionSyntax nameExpression)
        {
            return ArrowExpressionClause(
                InvocationExpression(MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("registry"),
                        IdentifierName("AddHandler")
                    ))
                    .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[] {
                        Argument(nameExpression),
                        Token(SyntaxKind.CommaToken),
                        Argument(InvocationExpression(MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("RequestHandler"),
                                IdentifierName("For")
                            ))
                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("handler"))))))
                    })))
            );
        }

        public static ArrowExpressionClauseSyntax GetNotificationInvokeExpression()
        {
            return ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("mediator"),
                            IdentifierName("SendNotification")))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(new[] {
                                Argument(IdentifierName(@"@params"))
                            }))
                    )
            );
        }

        public static ArrowExpressionClauseSyntax GetRequestInvokeExpression()
        {
            return ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("mediator"),
                            IdentifierName("SendRequest")))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(new[] {
                                Argument(IdentifierName(@"@params")),
                                Argument(IdentifierName("cancellationToken"))
                            }))
                    )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialInvokeExpression(NameSyntax responseType)
        {
            return ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("mediator"),
                                IdentifierName("ProgressManager")),
                            IdentifierName("MonitorUntil")))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                new [] {
                                    Argument(
                                        IdentifierName(@"@params")),
                                    Argument(
                                        SimpleLambdaExpression(
                                            Parameter(Identifier("value")),
                                            ObjectCreationExpression(responseType)
                                                .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value"))))))),
                                    Argument(IdentifierName("cancellationToken"))
                                }))));
        }

        public static string GetExtensionClassName(INamedTypeSymbol symbol)
        {
            return SpecialCasedHandlerFullName(symbol).Split('.').Last() + "Extensions";
            ;
        }

        private static string SpecialCasedHandlerFullName(INamedTypeSymbol symbol)
        {
            return new Regex(@"(\w+)$")
                    .Replace(symbol.ToDisplayString() ?? string.Empty,
                        symbol.Name.Substring(1, symbol.Name.IndexOf("Handler", StringComparison.Ordinal) - 1))
                ;
        }

        public static string SpecialCasedHandlerName(INamedTypeSymbol symbol)
        {
            var name = SpecialCasedHandlerFullName(symbol);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        public static string GetOnMethodName(INamedTypeSymbol symbol, AttributeData attributeData)
        {
            var namedMethod = attributeData.NamedArguments
                .Where(z => z.Key == "MethodName")
                .Select(z => z.Value.Value)
                .FirstOrDefault();
            if (namedMethod is string value) return value;
            return "On" + SpecialCasedHandlerName(symbol);
        }

        public static string GetSendMethodName(INamedTypeSymbol symbol, AttributeData attributeData)
        {
            var namedMethod = attributeData.NamedArguments
                .Where(z => z.Key == "MethodName")
                .Select(z => z.Value.Value)
                .FirstOrDefault();
            if (namedMethod is string value) return value;
            var name = SpecialCasedHandlerName(symbol);
            if (
                name.StartsWith("Run")
                || name.StartsWith("Execute")
                // TODO: Change this next breaking change
                // || name.StartsWith("Set")
                // || name.StartsWith("Attach")
                // || name.StartsWith("Read")
                || name.StartsWith("Did")
                || name.StartsWith("Log")
                || name.StartsWith("Show")
                || name.StartsWith("Register")
                || name.StartsWith("Prepare")
                || name.StartsWith("Publish")
                || name.StartsWith("ApplyWorkspaceEdit")
                || name.StartsWith("Unregister"))
            {
                return name;
            }

            if (name.EndsWith("Resolve", StringComparison.Ordinal))
            {
                return "Resolve" + name.Substring(0, name.IndexOf("Resolve", StringComparison.Ordinal));
            }

            return IsNotification(symbol) ? "Send" + name : "Request" + name;
        }

        private static string HandlerName(INamedTypeSymbol symbol)
        {
            var name = HandlerFullName(symbol);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        private static string HandlerFullName(INamedTypeSymbol symbol)
        {
            return new Regex(@"(\w+)$")
                .Replace(symbol.ToDisplayString() ?? string.Empty,
                    symbol.Name.Substring(1, symbol.Name.IndexOf("Handler", StringComparison.Ordinal) - 1));
        }
    }
}

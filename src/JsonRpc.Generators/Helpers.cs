#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    internal static class Helpers
    {
        public static bool IsNotification(INamedTypeSymbol symbol) => symbol.AllInterfaces.Any(z => z.Name == "IJsonRpcNotificationHandler");

        public static bool IsRequest(INamedTypeSymbol symbol) => symbol.AllInterfaces.Any(z => z.Name == "IJsonRpcRequestHandler");

        public static ExpressionSyntax GetJsonRpcMethodName(TypeDeclarationSyntax interfaceSyntax)
        {
            var methodAttribute = interfaceSyntax.AttributeLists
                                                 .SelectMany(z => z.Attributes)
                                                 .First(z => z.Name.ToString() == "MethodAttribute" || z.Name.ToString() == "Method");

            return methodAttribute.ArgumentList!.Arguments[0].Expression;
        }

        public static SyntaxSymbol GetResponseType(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "IJsonRpcRequestHandler")
                             .Select(z => z!.Arity == 1 ? IdentifierName("MediatR.Unit") : z.TypeArgumentList.Arguments[1])
                             .FirstOrDefault()
                    ?? syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "ICanBeResolvedHandler")
                             .Select(z => z!.TypeArgumentList.Arguments[0])
                             .FirstOrDefault();
            if (type == null) throw new ArgumentException($"Response Type {symbol.ToDisplayString()} is not a name symbol", nameof(symbol));

            var handlerInterface = symbol.AllInterfaces.First(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            if (handlerInterface.TypeArguments[1] is INamedTypeSymbol ns)
                return new SyntaxSymbol(type, ns);
            throw new ArgumentException($"Response Type {symbol.ToDisplayString()} is not a name symbol", nameof(symbol));
        }

        public static SyntaxSymbol GetRequestType(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            TypeSyntax type;
            if (syntax.ConstraintClauses.Any())
            {
                type = syntax.ConstraintClauses.First()
                             .Constraints
                             .OfType<TypeConstraintSyntax>()
                             .FirstOrDefault()?.Type
                    ?? throw new ArgumentException("Generic type does not have a constraint", nameof(syntax));
            }
            else
            {
                var interfaceType = syntax.BaseList?.Types
                                          .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                                          .Where(z => z != null)
                                          .First(
                                               z => z!.Identifier.Text == "IJsonRpcRequestHandler" || z.Identifier.Text == "ICanBeResolvedHandler"
                                                                                                   || z.Identifier.Text == "IJsonRpcNotificationHandler"
                                           )!;

                type = interfaceType.TypeArgumentList.Arguments[0];
            }

            var handlerInterface = symbol.AllInterfaces.First(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            var arg = handlerInterface.TypeArguments[0];
            if (arg is ITypeParameterSymbol typeParameterSymbol)
            {
                return new SyntaxSymbol(
                    type, typeParameterSymbol.ConstraintTypes.OfType<INamedTypeSymbol>().FirstOrDefault()
                       ?? throw new ArgumentException("Generic type does not have a constraint", nameof(symbol))
                );
            }

            if (arg is INamedTypeSymbol namedTypeSymbol)
            {
                return new SyntaxSymbol(type, namedTypeSymbol);
            }

            throw new NotSupportedException($"Request Type {symbol.ToDisplayString()} is not supported!");
        }

        public static SyntaxSymbol? GetCapability(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "ICapability" && z.Arity == 1)
                             .Select(z => z!.TypeArgumentList.Arguments[0])
                             .FirstOrDefault()
                    ?? syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "IRegistration" && z.Arity == 2)
                             .Select(z => z!.TypeArgumentList.Arguments[1])
                             .FirstOrDefault();

            if (type == null) return null;

            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "ICapability" && z.TypeArguments.Length == 1)?.TypeArguments[0]
                                ?? symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 2)?.TypeArguments[1];
            return new SyntaxSymbol(type, ( handlerInterface as INamedTypeSymbol )!);
        }

        public static SyntaxSymbol? GetRegistrationOptions(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "IRegistration" && z.Arity > 0)
                             .Select(z => z!.TypeArgumentList.Arguments[0])
                             .FirstOrDefault();
            if (type == null) return null;
            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 1)?.TypeArguments[0]
                                ?? symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 2)?.TypeArguments[0];
            return new SyntaxSymbol(type, ( handlerInterface as INamedTypeSymbol )!);
        }

        public static SyntaxSymbol? GetPartialItems(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SyntaxSymbol requestType)
        {
            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IPartialItems" && z.TypeArguments.Length == 1)
                                ?? requestType.Symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IPartialItems" && z.TypeArguments.Length == 1);
            var localSymbol = ( handlerInterface?.TypeArguments[0] as INamedTypeSymbol );
            if (localSymbol == null) return null;
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "IPartialItemsRequest" && z.Arity == 2)
                             .Select(z => z!.TypeArgumentList.Arguments[1])
                             .FirstOrDefault();

            return new SyntaxSymbol(type ?? ResolveTypeName(localSymbol), localSymbol);
        }

        public static SyntaxSymbol? GetPartialItem(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SyntaxSymbol requestType)
        {
            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IPartialItem" && z.TypeArguments.Length == 1)
                                ?? requestType.Symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IPartialItem" && z.TypeArguments.Length == 1);
            var localSymbol = ( handlerInterface?.TypeArguments[0] as INamedTypeSymbol );
            if (localSymbol == null) return null;
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z!.Identifier.Text == "IPartialItemRequest" && z.Arity == 2)
                             .Select(z => z!.TypeArgumentList.Arguments[1])
                             .FirstOrDefault();
            return new SyntaxSymbol(type ?? ResolveTypeName(localSymbol), localSymbol);
        }

        public static NameSyntax ResolveTypeName(ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.IsGenericType)
                {
                    // TODO: Fix for generic arguments
                    return ParseName(namedTypeSymbol.ToString());
                }

                // Assume that we're adding the correct namespaces.
                return IdentifierName(namedTypeSymbol.Name);
            }

            return IdentifierName(symbol.Name);
        }

        public static GenericNameSyntax CreatePartialAction(TypeSyntax requestType, TypeSyntax partialType, bool withCancellationToken, params TypeSyntax[] types)
        {
            var typeArguments = new List<TypeSyntax> {
                requestType,
                GenericName("IObserver").WithTypeArgumentList(TypeArgumentList(SeparatedList(new TypeSyntax[] { partialType }))),
            };
            typeArguments.AddRange(types);
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            return GenericName(Identifier("Action"))
               .WithTypeArgumentList(TypeArgumentList(SeparatedList(typeArguments)));
        }

        public static GenericNameSyntax CreatePartialAction(TypeSyntax requestType, TypeSyntax partialType, params TypeSyntax[] types) =>
            CreatePartialAction(requestType, partialType, true, types);

        private static ExpressionStatementSyntax EnsureRegistrationOptionsIsSet(NameSyntax registrationOptionsName, TypeSyntax registrationOptionsType, bool hasCapability) =>
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.CoalesceAssignmentExpression,
                    registrationOptionsName,
                    ( hasCapability
                        ? SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")
                            )
                        )
                        : ParenthesizedLambdaExpression() as LambdaExpressionSyntax
                    )
                   .WithExpressionBody(
                        ObjectCreationExpression(registrationOptionsType is NullableTypeSyntax nts ? nts.ElementType : registrationOptionsType)
                           .WithArgumentList(ArgumentList())
                    )
                )
            );

        private static InvocationExpressionSyntax AddHandler(params ArgumentSyntax[] arguments) => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("registry"),
                    IdentifierName("AddHandler")
                )
            )
           .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        private static ArgumentListSyntax GetHandlerArgumentList() =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler"))
                    }
                )
            );

        private static ArgumentListSyntax GetRegistrationHandlerArgumentList(NameSyntax registrationOptionsName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(registrationOptionsName)
                    }
                )
            );

        private static ArgumentListSyntax GetPartialResultArgumentList(TypeSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
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

        private static ArgumentListSyntax GetPartialResultRegistrationArgumentList(TypeSyntax registrationOptionsName, TypeSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(registrationOptionsName),
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

        private static ArgumentListSyntax GetPartialItemsArgumentList(TypeSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
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
                                ObjectCreationExpression(responseName)
                                   .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("values")))))
                            )
                        )
                    }
                )
            );

        private static ArgumentListSyntax GetPartialItemsRegistrationArgumentList(TypeSyntax registrationOptionsName, TypeSyntax responseName) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(registrationOptionsName),
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

        private static ObjectCreationExpressionSyntax CreateHandlerArgument(NameSyntax className, string innerClassName, params TypeSyntax[] genericArguments) =>
            ObjectCreationExpression(
                QualifiedName(
                    className,
                    GenericName(innerClassName).WithTypeArgumentList(TypeArgumentList(SeparatedList(genericArguments)))
                )
            );

        public static ArrowExpressionClauseSyntax GetNotificationCapabilityHandlerExpression(ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax capabilityName)
        {
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
                           .WithArgumentList(GetHandlerArgumentList())
                    )
                )
            );
        }

        public static BlockSyntax GetNotificationRegistrationHandlerExpression(ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax registrationOptionsName)
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptionsName, false),
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
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptionsFactory")))
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetNotificationRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax registrationOptionsName,
            TypeSyntax capabilityName
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptionsName, true),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Notification",
                                    requestName,
                                    registrationOptionsName,
                                    capabilityName
                                )
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptionsFactory")))
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetRequestCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType,
            TypeSyntax capability
        )
        {
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "RequestCapability",
                                requestType,
                                responseType,
                                capability
                            )
                           .WithArgumentList(GetHandlerArgumentList())
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetVoidRequestCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType,
            TypeSyntax capability
        )
        {
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        CreateHandlerArgument(
                                IdentifierName("LanguageProtocolDelegatingHandlers"),
                                "RequestCapability",
                                requestType,
                                capability
                            )
                           .WithArgumentList(GetHandlerArgumentList())
                    )
                )
            );
        }

        public static BlockSyntax GetRequestRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType,
            TypeSyntax registrationOptions
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, false),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "RequestRegistration",
                                    requestType,
                                    responseType,
                                    registrationOptions
                                )
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptionsFactory")))
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetVoidRequestRegistrationHandlerExpression(ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax registrationOptions)
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, false),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "RequestRegistration",
                                    requestType,
                                    registrationOptions
                                )
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptionsFactory")))
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetRequestRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType,
            TypeSyntax registrationOptions,
            TypeSyntax capability
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, true),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Request",
                                    requestType,
                                    responseType,
                                    registrationOptions,
                                    capability
                                )
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptionsFactory")))
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetVoidRequestRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax registrationOptions,
            TypeSyntax capability
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, true),
                ReturnStatement(
                    AddHandler(
                        Argument(nameExpression),
                        Argument(
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "Request",
                                    requestType,
                                    registrationOptions,
                                    capability
                                )
                               .WithArgumentList(GetRegistrationHandlerArgumentList(IdentifierName("registrationOptionsFactory")))
                        )
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
                           .WithArgumentList(GetHandlerArgumentList())
                    )
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
                           .WithArgumentList(GetHandlerArgumentList())
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax itemType, TypeSyntax responseType,
            TypeSyntax capabilityName
        )
        {
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")
                            ),
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResultCapability",
                                    requestName,
                                    responseType,
                                    itemType,
                                    capabilityName
                                )
                               .WithArgumentList(GetPartialResultArgumentList(responseType))
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetPartialResultRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax itemType, TypeSyntax responseType,
            TypeSyntax registrationOptions
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, false),
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
                                        "PartialResult",
                                        requestType,
                                        responseType,
                                        itemType,
                                        registrationOptions
                                    )
                                   .WithArgumentList(GetPartialResultRegistrationArgumentList(IdentifierName("registrationOptionsFactory"), responseType))
                            )
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetPartialResultRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax itemType, TypeSyntax responseType,
            TypeSyntax registrationOptions,
            TypeSyntax capability
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, true),
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
                                        "PartialResult",
                                        requestType,
                                        responseType,
                                        itemType,
                                        registrationOptions,
                                        capability
                                    )
                                   .WithArgumentList(GetPartialResultRegistrationArgumentList(IdentifierName("registrationOptionsFactory"), responseType))
                            )
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax partialItem, TypeSyntax responseType
        )
        {
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")
                            ),
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResult",
                                    requestName,
                                    responseType,
                                    partialItem
                                )
                               .WithArgumentList(GetPartialResultArgumentList(responseType))
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultsCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax responseType,
            NameSyntax itemName, TypeSyntax capabilityName
        )
        {
            return ArrowExpressionClause(
                AddHandler(
                    Argument(nameExpression),
                    Argument(
                        SimpleLambdaExpression(
                            Parameter(
                                Identifier("_")
                            ),
                            CreateHandlerArgument(
                                    IdentifierName("LanguageProtocolDelegatingHandlers"),
                                    "PartialResultsCapability",
                                    requestName,
                                    responseType,
                                    itemName,
                                    capabilityName
                                )
                               .WithArgumentList(GetPartialItemsArgumentList(responseType))
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetPartialResultsRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType, TypeSyntax itemName, TypeSyntax registrationOptions
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, false),
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
                                        requestType,
                                        responseType,
                                        itemName,
                                        registrationOptions
                                    )
                                   .WithArgumentList(GetPartialItemsRegistrationArgumentList(IdentifierName("registrationOptionsFactory"), responseType))
                            )
                        )
                    )
                )
            );
        }

        public static BlockSyntax GetPartialResultsRegistrationHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestType, TypeSyntax responseType,
            TypeSyntax itemName, TypeSyntax registrationOptions,
            TypeSyntax capability
        )
        {
            return Block(
                EnsureRegistrationOptionsIsSet(IdentifierName("registrationOptionsFactory"), registrationOptions, true),
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
                                        requestType,
                                        responseType,
                                        itemName,
                                        registrationOptions,
                                        capability
                                    )
                                   .WithArgumentList(GetPartialItemsRegistrationArgumentList(IdentifierName("registrationOptionsFactory"), responseType))
                            )
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultsHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax itemName,
            TypeSyntax responseType
        )
        {
            return ArrowExpressionClause(
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
                                    requestName,
                                    responseType,
                                    itemName
                                )
                               .WithArgumentList(GetPartialItemsArgumentList(responseType))
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetNotificationHandlerExpression(ExpressionSyntax nameExpression) =>
            ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("registry"),
                            IdentifierName("AddHandler")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[] {
                                    Argument(nameExpression),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("NotificationHandler"),
                                                    IdentifierName("For")
                                                )
                                            )
                                           .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("handler")))))
                                    )
                                }
                            )
                        )
                    )
            );

        public static ArrowExpressionClauseSyntax GetRequestHandlerExpression(ExpressionSyntax nameExpression) =>
            ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("registry"),
                            IdentifierName("AddHandler")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[] {
                                    Argument(nameExpression),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("RequestHandler"),
                                                    IdentifierName("For")
                                                )
                                            )
                                           .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("handler")))))
                                    )
                                }
                            )
                        )
                    )
            );

        public static ArrowExpressionClauseSyntax GetNotificationInvokeExpression() =>
            ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("mediator"),
                            IdentifierName("SendNotification")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                new[] {
                                    Argument(IdentifierName(@"request"))
                                }
                            )
                        )
                    )
            );

        public static ArrowExpressionClauseSyntax GetRequestInvokeExpression() =>
            ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("mediator"),
                            IdentifierName("SendRequest")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                new[] {
                                    Argument(IdentifierName(@"request")),
                                    Argument(IdentifierName("cancellationToken"))
                                }
                            )
                        )
                    )
            );

        public static ArrowExpressionClauseSyntax GetPartialInvokeExpression(TypeSyntax responseType) =>
            ArrowExpressionClause(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("mediator"),
                                IdentifierName("ProgressManager")
                            ),
                            IdentifierName("MonitorUntil")
                        )
                    )
                   .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                new[] {
                                    Argument(
                                        IdentifierName(@"request")
                                    ),
                                    Argument(
                                        SimpleLambdaExpression(
                                            Parameter(Identifier("value")),
                                            ObjectCreationExpression(responseType is NullableTypeSyntax nts ? nts.ElementType : responseType)
                                               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value")))))
                                        )
                                    ),
                                    Argument(IdentifierName("cancellationToken"))
                                }
                            )
                        )
                    )
            );

        public static string GetExtensionClassName(INamedTypeSymbol symbol) => SpecialCasedHandlerFullName(symbol).Split('.').Last() + "Extensions";

        private static string SpecialCasedHandlerFullName(INamedTypeSymbol symbol)
        {
            if (symbol.IsGenericType)
            {
            }

            return new Regex(@"(\w+(?:\<\w\>)?)$")
                   .Replace(
                        symbol.ToDisplayString(),
                        symbol.Name.Substring(1, symbol.Name.IndexOf("Handler", StringComparison.Ordinal) - 1)
                    )
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
    }
}

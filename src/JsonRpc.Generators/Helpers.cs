#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public static bool IsNotification(TypeDeclarationSyntax symbol) =>
            symbol.BaseList?.Types
                  .Any(
                       z =>
                           z.Type is SimpleNameSyntax and (
                               { Identifier: { Text: "IJsonRpcNotificationHandler" }, Arity: 0 or 1 }
                               or { Identifier: { Text: "IRequest" }, Arity: 0 }
                               )
                   ) == true;

        public static bool IsRequest(TypeDeclarationSyntax symbol) =>
            symbol.BaseList?.Types
                  .Any(
                       z =>
                           z.Type is SimpleNameSyntax and (
                               { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 or 2 }
                               or { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 }
                               or { Identifier: { Text: "IRequest" }, Arity: 1 }
                               )
                   ) == true;

        public static ExpressionSyntax GetJsonRpcMethodName(TypeDeclarationSyntax interfaceSyntax)
        {
            var methodAttribute = interfaceSyntax.AttributeLists
                                                 .SelectMany(z => z.Attributes)
                                                 .First(z => z.Name.ToString() == "MethodAttribute" || z.Name.ToString() == "Method");

            return methodAttribute.ArgumentList!.Arguments[0].Expression;
        }

        public static SyntaxSymbol GetResponseType(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            TypeSyntax? type = null!;
            foreach (var baseType in syntax.BaseList?.Types.AsEnumerable() ?? Array.Empty<BaseTypeSyntax>())
            {
                type = baseType.Type switch {
                    GenericNameSyntax gns => gns switch {
                        { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 } => IdentifierName("MediatR.Unit"),
                        { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 2 } => gns.TypeArgumentList.Arguments[1],
                        { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 }  => gns.TypeArgumentList.Arguments[0],
                        { Identifier: { Text: "IRequest" }, Arity: 1 }               => gns.TypeArgumentList.Arguments[0],
                        _                                                            => null
                    },
                    SimpleNameSyntax sns and { Identifier: { Text: "IRequest" } } => IdentifierName("MediatR.Unit"),
                    _                                                             => null
                };
                if (type != null) break;
            }

            if (type == null) throw new ArgumentException($"Response Type {symbol.ToDisplayString()} is not a name symbol", nameof(symbol));

            var handlerInterface = symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            if (handlerInterface?.TypeArguments[1] is INamedTypeSymbol ns)
                return new SyntaxSymbol(type, ns);
            handlerInterface = symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IRequest" && z.Arity == 1);
            if (handlerInterface?.TypeArguments[0] is INamedTypeSymbol ns2)
                return new SyntaxSymbol(type, ns2);
            throw new ArgumentException($"Response Type {symbol.ToDisplayString()} is not a name symbol", nameof(symbol));
        }

        public static SyntaxSymbol? GetRequestType(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            TypeSyntax? type;
            if (syntax.ConstraintClauses.Any())
            {
                type = syntax.ConstraintClauses.First()
                             .Constraints
                             .OfType<TypeConstraintSyntax>()
                             .FirstOrDefault()?.Type;
            }
            else if (syntax.BaseList?.Types.Select(z => z.Type).OfType<SimpleNameSyntax>().Any(z => z.Identifier.Text == "IRequest") == true)
            {
                type = ParseTypeName(syntax.Identifier.ToFullString());
            }
            else
            {
                var interfaceType = syntax.BaseList?.Types
                                          .Select(
                                               z => z.Type is GenericNameSyntax gns and (
                                                   { Identifier: { Text: "IJsonRpcRequestHandler" } } or
                                                   { Identifier: { Text: "ICanBeResolvedHandler" } } or
                                                   { Identifier: { Text: "IJsonRpcNotificationHandler" } }
                                                   )
                                                   ? gns
                                                   : null!
                                           )
                                          .FirstOrDefault(z => z is not null);

                type = interfaceType?.TypeArgumentList.Arguments[0];
            }

            if (type == null) return null;

            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            var arg = handlerInterface?.TypeArguments[0] ?? ( symbol.AllInterfaces.Any(z => z.Name == "IRequest" && z.Arity == 1) ? symbol as ITypeSymbol : null );
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

            return null;
        }

        public static SyntaxSymbol? GetCapability(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, LspAttributes? lspAttributes)
        {
            TypeSyntax? type = null!;
            foreach (var baseType in syntax.BaseList?.Types.AsEnumerable() ?? Array.Empty<BaseTypeSyntax>())
            {
                type = baseType.Type switch {
                    GenericNameSyntax gns => gns switch {
                        { Identifier: { Text: "ICapability" }, Arity: 1 }   => gns.TypeArgumentList.Arguments[0],
                        { Identifier: { Text: "IRegistration" }, Arity: 2 } => gns.TypeArgumentList.Arguments[1],
                        _                                                   => null
                    },
                    _ => null
                };
                if (type != null) break;
            }

            if (type == null
             && lspAttributes?.CapabilityAttribute?.Syntax is { } and { ArgumentList: { Arguments: { Count: > 0 } } arguments }
             && arguments.Arguments[0].Expression is TypeOfExpressionSyntax toes)
            {
                type = toes.Type;
            }

            if (type == null)
                return null;

            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "ICapability" && z.TypeArguments.Length == 1)?.TypeArguments[0]
                                ?? symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 2)?.TypeArguments[1];
            if (handlerInterface == null
             && lspAttributes?.CapabilityAttribute?.Data is { } and { ConstructorArguments: { Length: > 0 } constructorArguments }
             && constructorArguments[0].Value is INamedTypeSymbol nts)
            {
                handlerInterface = nts;
            }

            return new SyntaxSymbol(type, ( handlerInterface as INamedTypeSymbol )!);
        }

        public static SyntaxSymbol? GetRegistrationOptions(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, LspAttributes? lspAttributes)
        {
            TypeSyntax? type = null!;
            foreach (var baseType in syntax.BaseList?.Types.AsEnumerable() ?? Array.Empty<BaseTypeSyntax>())
            {
                type = baseType.Type switch {
                    GenericNameSyntax gns and { Identifier: { Text: "IRegistration" }, Arity: >0 } => gns.TypeArgumentList.Arguments[0],
                    _                                                                              => null
                };
                if (type != null) break;
            }

            if (type == null
             && lspAttributes?.RegistrationOptionsAttribute?.Syntax is { } and { ArgumentList: { Arguments: { Count: > 0 } } arguments }
             && arguments.Arguments[0].Expression is TypeOfExpressionSyntax toes)
            {
                type = toes.Type;
            }

            if (type == null)
                return null;
            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 1)?.TypeArguments[0]
                                ?? symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRegistration" && z.TypeArguments.Length == 2)?.TypeArguments[0];
            if (handlerInterface == null
             && lspAttributes?.RegistrationOptionsAttribute?.Data is { } and { ConstructorArguments: { Length: > 0 } constructorArguments }
             && constructorArguments[0].Value is INamedTypeSymbol nts)
            {
                handlerInterface = nts;
            }

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

        public static ExpressionStatementSyntax EnsureRegistrationOptionsIsSet(NameSyntax registrationOptionsName, TypeSyntax registrationOptionsType, bool hasCapability) =>
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

        public static InvocationExpressionSyntax AddHandler(params ArgumentSyntax[] arguments) => InvocationExpression(
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

        public static ArgumentListSyntax GetRegistrationHandlerArgumentList(TypeSyntax registrationOptionsName, TypeArgumentListSyntax genericTypes) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        Argument(IdentifierName("handler")),
                        Argument(GetRegistrationOptionsAdapter(registrationOptionsName, genericTypes))

                    }
                )
            );

        public static InvocationExpressionSyntax GetRegistrationOptionsAdapter(TypeSyntax registrationOptionsName, TypeArgumentListSyntax genericTypes)
        {
            return InvocationExpression(QualifiedName(IdentifierName("RegistrationOptionsFactoryAdapter")
                                                         , GenericName("Adapt").WithTypeArgumentList(genericTypes)))
               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(registrationOptionsName))));
        }

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

        public static ObjectCreationExpressionSyntax CreateHandlerArgument(NameSyntax className, string innerClassName, params TypeSyntax[] genericArguments) =>
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

            var substringIndex = symbol.Name.IndexOf("Handler", StringComparison.Ordinal);
            if (substringIndex is -1)
            {
                substringIndex = symbol.Name.IndexOf("Params", StringComparison.Ordinal);
            }

            if (substringIndex is -1)
            {
                substringIndex = symbol.Name.Length - 1;
            }

            var start = 0;
            if (symbol.Name.StartsWith("I", StringComparison.Ordinal) && char.IsUpper(symbol.Name[1]))
            {
                start = 1;
                substringIndex -= 1;
            }

            return new Regex(@"(\w+(?:\<\w+\>)?)$")
                   .Replace(
                        symbol.ToDisplayString(),
                        symbol.Name.Substring(start, substringIndex)
                    )
                ;
        }

        public static string SpecialCasedHandlerName(INamedTypeSymbol symbol)
        {
            var name = SpecialCasedHandlerFullName(symbol);
            return name.Substring(name.LastIndexOf('.') + 1);
        }
    }
}

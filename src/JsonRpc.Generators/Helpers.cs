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
                               or { Identifier: { Text: "IPartialItemRequest" }, Arity: 2 }
                               or { Identifier: { Text: "IPartialItemsRequest" }, Arity: 2 }
                               or { Identifier: { Text: "IRequest" }, Arity: 1 }
                               or { Identifier: { Text: "IJsonRpcRequest" }, Arity: 0 }
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
                        { Identifier: { Text: "IPartialItemRequest" }, Arity: 2 }    => gns.TypeArgumentList.Arguments[0],
                        { Identifier: { Text: "IPartialItemsRequest" }, Arity: 2 }   => gns.TypeArgumentList.Arguments[0],
                        { Identifier: { Text: "IRequest" }, Arity: 1 }               => gns.TypeArgumentList.Arguments[0],
                        _                                                            => null
                    },
                    SimpleNameSyntax sns and { Identifier: { Text: "IRequest" } }        => IdentifierName("MediatR.Unit"),
                    SimpleNameSyntax sns and { Identifier: { Text: "IJsonRpcRequest" } } => IdentifierName("MediatR.Unit"),
                    _                                                                    => null
                };
                if (type != null) break;
            }

            if (type == null) throw new ArgumentException($"Response Type {symbol.ToDisplayString()} is not a name symbol", nameof(symbol));

            var handlerInterface = symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            if (handlerInterface?.TypeArguments[1] is INamedTypeSymbol ns)
                return new SyntaxSymbol(type, ns);
            handlerInterface = symbol.AllInterfaces.FirstOrDefault(z => ( z.Name == "IRequest" && z.Arity == 1 ));
            if (handlerInterface?.TypeArguments[0] is INamedTypeSymbol ns2)
                return new SyntaxSymbol(type, ns2);
            throw new ArgumentException($"Response Type {symbol.ToDisplayString()} is not a name symbol", nameof(symbol));
        }

        public static SyntaxSymbol? GetRequestType(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            TypeSyntax? type = null;
            if (syntax.ConstraintClauses.Any())
            {
                type = syntax.ConstraintClauses.First()
                             .Constraints
                             .OfType<TypeConstraintSyntax>()
                             .FirstOrDefault()?.Type;
            }
            else if (syntax.BaseList?.Types.Select(z => z.Type).OfType<SimpleNameSyntax>().Any(z => z.Identifier.Text == "IRequest" || z.Identifier.Text == "IJsonRpcRequest")
                  == true)
            {
                type = ParseTypeName(syntax.Identifier.ToFullString());
            }
            else
            {
                foreach (var baseType in syntax.BaseList?.Types.AsEnumerable() ?? Array.Empty<BaseTypeSyntax>())
                {
                    type = baseType.Type switch {
                        GenericNameSyntax gns => gns switch {
                            { Identifier: { Text: "IJsonRpcRequestHandler" } }          => gns.TypeArgumentList.Arguments[0],
                            { Identifier: { Text: "IJsonRpcNotificationHandler" } }     => gns.TypeArgumentList.Arguments[0],
                            { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 } => gns.TypeArgumentList.Arguments[0],
                            { Identifier: { Text: "IRequest" } }                        => ParseTypeName(syntax.Identifier.ToFullString()),
                            { Identifier: { Text: "IJsonRpcRequest" } }                 => ParseTypeName(syntax.Identifier.ToFullString()),
                            { Identifier: { Text: "IPartialItemRequest" }, Arity: 2 }   => ParseTypeName(syntax.Identifier.ToFullString()),
                            { Identifier: { Text: "IPartialItemsRequest" }, Arity: 2 }  => ParseTypeName(syntax.Identifier.ToFullString()),
                            _                                                           => null,
                        },
                        _ => null,
                    };
                    if (type != null) break;
                }
            }

            if (type == null) return null;

            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            var arg = handlerInterface?.TypeArguments[0] ?? ( symbol.AllInterfaces.Any(z => (z.Name == "IRequest" && z.Arity == 1) || z.Name == "IJsonRpcRequest") ? symbol as ITypeSymbol : null );
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

        public static InvocationExpressionSyntax AddHandler(ExpressionSyntax syntax, params ArgumentSyntax[] arguments) =>
            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, syntax, IdentifierName("AddHandler")))
               .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        private static ArgumentListSyntax GetHandlerArgumentList() =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        HandlerArgument
                    }
                )
            );

        public static ArgumentListSyntax GetRegistrationHandlerArgumentList(
            TypeSyntax registrationOptionsName, TypeSyntax registrationType, ArgumentSyntax handlerArgument, TypeSyntax? capabilityType, bool includeId
        ) =>
            ArgumentList(
                SeparatedList(
                    includeId
                        ? new[] {
                            Argument(IdentifierName("id")),
                            handlerArgument,
                            Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityType))
                        }
                        : new[] {
                            handlerArgument,
                            Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityType))
                        }
                )
            );

        public static ArgumentSyntax HandlerArgument = Argument(IdentifierName("handler"));
        public static ArgumentSyntax ResolveHandlerArgument = Argument(IdentifierName("resolveHandler"));

        public static ArgumentSyntax GetHandlerAdapterArgument(
            TypeArgumentListSyntax typeArgumentListSyntax, ArgumentSyntax handlerArgument, TypeSyntax? capabilityType, bool isPartial
        )
        {
            var adapterName = ( isPartial ? "Partial" : "Handler" ) + "Adapter";
            TypeSyntax name = IdentifierName(adapterName);
            if (capabilityType is { })
            {
                name = GenericName(Identifier(adapterName))
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(capabilityType)));
            }

            return Argument(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            name,
                            GenericName(Identifier("Adapt"))
                               .WithTypeArgumentList(typeArgumentListSyntax)
                        )
                    )
                   .WithArgumentList(ArgumentList(SingletonSeparatedList(handlerArgument)))
            );
        }

        public static InvocationExpressionSyntax GetRegistrationOptionsAdapter(
            TypeSyntax registrationOptionsName,
            TypeSyntax registrationType,
            TypeSyntax? capabilityType
        )
        {
            NameSyntax name = IdentifierName("RegistrationAdapter");
            if (capabilityType is { })
            {
                name = GenericName(Identifier("RegistrationAdapter"))
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(capabilityType)));
            }

            return InvocationExpression(
                    QualifiedName(
                        name
                      , GenericName("Adapt").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { registrationType })))
                    )
                )
               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(registrationOptionsName))));
        }

        private static ArgumentListSyntax GetPartialResultArgumentList(TypeSyntax responseName, ArgumentSyntax handlerArgument) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        handlerArgument,
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

        private static ArgumentListSyntax GetPartialItemsArgumentList(TypeSyntax responseName, ArgumentSyntax handlerArgument) =>
            ArgumentList(
                SeparatedList(
                    new[] {
                        handlerArgument,
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

        public static ObjectCreationExpressionSyntax CreateHandlerArgument(string innerClassName, params TypeSyntax[] genericArguments) =>
            ObjectCreationExpression(GenericName(innerClassName).WithTypeArgumentList(TypeArgumentList(SeparatedList(genericArguments))));

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
                           .AddArgumentListArguments(
                                GetHandlerAdapterArgument(
                                    TypeArgumentList(SeparatedList(new[] { requestName })),
                                    HandlerArgument,
                                    capabilityName,
                                    false
                                )
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
                           .AddArgumentListArguments(
                                GetHandlerAdapterArgument(
                                    TypeArgumentList(SeparatedList(new[] { requestType, responseType })),
                                    HandlerArgument,
                                    capability,
                                    false
                                )
                            )
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
                           .AddArgumentListArguments(
                                GetHandlerAdapterArgument(
                                    TypeArgumentList(SeparatedList(new[] { requestType })),
                                    HandlerArgument,
                                    capability,
                                    false
                                )
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
                               .AddArgumentListArguments(
                                    GetHandlerAdapterArgument(
                                        TypeArgumentList(SeparatedList(new[] { requestName, itemType })),
                                        HandlerArgument,
                                        capabilityName,
                                        true
                                    )
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
                               .WithArgumentList(GetPartialResultArgumentList(responseType, HandlerArgument))
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
                               .AddArgumentListArguments(
                                    GetHandlerAdapterArgument(
                                        TypeArgumentList(SeparatedList(new[] { requestName, itemName })),
                                        HandlerArgument,
                                        capabilityName,
                                        true
                                    )
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
                               .WithArgumentList(GetPartialItemsArgumentList(responseType, HandlerArgument))
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
                                           .WithArgumentList(ArgumentList(SingletonSeparatedList(HandlerArgument)))
                                    )
                                }
                            )
                        )
                    )
            );

        public static ArrowExpressionClauseSyntax GetRequestHandlerExpression(
            RequestItem item,
            ExpressionSyntax nameExpression
        ) =>
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
                            SeparatedList(
                                new[] {
                                    Argument(nameExpression),
                                    Argument(
                                        item.IsUnit
                                            ? ObjectCreationExpression(
                                                    QualifiedName(
                                                        IdentifierName("DelegatingHandlers"),
                                                        GenericName(Identifier("Request"))
                                                           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(item.Request.Syntax)))
                                                    )
                                                )
                                               .WithArgumentList(ArgumentList(SingletonSeparatedList(HandlerArgument)))
                                            : InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("RequestHandler"),
                                                        IdentifierName("For")
                                                    )
                                                )
                                               .WithArgumentList(ArgumentList(SingletonSeparatedList(HandlerArgument)))
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

        public static TypeConstraintSyntax HandlerIdentityConstraint { get; } = TypeConstraint(NullableType(IdentifierName("HandlerIdentity")));

        public static TypeParameterConstraintClauseSyntax HandlerIdentityConstraintClause(IdentifierNameSyntax? openGenericType = null) =>
            TypeParameterConstraintClause(openGenericType ?? IdentifierName("T"))
               .WithConstraints(SeparatedList(new TypeParameterConstraintSyntax[] { HandlerIdentityConstraint, ConstructorConstraint() }));

        public static LocalDeclarationStatementSyntax NewGuid { get; } = LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
               .WithVariables(
                    SingletonSeparatedList(
                        VariableDeclarator(Identifier("id"))
                           .WithInitializer(
                                EqualsValueClause(
                                    InvocationExpression(
                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Guid"), IdentifierName("NewGuid"))
                                    )
                                )
                            )
                    )
                )
        );
    }

    public static class SyntaxExtensions
    {
        public static TypeSyntax EnsureNullable(this TypeSyntax typeSyntax) => typeSyntax is NullableTypeSyntax nts ? nts : NullableType(typeSyntax);
        public static TypeSyntax EnsureNotNullable(this TypeSyntax typeSyntax) => typeSyntax is NullableTypeSyntax nts ? nts.ElementType : typeSyntax;


        public static BaseMethodDeclarationSyntax MakeMethodNullable(this BaseMethodDeclarationSyntax syntax, IdentifierNameSyntax identifierNameSyntax)
        {
            if (syntax is MethodDeclarationSyntax mds)
            {
                syntax = mds.WithReturnType(mds.ReturnType.EnsureNullable());
            }

            if (syntax is ConversionOperatorDeclarationSyntax cods)
            {
                syntax = cods.WithType(cods.Type.EnsureNullable());
            }

            if (syntax.ExpressionBody is not null)
            {
                syntax = syntax.WithExpressionBody(syntax.ExpressionBody.WithExpression(syntax.ExpressionBody.Expression.InsideNullableSwitchExpression(identifierNameSyntax)));
            }

            return syntax
                  .WithParameterList(
                       ParameterList(SeparatedList(syntax.ParameterList.Parameters.Select(parameter => parameter.WithType(parameter.Type?.EnsureNullable())).ToArray()))
                   )
                  .AddAttributeLists(
                       AttributeList(
                               SingletonSeparatedList(
                                   Attribute(
                                           QualifiedName(
                                               QualifiedName(QualifiedName(IdentifierName("System"), IdentifierName("Diagnostics")), IdentifierName("CodeAnalysis")),
                                               IdentifierName("NotNullIfNotNull")
                                           )
                                       )
                                      .WithArgumentList(
                                           AttributeArgumentList(
                                               SingletonSeparatedList(
                                                   AttributeArgument(
                                                       LiteralExpression(
                                                           SyntaxKind.StringLiteralExpression,
                                                           Literal(identifierNameSyntax.Identifier.Text)
                                                       )
                                                   )
                                               )
                                           )
                                       )
                               )
                           )
                          .WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.ReturnKeyword)))
                   );
        }

        public static MethodDeclarationSyntax MakeMethodNullable(this MethodDeclarationSyntax syntax, IdentifierNameSyntax identifierNameSyntax)
        {
            return ( MakeMethodNullable(syntax as BaseMethodDeclarationSyntax, identifierNameSyntax) as MethodDeclarationSyntax )!;
        }

        public static ConversionOperatorDeclarationSyntax MakeMethodNullable(this ConversionOperatorDeclarationSyntax syntax, IdentifierNameSyntax identifierNameSyntax)
        {
            return ( MakeMethodNullable(syntax as BaseMethodDeclarationSyntax, identifierNameSyntax) as ConversionOperatorDeclarationSyntax )!;
        }

        public static SwitchExpressionSyntax InsideNullableSwitchExpression(this ExpressionSyntax creationExpression, IdentifierNameSyntax name)
        {
            return SwitchExpression(name)
               .WithArms(
                    SeparatedList(
                        new[] {
                            SwitchExpressionArm(
                                UnaryPattern(ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                                creationExpression
                            ),
                            SwitchExpressionArm(DiscardPattern(), LiteralExpression(SyntaxKind.NullLiteralExpression))
                        }
                    )
                );
        }

        public static ClassDeclarationSyntax WithHandlerIdentityConstraint(this ClassDeclarationSyntax syntax, IdentifierNameSyntax? openGenericType = null)
        {
            openGenericType ??= IdentifierName("T");
            return syntax
                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(openGenericType.Identifier.Text))))
                  .WithConstraintClauses(SingletonList(Helpers.HandlerIdentityConstraintClause(openGenericType)));
        }

        public static MethodDeclarationSyntax WithHandlerIdentityConstraint(this MethodDeclarationSyntax syntax, IdentifierNameSyntax? openGenericType = null)
        {
            openGenericType ??= IdentifierName("T");
            return syntax
                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(openGenericType.Identifier.Text))))
                  .WithConstraintClauses(SingletonList(Helpers.HandlerIdentityConstraintClause(openGenericType)));
        }
    }
}

#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                               or { Identifier: { Text: "IPartialItemRequest" or "IPartialItemWithInitialValueRequest" }, Arity: 2 }
                               or { Identifier: { Text: "IPartialItemsRequest" or "IPartialItemsWithInitialValueRequest" }, Arity: 2 }
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
                type = baseType.Type switch
                {
                    GenericNameSyntax gns => gns switch
                    {
                        { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 }                                       => ParseName("MediatR.Unit"),
                        { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 2 }                                       => gns.TypeArgumentList.Arguments[1],
                        { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 }                                        => gns.TypeArgumentList.Arguments[0],
                        { Identifier: { Text: "IPartialItemRequest" or "IPartialItemWithInitialValueRequest" }, Arity: 2 } => gns.TypeArgumentList.Arguments[0],
                        { Identifier: { Text: "IPartialItemsRequest" or "IPartialItemsWithInitialValueRequest" }, Arity: 2 } => gns.TypeArgumentList.Arguments
                            [0],
                        { Identifier: { Text: "IRequest" }, Arity: 1 } => gns.TypeArgumentList.Arguments[0],
                        _                                              => null
                    },
                    SimpleNameSyntax and { Identifier: { Text: "IRequest" } }        => ParseName("MediatR.Unit"),
                    SimpleNameSyntax and { Identifier: { Text: "IJsonRpcRequest" } } => ParseName("MediatR.Unit"),
                    _                                                                => null
                };
                if (type != null) break;
            }

            if (type == null) throw new ArgumentException($"Response Syntax {syntax.ToString()} is not a name syntax", nameof(syntax));

            var handlerInterface = symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            if (handlerInterface?.TypeArguments[1] is INamedTypeSymbol || handlerInterface?.TypeArguments[1] is ITypeParameterSymbol)
                return new SyntaxSymbol(type, handlerInterface.TypeArguments[1]);
            handlerInterface = symbol.AllInterfaces.FirstOrDefault(z => z.Name == "IRequest" && z.Arity == 1);
            if (handlerInterface?.TypeArguments[0] is INamedTypeSymbol || handlerInterface?.TypeArguments[0] is ITypeParameterSymbol)
                return new SyntaxSymbol(type, handlerInterface.TypeArguments[0]);
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
            else if (syntax.BaseList?.Types.Select(z => z.Type).OfType<SimpleNameSyntax>()
                           .Any(z => z.Identifier.Text == "IRequest" || z.Identifier.Text == "IJsonRpcRequest")
                  == true)
            {
                type = IdentifierName(syntax.Identifier.Text);
            }
            else
            {
                foreach (var baseType in syntax.BaseList?.Types.AsEnumerable() ?? Array.Empty<BaseTypeSyntax>())
                {
                    type = baseType.Type switch
                    {
                        GenericNameSyntax gns => gns switch
                        {
                            { Identifier: { Text: "IJsonRpcRequestHandler" } }          => gns.TypeArgumentList.Arguments[0],
                            { Identifier: { Text: "IJsonRpcNotificationHandler" } }     => gns.TypeArgumentList.Arguments[0],
                            { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 } => gns.TypeArgumentList.Arguments[0],
                            { Identifier: { Text: "IRequest" }, Arity: 1 }              => IdentifierName(syntax.Identifier.Text),
                            { Identifier: { Text: "IRequest" }, Arity: 0 }              => IdentifierName(syntax.Identifier.Text),
                            { Identifier: { Text: "IJsonRpcRequest" } }                 => IdentifierName(syntax.Identifier.Text),
                            { Identifier: { Text: "IPartialItemRequest" or "IPartialItemWithInitialValueRequest" }, Arity: 2 } => IdentifierName(
                                syntax.Identifier.Text
                            ),
                            { Identifier: { Text: "IPartialItemsRequest" or "IPartialItemsWithInitialValueRequest" }, Arity: 2 } => IdentifierName(
                                syntax.Identifier.Text
                            ),
                            _ => null,
                        },
                        _ => null,
                    };
                    if (type != null) break;
                }
            }

            if (type == null) return null;
            if (type is IdentifierNameSyntax ins && ins.Identifier.Text == syntax.Identifier.Text && syntax.TypeParameterList is { Parameters: { Count: > 0 } })
            {
                type = GenericName(syntax.Identifier.Text)
                   .WithTypeArgumentList(
                        TypeArgumentList(SeparatedList<TypeSyntax>(syntax.TypeParameterList?.Parameters.Select(z => IdentifierName(z.Identifier.Text))))
                    );
            }

            var handlerInterface = symbol.AllInterfaces
                                         .FirstOrDefault(z => z.Name == "IRequestHandler" && z.TypeArguments.Length == 2);
            var arg = handlerInterface?.TypeArguments[0]
                   ?? ( symbol.AllInterfaces.Any(z => z.Name == "IRequest" && z.Arity == 1 || z.Name == "IJsonRpcRequest") ? symbol as ITypeSymbol : null );
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
                type = baseType.Type switch
                {
                    GenericNameSyntax gns => gns switch
                    {
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
                type = baseType.Type switch
                {
                    GenericNameSyntax gns and { Identifier: { Text: "IRegistration" }, Arity: > 0 } => gns.TypeArgumentList.Arguments[0],
                    _                                                                               => null
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
            var handlerInterface = symbol.AllInterfaces.Concat(requestType.Symbol.AllInterfaces).FirstOrDefault(
                z => z is { Name: "IPartialItems", TypeArguments.Length: 1 } or { Name: "IPartialItemsWithInitialValue", TypeArguments.Length: 2 }
            );
            var localSymbol = handlerInterface?.TypeArguments[0] as INamedTypeSymbol;
            if (localSymbol == null) return null;
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z is { Identifier.Text: "IPartialItemsRequest" or "IPartialItemsWithInitialValueRequest", Arity: 2 })
                             .Select(z => z!.TypeArgumentList.Arguments[1])
                             .FirstOrDefault();

            return new SyntaxSymbol(type ?? ResolveTypeName(localSymbol), localSymbol);
        }

        public static (SyntaxSymbol? partialItem, bool inheritsFromSelf) GetPartialItem(
            TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, SyntaxSymbol requestType, Compilation compilation
        )
        {
            var handlerInterface = symbol.AllInterfaces.Concat(requestType.Symbol.AllInterfaces)
                                         .FirstOrDefault(
                                              z => z is { Name: "IPartialItem", TypeArguments.Length: 1 } or
                                                  { Name: "IPartialItemWithInitialValue", TypeArguments.Length: 2 }
                                          );
            var localSymbol = handlerInterface?.TypeArguments[0] as INamedTypeSymbol;
            if (localSymbol == null) return ( null, false );
            var type = syntax.BaseList?.Types
                             .Select(z => z.Type is GenericNameSyntax genericNameSyntax ? genericNameSyntax : null)
                             .Where(z => z != null)
                             .Where(z => z is { Identifier.Text: "IPartialItemRequest" or "IPartialItemWithInitialValueRequest", Arity: 2 })
                             .Select(z => z!.TypeArgumentList.Arguments[1])
                             .FirstOrDefault();
            return (
                new SyntaxSymbol(type ?? ResolveTypeName(localSymbol), localSymbol),
                handlerInterface!.TypeArguments.Length == 2 && compilation.HasImplicitConversion(handlerInterface!.TypeArguments[1], handlerInterface.TypeArguments[0])
            );
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

        public static GenericNameSyntax CreatePartialAction(
            TypeSyntax requestType, TypeSyntax partialType, bool withCancellationToken, params TypeSyntax[] types
        )
        {
            var typeArguments = new List<TypeSyntax>
            {
                requestType,
                GenericName("IObserver").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { partialType }))),
            };

            typeArguments.AddRange(types);
            if (withCancellationToken)
            {
                typeArguments.Add(IdentifierName("CancellationToken"));
            }

            return GenericName(Identifier("Action"))
               .WithTypeArgumentList(TypeArgumentList(SeparatedList(typeArguments)));
        }

        public static ExpressionStatementSyntax EnsureRegistrationOptionsIsSet(
            NameSyntax registrationOptionsName, TypeSyntax registrationOptionsType, bool hasCapability
        ) =>
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
                    new[]
                    {
                        HandlerArgument
                    }
                )
            );

        public static ArgumentListSyntax GetRegistrationHandlerArgumentList(
            TypeSyntax registrationOptionsName, TypeSyntax registrationType, ArgumentSyntax handlerArgument, ArgumentSyntax? initialArgument,
            TypeSyntax? capabilityType, bool includeId
        ) =>
            ArgumentList(
                SeparatedList(
                    ( includeId
                        ? new[]
                        {
                            Argument(IdentifierName("id")),
                            initialArgument!,
                            handlerArgument,
                            Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityType))
                        }
                        : new[]
                        {
                            initialArgument!,
                            handlerArgument,
                            Argument(GetRegistrationOptionsAdapter(registrationOptionsName, registrationType, capabilityType))
                        } )
                   .Where(z => z is not null)
                )
            );

        public static ArgumentSyntax HandlerArgument = Argument(IdentifierName("handler"));
        public static ArgumentSyntax InitialHandlerArgument = Argument(IdentifierName("initialHandler"));
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
                    QualifiedName(name, GenericName("Adapt").WithTypeArgumentList(TypeArgumentList(SeparatedList(new[] { registrationType }))))
                )
               .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(registrationOptionsName))));
        }

        private static SeparatedSyntaxList<ArgumentSyntax> GetPartialResultArgumentList(
            TypeSyntax responseName, ArgumentSyntax handlerArgument, ArgumentSyntax? initialArgument
        ) =>
            SeparatedList(
                new[]
                {
                    initialArgument!,
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
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            responseName.EnsureNotNullable(),
                            IdentifierName("From")
                        )
                    )
                }.Where(z => z is not null)
            )!;

        private static ArgumentListSyntax GetPartialItemsArgumentList(
            TypeSyntax responseName, ArgumentSyntax handlerArgument, ArgumentSyntax? initialArgument
        ) =>
            ArgumentList(
                SeparatedList(
                    new[]
                        {
                            initialArgument!,
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
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    responseName.EnsureNotNullable(),
                                    IdentifierName("From")
                                )
                            )
                        }
                       .Where(z => z is not null)
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

        public static ArrowExpressionClauseSyntax GetNotificationCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax capabilityName
        )
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

        public static ArrowExpressionClauseSyntax GetPartialResultCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax itemType, TypeSyntax responseType,
            TypeSyntax capabilityName, bool requestPartialHasInitialValue
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
                                    requestPartialHasInitialValue ? "PartialResultCapabilityWithInitialValue" : "PartialResultCapability",
                                    requestName,
                                    responseType,
                                    itemType,
                                    capabilityName
                                )
                               .WithArgumentList(
                                    ArgumentList(
                                        GetPartialResultArgumentList(
                                            responseType,
                                            GetHandlerAdapterArgument(
                                                TypeArgumentList(SeparatedList(new[] { requestName, itemType })),
                                                HandlerArgument,
                                                capabilityName,
                                                true
                                            ),
                                            requestPartialHasInitialValue
                                                ? GetHandlerAdapterArgument(
                                                    TypeArgumentList(SeparatedList(new[] { requestName, responseType })),
                                                    InitialHandlerArgument,
                                                    capabilityName,
                                                    false
                                                )
                                                : null
                                        )
                                    )
                                )
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax partialItem, TypeSyntax responseType, bool requestPartialHasInitialValue
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
                                    requestPartialHasInitialValue ? "PartialResultWithInitialValue" : "PartialResult",
                                    requestName,
                                    responseType,
                                    partialItem
                                )
                               .WithArgumentList(
                                    ArgumentList(
                                        GetPartialResultArgumentList(
                                            responseType, HandlerArgument, requestPartialHasInitialValue ? InitialHandlerArgument : null
                                        )
                                    )
                                )
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultsCapabilityHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax responseType,
            TypeSyntax itemName, TypeSyntax capabilityName, bool requestPartialHasInitialValue
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
                                    requestPartialHasInitialValue ? "PartialResultsCapabilityWithInitialValue" : "PartialResultsCapability",
                                    requestName,
                                    responseType,
                                    itemName,
                                    capabilityName
                                )
                               .WithArgumentList(
                                    ArgumentList(
                                        GetPartialResultArgumentList(
                                            responseType,
                                            GetHandlerAdapterArgument(
                                                TypeArgumentList(SeparatedList(new[] { requestName, itemName })),
                                                HandlerArgument,
                                                capabilityName,
                                                true
                                            ),
                                            requestPartialHasInitialValue
                                                ? GetHandlerAdapterArgument(
                                                    TypeArgumentList(SeparatedList(new[] { requestName, responseType })),
                                                    InitialHandlerArgument,
                                                    capabilityName,
                                                    false
                                                )
                                                : null
                                        )
                                    )
                                )
                        )
                    )
                )
            );
        }

        public static ArrowExpressionClauseSyntax GetPartialResultsHandlerExpression(
            ExpressionSyntax nameExpression, TypeSyntax requestName, TypeSyntax itemName,
            TypeSyntax responseType, bool requestPartialHasInitialValue
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
                                    requestPartialHasInitialValue ? "PartialResultsWithInitialValue" : "PartialResults",
                                    requestName,
                                    responseType,
                                    itemName
                                )
                               .WithArgumentList(
                                    GetPartialItemsArgumentList(
                                        responseType,
                                        GetHandlerAdapterArgument(
                                            TypeArgumentList(SeparatedList(new[] { requestName, itemName })),
                                            HandlerArgument,
                                            null,
                                            true
                                        ),
                                        requestPartialHasInitialValue
                                            ? GetHandlerAdapterArgument(
                                                TypeArgumentList(SeparatedList(new[] { requestName, responseType })),
                                                InitialHandlerArgument,
                                                null,
                                                false
                                            )
                                            : null
                                    )
                                )
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
                                new SyntaxNodeOrToken[]
                                {
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
                                new[]
                                {
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
                                               .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            GetHandlerAdapterArgument(
                                                                TypeArgumentList(SeparatedList(new[] { item.Request.Syntax })),
                                                                HandlerArgument,
                                                                null,
                                                                false
                                                            )
                                                        )
                                                    )
                                                )
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
                                new[]
                                {
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
                                new[]
                                {
                                    Argument(IdentifierName(@"request")),
                                    Argument(IdentifierName("cancellationToken"))
                                }
                            )
                        )
                    )
            );

        public static ArrowExpressionClauseSyntax GetPartialInvokeExpression(
            TypeSyntax responseType, TypeSyntax? partialItemType, bool partialItemTypeInheritsFromSelf
        )
        {
            var realResponseType = responseType is NullableTypeSyntax nts ? nts.ElementType : responseType;
            var factoryArgument = Argument(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    realResponseType.EnsureNotNullable(),
                    IdentifierName("From")
                )
            );
            var arguments = new[]
            {
                Argument(IdentifierName(@"request")),
                factoryArgument,
                Argument(IdentifierName("cancellationToken"))
            };
            if (partialItemType is { } && !partialItemTypeInheritsFromSelf)
            {
                var realPartialItemType = partialItemType is NullableTypeSyntax nts2 ? nts2.ElementType : partialItemType;
                arguments = new[]
                {
                    arguments[0],
                    arguments[1],
                    Argument(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            realPartialItemType.EnsureNotNullable(),
                            IdentifierName("From")
                        )
                    ),
                    arguments[2]
                };
            }

            return ArrowExpressionClause(
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
                   .WithArgumentList(ArgumentList(SeparatedList(arguments)))
            );
        }

        public static string GetExtensionClassName(INamedTypeSymbol symbol) => SpecialCasedHandlerFullName(symbol).Split('.').Last() + "Extensions";

        private static string SpecialCasedHandlerFullName(INamedTypeSymbol symbol)
        {
            var substringIndex = symbol.Name.LastIndexOf("Handler", StringComparison.Ordinal);
            if (substringIndex is -1)
            {
                substringIndex = symbol.Name.LastIndexOf("Params", StringComparison.Ordinal);
            }

            if (substringIndex is -1)
            {
                substringIndex = symbol.Name.LastIndexOf("Arguments", StringComparison.Ordinal);
            }

            if (substringIndex is -1)
            {
                substringIndex = symbol.Name.LastIndexOf("Event", StringComparison.Ordinal);
            }

            if (substringIndex is -1)
            {
                substringIndex = symbol.Name.Length;
            }

            var start = 0;
            if (symbol.Name.StartsWith("I", StringComparison.Ordinal) && char.IsUpper(symbol.Name[1]))
            {
                start = 1;
                substringIndex -= 1;
            }

            return symbol.Name.Substring(start, substringIndex);
        }

        public static string SpecialCasedHandlerName(INamedTypeSymbol symbol)
        {
            var name = SpecialCasedHandlerFullName(symbol);
            return name.Substring(name.LastIndexOf('.') + 1);
        }

        public static SeparatedSyntaxList<TypeParameterConstraintSyntax> HandlerIdentityConstraint { get; } = SeparatedList(
            new TypeParameterConstraintSyntax[]
            {
                ClassOrStructConstraint(SyntaxKind.ClassConstraint)
                   .WithQuestionToken(Token(SyntaxKind.QuestionToken)),
                TypeConstraint(NullableType(IdentifierName("IHandlerIdentity"))),
            }
        );

        public static SyntaxList<TypeParameterConstraintClauseSyntax> HandlerIdentityConstraintClause(
            bool withHandlerIdentity, IdentifierNameSyntax? openGenericType = null
        )
        {
            if (!withHandlerIdentity)
                return SingletonList(
                    TypeParameterConstraintClause(openGenericType ?? IdentifierName("T")).WithConstraints(
                        SingletonSeparatedList<TypeParameterConstraintSyntax>(
                            ClassOrStructConstraint(SyntaxKind.ClassConstraint)
                               .WithQuestionToken(Token(SyntaxKind.QuestionToken))
                        )
                    )
                );
            return SingletonList(
                TypeParameterConstraintClause(openGenericType ?? IdentifierName("T"))
                   .WithConstraints(HandlerIdentityConstraint)
            );
        }

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

    public static class LazyFactory
    {
        public static Lazy<T> Create<T>(Func<T> func) => new(func);
    }

    public static class CommonElements
    {
        public static AccessorListSyntax GetSetAccessor => GetSetAccessorLazy.Value;

        private static readonly Lazy<AccessorListSyntax> GetSetAccessorLazy = LazyFactory.Create(
            () => AccessorList(
                List(
                    new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    }
                )
            )
        );

        private static readonly Lazy<AccessorListSyntax> GetInitAccessorLazy = LazyFactory.Create(
            () => AccessorList(
                List(
                    new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    }
                )
            )
        );

        public static AccessorListSyntax GetInitAccessor => GetInitAccessorLazy.Value;

        private static readonly Lazy<AccessorListSyntax> GetAccessorLazy = LazyFactory.Create(
            () => AccessorList(
                List(
                    new[]
                    {
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    }
                )
            )
        );

        public static AccessorListSyntax GetAccessor => GetAccessorLazy.Value;
    }

    public static class SyntaxExtensions
    {
        public static TypeSyntax EnsureNullable(this TypeSyntax typeSyntax) => typeSyntax is NullableTypeSyntax nts ? nts : NullableType(typeSyntax);
        public static TypeSyntax EnsureNotNullable(this TypeSyntax typeSyntax) => typeSyntax is NullableTypeSyntax nts ? nts.ElementType : typeSyntax;

        public static string? GetSyntaxName(this TypeSyntax typeSyntax)
        {
            return typeSyntax switch
            {
                SimpleNameSyntax sns     => sns.Identifier.Text,
                QualifiedNameSyntax qns  => qns.Right.Identifier.Text,
                NullableTypeSyntax nts   => nts.ElementType.GetSyntaxName() + "?",
                PredefinedTypeSyntax pts => pts.Keyword.Text,
                ArrayTypeSyntax ats      => ats.ElementType.GetSyntaxName() + "[]",
                TupleTypeSyntax tts      => "(" + tts.Elements.Select(z => $"{z.Type.GetSyntaxName()}{z.Identifier.Text}") + ")",
                _                        => null // there might be more but for now... throw new NotSupportedException(typeSyntax.GetType().FullName)
            };
        }

        private static readonly ConcurrentDictionary<string, HashSet<string>> AttributeNames = new();

        private static HashSet<string> GetNames(string attributePrefixes)
        {
            if (!AttributeNames.TryGetValue(attributePrefixes, out var names))
            {
                names = new HashSet<string>(attributePrefixes.Split(',').SelectMany(z => new[] { z, z + "Attribute" }));
                AttributeNames.TryAdd(attributePrefixes, names);
            }

            return names;
        }

        public static bool ContainsAttribute(this AttributeListSyntax list, string attributePrefixes) // string is comma separated
        {
            if (list is { Attributes: { Count: 0 } }) return false;
            var names = GetNames(attributePrefixes);

            foreach (var item in list.Attributes)
            {
                if (item.Name.GetSyntaxName() is { } n && names.Contains(n)) return true;
            }

            return false;
        }

        public static bool ContainsAttribute(this in SyntaxList<AttributeListSyntax> list, string attributePrefixes) // string is comma separated
        {
            if (list is { Count: 0 }) return false;
            var names = GetNames(attributePrefixes);

            foreach (var item in list)
            {
                foreach (var attribute in item.Attributes)
                {
                    if (attribute.Name.GetSyntaxName() is { } n && names.Contains(n)) return true;
                }
            }

            return false;
        }

        public static AttributeSyntax? GetAttribute(this AttributeListSyntax list, string attributePrefixes) // string is comma separated
        {
            if (list is { Attributes: { Count: 0 } }) return null;
            var names = GetNames(attributePrefixes);

            foreach (var item in list.Attributes)
            {
                if (item.Name.GetSyntaxName() is { } n && names.Contains(n)) return item;
            }

            return null;
        }

        public static AttributeSyntax? GetAttribute(this in SyntaxList<AttributeListSyntax> list, string attributePrefixes) // string is comma separated
        {
            if (list is { Count: 0 }) return null;
            var names = GetNames(attributePrefixes);

            foreach (var item in list)
            {
                foreach (var attribute in item.Attributes)
                {
                    if (attribute.Name.GetSyntaxName() is { } n && names.Contains(n)) return attribute;
                }
            }

            return null;
        }

        public static bool IsAttribute(this AttributeSyntax attributeSyntax, string attributePrefixes) // string is comma separated
        {
            var names = GetNames(attributePrefixes);
            return attributeSyntax.Name.GetSyntaxName() is { } n && names.Contains(n);
        }

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
                syntax = syntax.WithExpressionBody(
                    syntax.ExpressionBody.WithExpression(syntax.ExpressionBody.Expression.InsideNullableSwitchExpression(identifierNameSyntax))
                );
            }

            return syntax
                  .WithParameterList(
                       ParameterList(
                           SeparatedList(syntax.ParameterList.Parameters.Select(parameter => parameter.WithType(parameter.Type?.EnsureNullable())).ToArray())
                       )
                   )
                  .AddAttributeLists(
                       AttributeList(
                               SingletonSeparatedList(
                                   Attribute(
                                           QualifiedName(
                                               QualifiedName(
                                                   QualifiedName(IdentifierName("System"), IdentifierName("Diagnostics")), IdentifierName("CodeAnalysis")
                                               ),
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

        public static ConversionOperatorDeclarationSyntax MakeMethodNullable(
            this ConversionOperatorDeclarationSyntax syntax, IdentifierNameSyntax identifierNameSyntax
        )
        {
            return ( MakeMethodNullable(syntax as BaseMethodDeclarationSyntax, identifierNameSyntax) as ConversionOperatorDeclarationSyntax )!;
        }

        public static SwitchExpressionSyntax InsideNullableSwitchExpression(this ExpressionSyntax creationExpression, IdentifierNameSyntax name)
        {
            return SwitchExpression(name)
               .WithArms(
                    SeparatedList(
                        new[]
                        {
                            SwitchExpressionArm(
                                UnaryPattern(ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                                creationExpression
                            ),
                            SwitchExpressionArm(DiscardPattern(), LiteralExpression(SyntaxKind.NullLiteralExpression))
                        }
                    )
                );
        }

        public static TypeDeclarationSyntax WithHandlerIdentityConstraint(
            this TypeDeclarationSyntax syntax, bool includeHandlerIdentity, IdentifierNameSyntax? openGenericType = null
        )
        {
            openGenericType ??= IdentifierName("T");
            return syntax
                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(openGenericType.Identifier.Text))))
                  .WithConstraintClauses(Helpers.HandlerIdentityConstraintClause(includeHandlerIdentity, openGenericType));
        }

        public static MethodDeclarationSyntax WithHandlerIdentityConstraint(
            this MethodDeclarationSyntax syntax, bool includeHandlerIdentity, IdentifierNameSyntax? openGenericType = null
        )
        {
            openGenericType ??= IdentifierName("T");
            return syntax
                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(openGenericType.Identifier.Text))))
                  .WithConstraintClauses(Helpers.HandlerIdentityConstraintClause(includeHandlerIdentity, openGenericType));
        }
    }
}

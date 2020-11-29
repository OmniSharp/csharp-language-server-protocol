using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class TypedDelegatingHandlerStrategy : IExtensionMethodGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(GeneratorData item)
        {
            if (item is not RequestItem requestItem) yield break;
            if (requestItem is not { LspAttributes: { Resolver: { } } }) yield break;
            var resolverData = GeneratorData.CreateForResolver(item);
            if (resolverData is not { LspAttributes: { GenerateTypedData: true } }) yield break;

            yield return CreateDelegatingHandler(requestItem, resolverData);

            if (requestItem is { PartialItem: { } } or { PartialItems: { } })
            {
                yield return CreateDelegatingPartialHandler(requestItem, resolverData);
            }
        }

        private TypeDeclarationSyntax CreateDelegatingHandler(RequestItem item, RequestItem resolver)
        {
            TypeSyntax requestType = item.Request.Syntax;
            TypeSyntax resolveType = GenericName(Identifier(resolver.Request.Symbol.Name))
               .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("T")
                        )
                    )
                );
            TypeSyntax responseType = GenericName(Identifier(item.Response.Symbol.Name))
               .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));

            // Special case... because the spec is awesome
            if (item.Response.Symbol.Name == "CommandOrCodeActionContainer")
            {
                responseType = GenericName(Identifier("CodeActionContainer"))
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
            }

            responseType = item.Response.Syntax is NullableTypeSyntax ? NullableType(responseType) : responseType;

            var handler = ClassDeclaration($"Delegating{item.JsonRpcAttributes.HandlerName}Handler")
                         .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.SealedKeyword)))
                         .WithHandlerIdentityConstraint(true)
                         .WithBaseList(
                              BaseList(
                                  SingletonSeparatedList<BaseTypeSyntax>(
                                      SimpleBaseType(
                                          GenericName(Identifier($"{item.JsonRpcAttributes.HandlerName}HandlerBase"))
                                             .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                                      )
                                  )
                              )
                          );

            var constructorParams = ParameterList();

            if (item.RegistrationOptions is { })
            {
                if (item.Capability is { })
                {
                    var type = GenericName(Identifier("RegistrationOptionsDelegate"))
                       .WithTypeArgumentList(
                            TypeArgumentList(
                                SeparatedList(
                                    new[] {
                                        item.RegistrationOptions.Syntax,
                                        item.Capability.Syntax
                                    }
                                )
                            )
                        );

                    handler = handler.AddMembers(
                        FieldDeclaration(VariableDeclaration(type).WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_registrationOptionsFactory")))))
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                        MethodDeclaration(item.RegistrationOptions.Syntax, Identifier("CreateRegistrationOptions"))
                           .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.OverrideKeyword)))
                           .WithParameterList(
                                ParameterList(
                                    SeparatedList(
                                        new[] {
                                            Parameter(Identifier("capability")).WithType(item.Capability.Syntax),
                                            Parameter(Identifier("clientCapabilities")).WithType(IdentifierName("ClientCapabilities"))
                                        }
                                    )
                                )
                            )
                           .WithExpressionBody(
                                ArrowExpressionClause(
                                    InvocationExpression(IdentifierName("_registrationOptionsFactory"))
                                       .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList(
                                                    new[] {
                                                        Argument(IdentifierName("capability")),
                                                        Argument(IdentifierName("clientCapabilities"))
                                                    }
                                                )
                                            )
                                        )
                                )
                            )
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    );
                    constructorParams = constructorParams.AddParameters(Parameter(Identifier("registrationOptionsFactory")).WithType(type));
                }
                else
                {
                    var type = GenericName(Identifier("RegistrationOptionsDelegate"))
                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(item.RegistrationOptions.Syntax)));

                    handler = handler.AddMembers(
                        FieldDeclaration(VariableDeclaration(type).WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_registrationOptionsFactory")))))
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                        MethodDeclaration(item.RegistrationOptions.Syntax, Identifier("CreateRegistrationOptions"))
                           .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.OverrideKeyword)))
                           .WithExpressionBody(
                                ArrowExpressionClause(
                                    InvocationExpression(IdentifierName("_registrationOptionsFactory"))
                                       .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList(
                                                    new[] {
                                                        Argument(IdentifierName("capability")),
                                                        Argument(IdentifierName("clientCapabilities"))
                                                    }
                                                )
                                            )
                                        )
                                )
                            )
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    );
                    constructorParams = constructorParams.AddParameters(
                        Parameter(Identifier("registrationOptionsFactory"))
                           .WithType(type)
                    );
                }
            }
            else
            {
                // set capability?
            }

            if (item.Capability is { })
            {
                handler = handler.AddMembers(
                    FieldDeclaration(
                            VariableDeclaration(DelegateHelpers.CreateAsyncFunc(responseType, true, requestType, item.Capability.Syntax))
                               .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handleParams"))))
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                    FieldDeclaration(
                            VariableDeclaration(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType, item.Capability.Syntax))
                               .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handleResolve"))))
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
                );
                constructorParams = constructorParams.AddParameters(
                    Parameter(Identifier("handleParams"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(responseType, true, requestType, item.Capability.Syntax)),
                    Parameter(Identifier("handleResolve"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType, item.Capability.Syntax))
                );
            }
            else
            {
                handler = handler.AddMembers(
                        FieldDeclaration(
                                VariableDeclaration(DelegateHelpers.CreateAsyncFunc(responseType, true, requestType))
                                   .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handleParams"))))
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                        FieldDeclaration(
                                VariableDeclaration(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType))
                                   .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handleResolve"))))
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
                    )
                    ;
                constructorParams = constructorParams.AddParameters(
                    Parameter(Identifier("handleParams"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(responseType, true, requestType)),
                    Parameter(Identifier("handleResolve"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType))
                );
            }

            handler = handler.AddMembers(
                ConstructorDeclaration(Identifier($"Delegating{item.JsonRpcAttributes.HandlerName}Handler"))
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                   .WithParameterList(constructorParams)
                   .WithInitializer(
                        ConstructorInitializer(
                            SyntaxKind.BaseConstructorInitializer,
                            ArgumentList()
                        )
                    )
                   .WithBody(
                        Block(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_registrationOptionsFactory"),
                                    IdentifierName("registrationOptionsFactory")
                                )
                            ),
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_handleParams"),
                                    IdentifierName("handleParams")
                                )
                            ),
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_handleResolve"),
                                    IdentifierName("handleResolve")
                                )
                            )
                        )
                    ),
                MethodDeclaration(
                        GenericName(Identifier("Task"))
                           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(responseType))),
                        Identifier("HandleParams")
                    )
                   .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                   .WithParameterList(
                        ParameterList(
                            SeparatedList(
                                new[] {
                                    Parameter(Identifier("request")).WithType(requestType),
                                    Parameter(Identifier("cancellationToken")).WithType(IdentifierName("CancellationToken"))
                                }
                            )
                        )
                    )
                   .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(IdentifierName("_handleParams"))
                               .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList(
                                            item.Capability is { }
                                                ? new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("Capability")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                                : new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                        )
                                    )
                                )
                        )
                    )
                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            );

            handler = handler.AddMembers(
                MethodDeclaration(
                        GenericName(Identifier("Task"))
                           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType))),
                        Identifier("HandleResolve")
                    )
                   .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                   .WithParameterList(
                        ParameterList(
                            SeparatedList(
                                new[] {
                                    Parameter(Identifier("request")).WithType(resolveType),
                                    Parameter(Identifier("cancellationToken")).WithType(IdentifierName("CancellationToken"))
                                }
                            )
                        )
                    )
                   .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(IdentifierName("_handleResolve"))
                               .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList(
                                            item.Capability is { }
                                                ? new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("Capability")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                                : new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                        )
                                    )
                                )
                        )
                    )
                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            );


            return handler;
        }

        private TypeDeclarationSyntax CreateDelegatingPartialHandler(RequestItem item, RequestItem resolver)
        {
            TypeSyntax requestType = item.Request.Syntax;
            TypeSyntax resolveType = GenericName(Identifier(resolver.Request.Symbol.Name))
               .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("T")
                        )
                    )
                );
            TypeSyntax responseType = GenericName(Identifier(item.Response.Symbol.Name))
               .WithTypeArgumentList(
                    TypeArgumentList(
                        SingletonSeparatedList<TypeSyntax>(
                            IdentifierName("T")
                        )
                    )
                );

            // Special case... because the spec is awesome
            if (item.Response.Symbol.Name == "CommandOrCodeActionContainer")
            {
                responseType = GenericName(Identifier("CodeActionContainer"))
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
            }

            responseType = item.Response.Syntax is NullableTypeSyntax ? NullableType(responseType) : responseType;

            TypeSyntax observerType = null!;
            if (item is { PartialItem: { } partialItem })
            {
                observerType = GenericName("IObserver").WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)));
            }

            if (item is { PartialItems: { } partialItems })
            {
                observerType = GenericName("IObserver")
                   .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                GenericName("IEnumerable")
                                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType)))
                            )
                        )
                    );
            }


            var handler = ClassDeclaration($"Delegating{item.JsonRpcAttributes.HandlerName}PartialHandler")
                         .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.SealedKeyword)))
                         .WithHandlerIdentityConstraint(true)
                         .WithBaseList(
                              BaseList(
                                  SingletonSeparatedList<BaseTypeSyntax>(
                                      SimpleBaseType(
                                          GenericName(Identifier($"{item.JsonRpcAttributes.HandlerName}PartialHandlerBase"))
                                             .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                                      )
                                  )
                              )
                          );

            var constructorParams = ParameterList(SingletonSeparatedList(Parameter(Identifier("progressManager")).WithType(IdentifierName("IProgressManager"))));

            if (item.RegistrationOptions is { })
            {
                if (item.Capability is { })
                {
                    var type = GenericName(Identifier("RegistrationOptionsDelegate"))
                       .WithTypeArgumentList(
                            TypeArgumentList(
                                SeparatedList(
                                    new[] {
                                        item.RegistrationOptions.Syntax,
                                        item.Capability.Syntax
                                    }
                                )
                            )
                        );

                    handler = handler.AddMembers(
                        FieldDeclaration(VariableDeclaration(type).WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_registrationOptionsFactory")))))
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                        MethodDeclaration(item.RegistrationOptions.Syntax, Identifier("CreateRegistrationOptions"))
                           .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.OverrideKeyword)))
                           .WithParameterList(
                                ParameterList(
                                    SeparatedList(
                                        new[] {
                                            Parameter(Identifier("capability")).WithType(item.Capability.Syntax),
                                            Parameter(Identifier("clientCapabilities")).WithType(IdentifierName("ClientCapabilities"))
                                        }
                                    )
                                )
                            )
                           .WithExpressionBody(
                                ArrowExpressionClause(
                                    InvocationExpression(IdentifierName("_registrationOptionsFactory"))
                                       .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList(
                                                    SeparatedList(
                                                        new[] {
                                                            Argument(IdentifierName("capability")),
                                                            Argument(IdentifierName("clientCapabilities"))
                                                        }
                                                    )
                                                )
                                            )
                                        )
                                )
                            )
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    );
                    constructorParams = constructorParams.AddParameters(Parameter(Identifier("registrationOptionsFactory")).WithType(type));
                }
                else
                {
                    var type = GenericName(Identifier("RegistrationOptionsDelegate"))
                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(item.RegistrationOptions.Syntax)));

                    handler = handler.AddMembers(
                        FieldDeclaration(VariableDeclaration(type).WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_registrationOptionsFactory")))))
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                        MethodDeclaration(item.RegistrationOptions.Syntax, Identifier("CreateRegistrationOptions"))
                           .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.OverrideKeyword)))
                           .WithExpressionBody(
                                ArrowExpressionClause(
                                    InvocationExpression(IdentifierName("_registrationOptionsFactory"))
                                       .WithArgumentList(
                                            ArgumentList(
                                                SeparatedList(
                                                    new[] {
                                                        Argument(IdentifierName("capability")),
                                                        Argument(IdentifierName("clientCapabilities"))
                                                    }
                                                )
                                            )
                                        )
                                )
                            )
                           .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    );
                    constructorParams = constructorParams.AddParameters(
                        Parameter(Identifier("registrationOptionsFactory"))
                           .WithType(type)
                    );
                }
            }
            else
            {
                // set capability?
            }

            if (item.Capability is { })
            {
                handler = handler.AddMembers(
                    FieldDeclaration(
                            VariableDeclaration(DelegateHelpers.CreateAction(true, requestType, observerType, item.Capability.Syntax))
                               .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handle"))))
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                    FieldDeclaration(
                            VariableDeclaration(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType, item.Capability.Syntax))
                               .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handleResolve"))))
                        )
                       .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
                );
                constructorParams = constructorParams.AddParameters(
                    Parameter(Identifier("handle"))
                       .WithType(DelegateHelpers.CreateAction(true, requestType, observerType, item.Capability.Syntax)),
                    Parameter(Identifier("handleResolve"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType, item.Capability.Syntax))
                );
            }
            else
            {
                handler = handler.AddMembers(
                        FieldDeclaration(
                                VariableDeclaration(DelegateHelpers.CreateAction(true, requestType, observerType))
                                   .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handle"))))
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword))),
                        FieldDeclaration(
                                VariableDeclaration(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType))
                                   .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("_handleResolve"))))
                            )
                           .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)))
                    )
                    ;
                constructorParams = constructorParams.AddParameters(
                    Parameter(Identifier("handle"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(responseType, true, requestType)),
                    Parameter(Identifier("handleResolve"))
                       .WithType(DelegateHelpers.CreateAsyncFunc(resolveType, true, resolveType))
                );
            }

            handler = handler.AddMembers(
                ConstructorDeclaration(Identifier($"Delegating{item.JsonRpcAttributes.HandlerName}PartialHandler"))
                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                   .WithParameterList(constructorParams)
                   .WithInitializer(
                        ConstructorInitializer(
                            SyntaxKind.BaseConstructorInitializer,
                            ArgumentList(SingletonSeparatedList(Argument(IdentifierName("progressManager"))))
                        )
                    )
                   .WithBody(
                        Block(
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_registrationOptionsFactory"),
                                    IdentifierName("registrationOptionsFactory")
                                )
                            ),
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_handle"),
                                    IdentifierName("handle")
                                )
                            ),
                            ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_handleResolve"),
                                    IdentifierName("handleResolve")
                                )
                            )
                        )
                    ),
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Handle"))
                   .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                   .WithParameterList(
                        ParameterList(
                            SeparatedList(
                                new[] {
                                    Parameter(Identifier("request")).WithType(requestType),
                                    Parameter(Identifier("results")).WithType(observerType),
                                    Parameter(Identifier("cancellationToken")).WithType(IdentifierName("CancellationToken"))
                                }
                            )
                        )
                    )
                   .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(IdentifierName("_handle"))
                               .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList(
                                            item.Capability is { }
                                                ? new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("results")),
                                                    Argument(IdentifierName("Capability")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                                : new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("results")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                        )
                                    )
                                )
                        )
                    )
                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            );
            handler = handler.AddMembers(
                MethodDeclaration(
                        GenericName(Identifier("Task"))
                           .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(resolveType))),
                        Identifier("HandleResolve")
                    )
                   .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                   .WithParameterList(
                        ParameterList(
                            SeparatedList(
                                new[] {
                                    Parameter(Identifier("request")).WithType(resolveType),
                                    Parameter(Identifier("cancellationToken")).WithType(IdentifierName("CancellationToken"))
                                }
                            )
                        )
                    )
                   .WithExpressionBody(
                        ArrowExpressionClause(
                            InvocationExpression(IdentifierName("_handleResolve"))
                               .WithArgumentList(
                                    ArgumentList(
                                        SeparatedList(
                                            item.Capability is { }
                                                ? new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("Capability")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                                : new[] {
                                                    Argument(IdentifierName("request")),
                                                    Argument(IdentifierName("cancellationToken"))
                                                }
                                        )
                                    )
                                )
                        )
                    )
                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
            );


            return handler;
        }
    }
}

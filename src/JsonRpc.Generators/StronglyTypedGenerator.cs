using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class StronglyTypedGenerator : CachedSourceGenerator<StronglyTypedGenerator.SyntaxReceiver, TypeDeclarationSyntax>
    {
        private static string[] RequiredUsings = new[] {
            "System.Collections.Generic",
            "System.Collections.ObjectModel",
            "System.Collections.Immutable",
            "System.Linq",
        };

        protected override void Execute(
            GeneratorExecutionContext context, SyntaxReceiver syntaxReceiver, AddCacheSource<TypeDeclarationSyntax> addCacheSource,
            ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic
        )
        {
            var generateTypedDataAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Generation.GenerateTypedDataAttribute");
            var generateContainerAttributeSymbol = context.Compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Generation.GenerateContainerAttribute");

            foreach (var classToContain in syntaxReceiver.CreateContainers)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classToContain.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(classToContain);
                var attribute = typeSymbol?.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateContainerAttributeSymbol));
                if (typeSymbol == null || attribute is null) continue;

                var containerName = attribute is { ConstructorArguments: { Length: > 0 } arguments } ? arguments[0].Value as string : null;

                var container = CreateContainerClass(classToContain, containerName)
                   .AddAttributeLists(
                        AttributeList(
                            SeparatedList(
                                new[] {
                                    Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                                    Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
                                }
                            )
                        )
                    );

                var cu = CompilationUnit()
                        .WithUsings(classToContain.SyntaxTree.GetCompilationUnitRoot().Usings)
                        .AddMembers(
                             NamespaceDeclaration(ParseName(typeSymbol.ContainingNamespace.ToDisplayString()))
                                .AddMembers(container)
                                .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                                .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                         )
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool))
                        .WithTrailingTrivia(CarriageReturnLineFeed);

                foreach (var ns in RequiredUsings)
                {
                    if (cu.Usings.All(z => z.Name.ToFullString() != ns))
                    {
                        cu = cu.AddUsings(UsingDirective(ParseName(ns)));
                    }
                }

                addCacheSource(
                    $"{containerName ?? classToContain.Identifier.Text + "Container"}.cs",
                    classToContain,
                    cu.NormalizeWhitespace().GetText(Encoding.UTF8)
                );
            }

            foreach (var canBeResolved in syntaxReceiver.CanBeResolved)
            {
                var dataInterfaceName = IdentifierName("ICanBeResolved");
                CreateTypedClass(
                    context, canBeResolved, dataInterfaceName, generateTypedDataAttributeSymbol, generateContainerAttributeSymbol, true, addCacheSource, cacheDiagnostic
                );
            }

            foreach (var canBeResolved in syntaxReceiver.CanHaveData)
            {
                var dataInterfaceName = IdentifierName("ICanHaveData");
                CreateTypedClass(
                    context, canBeResolved, dataInterfaceName, generateTypedDataAttributeSymbol, generateContainerAttributeSymbol, false, addCacheSource, cacheDiagnostic
                );
            }

            static void CreateTypedClass(
                GeneratorExecutionContext context,
                TypeDeclarationSyntax candidate,
                IdentifierNameSyntax dataInterfaceName,
                INamedTypeSymbol? generateTypedDataAttributeSymbol,
                INamedTypeSymbol? generateContainerAttributeSymbol,
                bool includeHandlerIdentity,
                AddCacheSource<TypeDeclarationSyntax> cacheItem,
                ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic
            )
            {
                var semanticModel = context.Compilation.GetSemanticModel(candidate.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(candidate);
                var attribute = typeSymbol?.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateTypedDataAttributeSymbol));
                if (typeSymbol == null || attribute is null) return;
                var container = typeSymbol.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateContainerAttributeSymbol));

                if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    cacheDiagnostic(candidate, static c => Diagnostic.Create(GeneratorDiagnostics.MustBePartial, c.Identifier.GetLocation(), c.Identifier.Text));
                    return;
                }

                var property = candidate.Members.OfType<PropertyDeclarationSyntax>().Single(z => z.Identifier.Text == "Data");
                var partialClass = candidate
                                  .WithAttributeLists(List<AttributeListSyntax>())
                                  .WithBaseList(null)
                                  .WithMembers(List<MemberDeclarationSyntax>())
                                  .AddMembers(
                                       GetWithDataMethod(candidate, HandlerIdentityConstraintClause(includeHandlerIdentity, IdentifierName("TData"))),
                                       GetFromMethod(candidate, includeHandlerIdentity)
                                   );

                var compilationMembers = new List<MemberDeclarationSyntax>();


                var convertFromOperator = GetConvertFromOperator(candidate);
                var convertToOperator = GetConvertToOperator(candidate);
                // remove the data property
                var typedClass = candidate
                                .WithHandlerIdentityConstraint(includeHandlerIdentity)
                                .WithMembers(candidate.Members.Replace(property, GetPropertyImpl(property).WithType(IdentifierName("T"))))
                                .AddMembers(
                                     GetWithDataMethod(candidate, HandlerIdentityConstraintClause(includeHandlerIdentity, IdentifierName("TData"))),
                                     GetExplicitProperty(property, dataInterfaceName),
                                     GetJDataProperty(),
                                     convertFromOperator,
                                     convertToOperator,
                                     GetGenericFromMethod(candidate)
                                 )
                                .WithAttributeLists(
                                     List(
                                         candidate.AttributeLists
                                                  .Where(z => !z.ContainsAttribute("Method") && !z.ContainsAttribute("GenerateTypedData"))
                                     )
                                 )
                                .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(dataInterfaceName))))
                                .AddAttributeLists(
                                     AttributeList(
                                         SeparatedList(
                                             new[] {
                                                 Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                                                 Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
                                             }
                                         )
                                     )
                                 )
//                                .WithLeadingTrivia(candidate.GetLeadingTrivia().Where(z => !z.ToString().Contains("#nullable")))
//                                .WithTrailingTrivia(candidate.GetTrailingTrivia().Where(z => !z.ToString().Contains("#nullable")))
                    ;

                if (container is { })
                {
                    var containerName = container is { ConstructorArguments: { Length: > 0 } arguments } ? arguments[0].Value as string : null;
                    var typedContainer = CreateContainerClass(typedClass, containerName)
                       .WithHandlerIdentityConstraint(includeHandlerIdentity);

                    var typedArgumentList = TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T")));
                    typedContainer = typedContainer
                       .AddMembers(
                            ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), IdentifierName(typedContainer.Identifier))
                               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                               .WithParameterList(
                                    ParameterList(
                                        SingletonSeparatedList(
                                            Parameter(Identifier("container"))
                                               .WithType(GenericName(typedContainer.Identifier).WithTypeArgumentList(typedArgumentList))
                                        )
                                    )
                                )
                               .WithExpressionBody(
                                    ArrowExpressionClause(
                                        ObjectCreationExpression(IdentifierName(typedContainer.Identifier))
                                           .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(
                                                            InvocationExpression(
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("container"),
                                                                        IdentifierName("Select")
                                                                    )
                                                                )
                                                               .WithArgumentList(
                                                                    ArgumentList(
                                                                        SingletonSeparatedList(
                                                                            Argument(
                                                                                SimpleLambdaExpression(Parameter(Identifier("value")))
                                                                                   .WithExpressionBody(
                                                                                        CastExpression(
                                                                                            IdentifierName(candidate.Identifier),
                                                                                            IdentifierName("value")
                                                                                        )
                                                                                    )
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                        )
                                                    )
                                                )
                                            )
                                    )
                                )
                               .MakeMethodNullable(IdentifierName("container"))
                               .WithSemicolonToken(
                                    Token(SyntaxKind.SemicolonToken)
                                )
                        );

                    compilationMembers.Add(typedContainer);
                }

                var cu = CompilationUnit()
                        .WithUsings(candidate.SyntaxTree.GetCompilationUnitRoot().Usings)
                        .AddMembers(
                             NamespaceDeclaration(ParseName(typeSymbol.ContainingNamespace.ToDisplayString()))
                                .AddMembers(partialClass, typedClass)
                                .AddMembers(compilationMembers.ToArray())
                                .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                                .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                         )
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool))
                        .WithTrailingTrivia(CarriageReturnLineFeed);
                foreach (var ns in RequiredUsings)
                {
                    if (cu.Usings.All(z => z.Name.ToFullString() != ns))
                    {
                        cu = cu.AddUsings(UsingDirective(ParseName(ns)));
                    }
                }

                cacheItem(
                    $"{Path.GetFileNameWithoutExtension(candidate.SyntaxTree.FilePath)}_{candidate.Identifier.Text}Typed.cs",
                    candidate,
                    cu.NormalizeWhitespace().GetText(Encoding.UTF8)
                );
            }
        }

        private static MethodDeclarationSyntax GetWithDataMethod(TypeDeclarationSyntax syntax, SyntaxList<TypeParameterConstraintClauseSyntax> constraintSyntax)
        {
            return MethodDeclaration(
                       GenericName(Identifier(syntax.Identifier.Text))
                          .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("TData")))),
                       Identifier("WithData")
                   )
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("TData")))))
                  .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("data")).WithType(IdentifierName("TData")))))
                  .WithConstraintClauses(List(constraintSyntax))
                  .WithBody(
                       Block(
                           SingletonList<StatementSyntax>(
                               ReturnStatement(
                                   ObjectCreationExpression(
                                           GenericName(Identifier(syntax.Identifier.Text))
                                              .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("TData"))))
                                       )
                                      .WithInitializer(
                                           InitializerExpression(
                                               SyntaxKind.ObjectInitializerExpression,
                                               SeparatedList(
                                                   GetMapping(syntax, null).Concat(
                                                       new ExpressionSyntax[] {
                                                           AssignmentExpression(
                                                               SyntaxKind.SimpleAssignmentExpression,
                                                               IdentifierName("Data"),
                                                               IdentifierName("data")
                                                           )
                                                       }
                                                   )
                                               )
                                           )
                                       )
                               )
                           )
                       )
                   );
        }

        private static MethodDeclarationSyntax GetGenericFromMethod(TypeDeclarationSyntax syntax)
        {
            return MethodDeclaration(
                       GenericName(Identifier(syntax.Identifier.Text))
                          .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T")))), Identifier("From")
                   )
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                  .WithParameterList(
                       ParameterList(
                           SingletonSeparatedList(
                               Parameter(Identifier("item")).WithType(IdentifierName(syntax.Identifier.Text))
                           )
                       )
                   )
                  .WithExpressionBody(ArrowExpressionClause(IdentifierName("item")))
                  .MakeMethodNullable(IdentifierName("item"))
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static MethodDeclarationSyntax GetFromMethod(TypeDeclarationSyntax syntax, bool includeHandlerIdentity)
        {
            return MethodDeclaration(IdentifierName(syntax.Identifier.Text), Identifier("From"))
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                  .WithHandlerIdentityConstraint(includeHandlerIdentity)
                  .WithParameterList(
                       ParameterList(
                           SingletonSeparatedList(
                               Parameter(Identifier("item"))
                                  .WithType(
                                       GenericName(Identifier(syntax.Identifier.Text))
                                          .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                                   )
                           )
                       )
                   )
                  .WithExpressionBody(ArrowExpressionClause(IdentifierName("item")))
                  .MakeMethodNullable(IdentifierName("item"))
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static IEnumerable<ExpressionSyntax> GetMapping(TypeDeclarationSyntax syntax, IdentifierNameSyntax? paramName)
        {
            return syntax.Members.OfType<PropertyDeclarationSyntax>()
                         .Where(z => z.AccessorList?.Accessors.Any(a => a.Keyword.Kind() == SyntaxKind.SetKeyword || a.Keyword.Kind() == SyntaxKind.InitKeyword) == true)
                         .Where(z => z.Identifier.Text != "Data")
                         .Select(
                              property => AssignmentExpression(
                                  SyntaxKind.SimpleAssignmentExpression,
                                  IdentifierName(property.Identifier.Text),
                                  paramName == null
                                      ? IdentifierName(property.Identifier.Text)
                                      : MemberAccessExpression(
                                          SyntaxKind.SimpleMemberAccessExpression,
                                          paramName,
                                          IdentifierName(property.Identifier.Text)
                                      )
                              )
                          );
        }

        private static ConversionOperatorDeclarationSyntax GetConvertToOperator(TypeDeclarationSyntax syntax)
        {
            var name = IdentifierName(syntax.Identifier.Text);
            var identifier = Identifier(syntax.Identifier.Text);
            var paramName = IdentifierName("value");
            return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), name)
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                  .WithParameterList(
                       ParameterList(
                           SingletonSeparatedList(
                               Parameter(
                                       Identifier("value")
                                   )
                                  .WithType(
                                       GenericName(identifier)
                                          .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                                   )
                           )
                       )
                   )
                  .WithExpressionBody(
                       ArrowExpressionClause(
                           ObjectCreationExpression(name)
                              .WithInitializer(
                                   InitializerExpression(
                                       SyntaxKind.ObjectInitializerExpression,
                                       SeparatedList(
                                           GetMapping(syntax, paramName)
                                              .Concat(
                                                   new[] {
                                                       AssignmentExpression(
                                                           SyntaxKind.SimpleAssignmentExpression,
                                                           IdentifierName("Data"),
                                                           MemberAccessExpression(
                                                               SyntaxKind.SimpleMemberAccessExpression,
                                                               paramName,
                                                               IdentifierName("JData")
                                                           )
                                                       )
                                                   }
                                               )
                                       )
                                   )
                               )
                       )
                   )
                  .MakeMethodNullable(paramName)
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static ConversionOperatorDeclarationSyntax GetConvertFromOperator(TypeDeclarationSyntax syntax)
        {
            var name = IdentifierName(syntax.Identifier.Text);
            var identifier = Identifier(syntax.Identifier.Text);
            var paramName = IdentifierName("value");
            var paramIdentifier = Identifier("value");
            return ConversionOperatorDeclaration(
                       Token(SyntaxKind.ImplicitKeyword),
                       GenericName(identifier).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                   )
                  .WithModifiers(TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword) }))
                  .WithParameterList(
                       ParameterList(
                           SingletonSeparatedList(Parameter(paramIdentifier).WithType(name))
                       )
                   )
                  .WithExpressionBody(
                       ArrowExpressionClause(
                           ObjectCreationExpression(
                                   GenericName(identifier)
                                      .WithTypeArgumentList(
                                           TypeArgumentList(
                                               SingletonSeparatedList<TypeSyntax>(
                                                   IdentifierName("T")
                                               )
                                           )
                                       )
                               )
                              .WithInitializer(
                                   InitializerExpression(
                                       SyntaxKind.ObjectInitializerExpression,
                                       SeparatedList(
                                           GetMapping(syntax, paramName)
                                              .Concat(
                                                   new[] {
                                                       AssignmentExpression(
                                                           SyntaxKind.SimpleAssignmentExpression,
                                                           IdentifierName("JData"),
                                                           MemberAccessExpression(
                                                               SyntaxKind.SimpleMemberAccessExpression,
                                                               paramName,
                                                               IdentifierName("Data")
                                                           )
                                                       )
                                                   }
                                               )
                                       )
                                   )
                               )
                       )
                   )
                  .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

        private static PropertyDeclarationSyntax GetExplicitProperty(PropertyDeclarationSyntax syntax, IdentifierNameSyntax dataInterfaceName)
        {
            return syntax
                  .WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifier(dataInterfaceName))
                  .WithModifiers(TokenList())
                  .WithAttributeLists(List(SeparatedList<AttributeListSyntax>()))
                  .WithAccessorList(CommonElements.GetInitAccessor);
        }

        private static PropertyDeclarationSyntax GetJDataProperty()
        {
            return PropertyDeclaration(NullableType(IdentifierName("JToken")), Identifier("JData"))
                  .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                  .WithAccessorList(
                       AccessorList(
                           List(
                               new[] {
                                   AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                      .WithExpressionBody(
                                           ArrowExpressionClause(
                                               InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("GetRawData")))
                                           )
                                       )
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                   AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                      .WithExpressionBody(
                                           ArrowExpressionClause(
                                               InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("SetRawData")))
                                                  .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value")))))
                                           )
                                       )
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                               }
                           )
                       )
                   );
        }

        static PropertyDeclarationSyntax GetPropertyImpl(PropertyDeclarationSyntax syntax)
        {
            return syntax.WithAccessorList(
                AccessorList(
                    List(
                        new[] {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                               .WithExpressionBody(
                                    ArrowExpressionClause(
                                        PostfixUnaryExpression(
                                            SyntaxKind.SuppressNullableWarningExpression,
                                            InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), GenericName("GetRawData")
                                                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                                                )
                                            )
                                        )
                                    )
                                ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(
                                    SyntaxKind.InitAccessorDeclaration
                                )
                               .WithExpressionBody(
                                    ArrowExpressionClause(
                                        InvocationExpression(
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    GenericName("SetRawData")
                                                       .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))))
                                                )
                                            )
                                           .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value")))))
                                    )
                                ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        }
                    )
                )
            );
        }

        private static TypeDeclarationSyntax CreateContainerClass(TypeDeclarationSyntax syntax, string? name)
        {
            TypeSyntax typeName = IdentifierName(syntax.Identifier.Text);
            var classIdentifier = Identifier(name ?? $"{syntax.Identifier.Text}Container");
            TypeSyntax className = IdentifierName(name ?? $"{syntax.Identifier.Text}Container");
            if (syntax.Arity > 0)
            {
                typeName = GenericName(syntax.Identifier.Text)
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
                className = GenericName(name ?? $"{syntax.Identifier.Text}Container")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
            }

            var typeArgumentList = TypeArgumentList(SingletonSeparatedList(typeName));

            var modifiers = syntax.Modifiers;
            if (!modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
            {
                modifiers = modifiers.Add(Token(SyntaxKind.PartialKeyword));
            }

            return ClassDeclaration(classIdentifier)
                  .WithModifiers(modifiers)
                  .WithBaseList(
                       BaseList(
                           SingletonSeparatedList<BaseTypeSyntax>(
                               SimpleBaseType(
                                   GenericName(Identifier("ContainerBase"))
                                      .WithTypeArgumentList(typeArgumentList)
                               )
                           )
                       )
                   )
                  .WithMembers(
                       List(
                           new MemberDeclarationSyntax[] {
                               ConstructorDeclaration(classIdentifier)
                                  .WithModifiers(
                                       TokenList(
                                           Token(SyntaxKind.PublicKeyword)
                                       )
                                   )
                                  .WithInitializer(
                                       ConstructorInitializer(
                                           SyntaxKind.ThisConstructorInitializer,
                                           ArgumentList(
                                               SingletonSeparatedList(
                                                   Argument(
                                                       InvocationExpression(
                                                           MemberAccessExpression(
                                                               SyntaxKind.SimpleMemberAccessExpression,
                                                               IdentifierName("Enumerable"),
                                                               GenericName(Identifier("Empty"))
                                                                  .WithTypeArgumentList(typeArgumentList)
                                                           )
                                                       )
                                                   )
                                               )
                                           )
                                       )
                                   )
                                  .WithBody(
                                       Block()
                                   ),
                               ConstructorDeclaration(classIdentifier)
                                  .WithModifiers(
                                       TokenList(
                                           Token(SyntaxKind.PublicKeyword)
                                       )
                                   )
                                  .WithParameterList(
                                       ParameterList(
                                           SingletonSeparatedList(
                                               Parameter(Identifier("items")).WithType(GenericName(Identifier("IEnumerable")).WithTypeArgumentList(typeArgumentList))
                                           )
                                       )
                                   )
                                  .WithInitializer(
                                       ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, ArgumentList(SingletonSeparatedList(Argument(IdentifierName("items")))))
                                   )
                                  .WithBody(Block()),
                               ConstructorDeclaration(classIdentifier)
                                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                  .WithParameterList(ParameterList(SingletonSeparatedList(ArrayParameter(typeName).WithModifiers(TokenList(Token(SyntaxKind.ParamsKeyword))))))
                                  .WithInitializer(
                                       ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, ArgumentList(SingletonSeparatedList(Argument(IdentifierName("items")))))
                                   )
                                  .WithBody(Block()),
                               AddConversionBody(
                                   typeName,
                                   Identifier("IEnumerable"),
                                   MethodDeclaration(
                                       className,
                                       Identifier("From")
                                   )
                               ),
                               AddConversionBody(
                                       typeName,
                                       Identifier("List"),
                                       ConversionOperatorDeclaration(
                                           Token(SyntaxKind.ImplicitKeyword),
                                           className
                                       )
                                   )
                                   // array init is different param list
                                  .WithParameterList(ArrayParameters(typeName)),
                               AddConversionBody(
                                       typeName,
                                       Identifier("List"),
                                       MethodDeclaration(className, Identifier("From"))
                                   )
                                  .WithParameterList(ParameterList(SingletonSeparatedList(ArrayParameter(typeName).WithModifiers(TokenList(Token(SyntaxKind.ParamsKeyword)))))),
                               AddConversionBody(
                                   typeName,
                                   Identifier("Collection"),
                                   ConversionOperatorDeclaration(
                                       Token(SyntaxKind.ImplicitKeyword),
                                       className
                                   )
                               ),
                               AddConversionBody(
                                   typeName,
                                   Identifier("Collection"),
                                   MethodDeclaration(
                                       className,
                                       Identifier("From")
                                   )
                               ),
                               AddConversionBody(
                                   typeName,
                                   Identifier("List"),
                                   ConversionOperatorDeclaration(
                                       Token(SyntaxKind.ImplicitKeyword),
                                       className
                                   )
                               ),

                               AddConversionBody(
                                   typeName,
                                   Identifier("List"), MethodDeclaration(
                                       className,
                                       Identifier("From")
                                   )
                               ),
                               AddConversionBody(
                                       typeName,
                                       Identifier("List"),
                                       ConversionOperatorDeclaration(
                                           Token(SyntaxKind.ImplicitKeyword),
                                           className
                                       )
                                   )
                                  .WithParameterList(ImmutableArrayParameters(typeName)),

                               AddConversionBody(
                                       typeName,
                                       Identifier("List"),
                                       MethodDeclaration(
                                           className,
                                           Identifier("From")
                                       )
                                   )
                                  .WithParameterList(ImmutableArrayParameters(typeName)),

                               AddConversionBody(
                                   typeName,
                                   Identifier("ImmutableList"),
                                   ConversionOperatorDeclaration(
                                       Token(SyntaxKind.ImplicitKeyword),
                                       className
                                   )
                               ),

                               AddConversionBody(
                                   typeName,
                                   Identifier("ImmutableList"), MethodDeclaration(
                                       className,
                                       Identifier("From")
                                   )
                               )
                           }
                       )
                   );

            static ParameterListSyntax ImmutableArrayParameters(TypeSyntax typeName)
            {
                return ParameterList(
                    SingletonSeparatedList(
                        Parameter(
                                Identifier("items")
                            )
                           .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.InKeyword)
                                )
                            )
                           .WithType(
                                GenericName(
                                        Identifier("ImmutableArray")
                                    )
                                   .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList(
                                                typeName
                                            )
                                        )
                                    ).EnsureNullable()
                            )
                    )
                );
            }

            static ParameterListSyntax ArrayParameters(TypeSyntax typeName)
            {
                return ParameterList(SingletonSeparatedList(ArrayParameter(typeName)));
            }

            static ParameterSyntax ArrayParameter(TypeSyntax typeName)
            {
                return Parameter(Identifier("items"))
                   .WithType(
                        ArrayType(typeName)
                           .WithRankSpecifiers(
                                SingletonList(
                                    ArrayRankSpecifier(
                                        SingletonSeparatedList<ExpressionSyntax>(
                                            OmittedArraySizeExpression()
                                        )
                                    )
                                )
                            )
                    );
            }

            static BaseMethodDeclarationSyntax AddConversionBody(TypeSyntax typeName, SyntaxToken collectionName, BaseMethodDeclarationSyntax syntax)
            {
                TypeSyntax objectName = syntax is ConversionOperatorDeclarationSyntax d ? d.Type : syntax is MethodDeclarationSyntax m ? m.ReturnType : null!;
                objectName = objectName.EnsureNotNullable();
                return syntax
                      .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                      .WithParameterList(
                           ParameterList(
                               SingletonSeparatedList(
                                   Parameter(
                                           Identifier("items")
                                       )
                                      .WithType(
                                           GenericName(collectionName)
                                              .WithTypeArgumentList(
                                                   TypeArgumentList(
                                                       SingletonSeparatedList(
                                                           typeName
                                                       )
                                                   )
                                               ).EnsureNullable()
                                       )
                               )
                           )
                       )
                      .WithExpressionBody(
                           ArrowExpressionClause(
                               ObjectCreationExpression(objectName)
                                  .WithArgumentList(
                                       ArgumentList(
                                           SingletonSeparatedList(
                                               Argument(
                                                   IdentifierName("items")
                                               )
                                           )
                                       )
                                   )
                           )
                       )
                      .MakeMethodNullable(IdentifierName("items"))
                      .WithSemicolonToken(
                           Token(SyntaxKind.SemicolonToken)
                       );
            }
        }

        public StronglyTypedGenerator() : base(() => new SyntaxReceiver(Cache))
        {
        }

        public static CacheContainer<TypeDeclarationSyntax> Cache = new();

        public class SyntaxReceiver : SyntaxReceiverCache<TypeDeclarationSyntax>
        {
            public List<TypeDeclarationSyntax> CanBeResolved { get; } = new();
            public List<TypeDeclarationSyntax> CanHaveData { get; } = new();
            public List<TypeDeclarationSyntax> CreateContainers { get; } = new();

            public override string? GetKey(TypeDeclarationSyntax syntax)
            {
                var hasher = new CacheKeyHasher();
                hasher.Append(syntax.SyntaxTree.FilePath);
                hasher.Append(syntax.Keyword.Text);
                hasher.Append(syntax.Identifier.Text);
                hasher.Append(syntax.TypeParameterList);
                hasher.Append(syntax.AttributeLists);
                hasher.Append(syntax.BaseList);
                return hasher;
            }

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public override void OnVisitNode(TypeDeclarationSyntax syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation

                if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax)
                {
                    if (structDeclarationSyntax.AttributeLists.ContainsAttribute("GenerateContainer"))
                    {
                        CreateContainers.Add(structDeclarationSyntax);
                    }
                }

                if (syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax)
                {
                    if (syntaxNode.AttributeLists.ContainsAttribute("GenerateContainer"))
                    {
                        CreateContainers.Add(syntaxNode);
                    }

                    if (
                        syntaxNode.BaseList != null &&
                        syntaxNode.SyntaxTree.HasCompilationUnitRoot &&
                        syntaxNode.Members.OfType<PropertyDeclarationSyntax>().Any(z => z.Identifier.Text == "Data")
                    )
                    {
                        if (syntaxNode.BaseList.Types.Any(z => z.Type.GetSyntaxName() == "ICanBeResolved"))
                        {
                            CanBeResolved.Add(syntaxNode);
                        }
                        else if (syntaxNode.BaseList.Types.Any(z => z.Type.GetSyntaxName() == "ICanHaveData"))
                        {
                            CanHaveData.Add(syntaxNode);
                        }
                    }
                }
            }

            public SyntaxReceiver(CacheContainer<TypeDeclarationSyntax> cache) : base(cache)
            {
            }
        }
    }
}

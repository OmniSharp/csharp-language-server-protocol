using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxTrivia = Microsoft.CodeAnalysis.SyntaxTrivia;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class StronglyTypedGenerator : ISourceGenerator
    {
        private static string[] RequiredUsings = new[] {
            "System.Collections.Generic",
            "System.Collections.ObjectModel",
            "System.Collections.Immutable",
            "System.Linq",
        };

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!( context.SyntaxReceiver is SyntaxReceiver syntaxReceiver ))
            {
                return;
            }

            var options = ( context.Compilation as CSharpCompilation )?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation;

            var canBeResolvedSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.ICanBeResolved");
            var canHaveDataSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.ICanHaveData");
            var generateTypedDataAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.GenerateTypedDataAttribute");
            var generateContainerAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.GenerateContainerAttribute");
            var requestSymbol = compilation.GetTypeByMetadataName("MediatR.IRequest");

            foreach (var classToContain in syntaxReceiver.CreateContainers)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classToContain.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(classToContain);
                var hasAttribute = typeSymbol?.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateContainerAttributeSymbol));
                if (typeSymbol == null || hasAttribute != true) continue;

                var container = CreateContainerClass(classToContain)
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

                context.AddSource(
                    $"{classToContain.Identifier.Text}Container.cs",
                    cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }

            foreach (var canBeResolved in syntaxReceiver.CanBeResolved)
            {
                var semanticModel = context.Compilation.GetSemanticModel(canBeResolved.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(canBeResolved);
                if (typeSymbol == null) continue;
                var attribute = typeSymbol?.GetAttributes().FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateTypedDataAttributeSymbol));
                var isContainer = typeSymbol?.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateContainerAttributeSymbol)) == true;
                if (attribute == null) continue;

                if (!canBeResolved.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.ClassMustBePartial, canBeResolved.Identifier.GetLocation()));
                }

                var property = canBeResolved.Members.OfType<PropertyDeclarationSyntax>().Single(z => z.Identifier.Text == "Data");
                var dataInterfaceName = IdentifierName("ICanBeResolved");
                var partialClass = canBeResolved
                                  .WithAttributeLists(List<AttributeListSyntax>())
                                  .WithBaseList(null)
                                  .WithMembers(List<MemberDeclarationSyntax>())
                                  .AddMembers(GetWithDataMethod(canBeResolved, TypeConstraint(NullableType(IdentifierName("HandlerIdentity")))));

                var compilationMembers = new List<MemberDeclarationSyntax>() {
                };

                // remove the data property
                var typedClass = canBeResolved
                                .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter("T"))))
                                .WithMembers(canBeResolved.Members.Replace(property, GetPropertyImpl(property, dataInterfaceName).WithType(IdentifierName("T"))))
                                .AddMembers(
                                     GetWithDataMethod(canBeResolved, TypeConstraint(NullableType(IdentifierName("HandlerIdentity")))),
                                     GetExplicitProperty(property, dataInterfaceName),
                                     GetJDataProperty(),
                                     GetConvertFromOperator(canBeResolved, dataInterfaceName),
                                     GetConvertToOperator(canBeResolved, dataInterfaceName)
                                 )
                                .WithAttributeLists(
                                     List(
                                         canBeResolved.AttributeLists
                                                      .Where(z => !z.ToFullString().Contains("Method") && !z.ToFullString().Contains("GenerateTypedData"))
                                     )
                                 )
                                .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(dataInterfaceName))))
                                .WithConstraintClauses(
                                     SingletonList(
                                         TypeParameterConstraintClause(IdentifierName("T"))
                                            .WithConstraints(
                                                 SeparatedList<TypeParameterConstraintSyntax>(
                                                     new SyntaxNodeOrToken[] {
                                                         TypeConstraint(NullableType(IdentifierName("HandlerIdentity"))),
                                                         Token(SyntaxKind.CommaToken),
                                                         ConstructorConstraint()
                                                     }
                                                 )
                                             )
                                     )
                                 )
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
                                .WithLeadingTrivia(canBeResolved.GetLeadingTrivia().Where(z => !z.ToString().Contains("#nullable")))
                                .WithTrailingTrivia(canBeResolved.GetTrailingTrivia().Where(z => !z.ToString().Contains("#nullable")))
                    ;

                if (isContainer)
                {
                    var typedContainer = CreateContainerClass(typedClass)
                                        .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter("T"))))
                                        .WithConstraintClauses(
                                             SingletonList(
                                                 TypeParameterConstraintClause(IdentifierName("T"))
                                                    .WithConstraints(
                                                         SeparatedList(
                                                             new TypeParameterConstraintSyntax[] {
                                                                 TypeConstraint(NullableType(IdentifierName("HandlerIdentity"))),
                                                                 ConstructorConstraint()
                                                             }
                                                         )
                                                     )
                                             )
                                         );

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
                                                                                SimpleLambdaExpression(Parameter(Identifier("z")))
                                                                                   .WithExpressionBody(
                                                                                        CastExpression(
                                                                                            IdentifierName(canBeResolved.Identifier),
                                                                                            IdentifierName("z")
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
                               .WithSemicolonToken(
                                    Token(SyntaxKind.SemicolonToken)
                                )
                        );

                    compilationMembers.Add(typedContainer);
                }

                var cu = CompilationUnit()
                        .WithUsings(canBeResolved.SyntaxTree.GetCompilationUnitRoot().Usings)
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

                context.AddSource(
                    $"{canBeResolved.Identifier.Text}Typed.cs",
                    cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        }

        private static MethodDeclarationSyntax GetWithDataMethod(ClassDeclarationSyntax syntax, TypeParameterConstraintSyntax constraintSyntax)
        {
            return MethodDeclaration(
                       GenericName(Identifier(syntax.Identifier.Text))
                          .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("TData")))),
                       Identifier("WithData")
                   )
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                  .WithTypeParameterList(TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier("TData")))))
                  .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier("data")).WithType(IdentifierName("TData")))))
                  .WithConstraintClauses(
                       SingletonList(
                           TypeParameterConstraintClause(IdentifierName("TData"))
                              .WithConstraints(SeparatedList(new[] { constraintSyntax, ConstructorConstraint() }))
                       )
                   )
                  .WithBody(
                       Block(
                           SingletonList<StatementSyntax>(
                               ReturnStatement(
                                   ObjectCreationExpression(GenericName(Identifier(syntax.Identifier.Text))
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

        private static IEnumerable<ExpressionSyntax> GetMapping(ClassDeclarationSyntax syntax, IdentifierNameSyntax? paramName)
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

        private static ConversionOperatorDeclarationSyntax GetConvertToOperator(ClassDeclarationSyntax syntax, IdentifierNameSyntax dataInterfaceName)
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
                                                               ParenthesizedExpression(CastExpression(dataInterfaceName, paramName)),
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

        private static ConversionOperatorDeclarationSyntax GetConvertFromOperator(ClassDeclarationSyntax syntax, IdentifierNameSyntax dataInterfaceName)
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
                                                               ParenthesizedExpression(CastExpression(dataInterfaceName, paramName)),
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
                  .WithAccessorList(
                       AccessorList(
                           List(
                               new[] {
                                   AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                   AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                               }
                           )
                       )
                   );
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
                                               MemberAccessExpression(
                                                   SyntaxKind.SimpleMemberAccessExpression,
                                                   ParenthesizedExpression(
                                                       CastExpression(
                                                           IdentifierName("ICanBeResolved"),
                                                           ThisExpression()
                                                       )
                                                   ),
                                                   IdentifierName("Data")
                                               )
                                           )
                                       )
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                   AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                      .WithExpressionBody(
                                           ArrowExpressionClause(
                                               AssignmentExpression(
                                                   SyntaxKind.SimpleAssignmentExpression,
                                                   MemberAccessExpression(
                                                       SyntaxKind.SimpleMemberAccessExpression,
                                                       ParenthesizedExpression(
                                                           CastExpression(
                                                               IdentifierName("ICanBeResolved"),
                                                               ThisExpression()
                                                           )
                                                       ),
                                                       IdentifierName("Data")
                                                   ),
                                                   IdentifierName("value")
                                               )
                                           )
                                       )
                                      .WithSemicolonToken(
                                           Token(SyntaxKind.SemicolonToken)
                                       )
                               }
                           )
                       )
                   );
        }

        static PropertyDeclarationSyntax GetPropertyImpl(PropertyDeclarationSyntax syntax, IdentifierNameSyntax dataInterfaceName)
        {
            var canBeResolvedAccess = MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                ParenthesizedExpression(CastExpression(dataInterfaceName, ThisExpression())),
                IdentifierName("Data")
            );
            return syntax.WithAccessorList(
                AccessorList(
                    List(
                        new[] {
                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                               .WithExpressionBody(
                                    ArrowExpressionClause(
                                        PostfixUnaryExpression(
                                            SyntaxKind.SuppressNullableWarningExpression,
                                            ConditionalAccessExpression(
                                                canBeResolvedAccess,
                                                InvocationExpression(
                                                    MemberBindingExpression(
                                                        GenericName(Identifier("ToObject"))
                                                           .WithTypeArgumentList(
                                                                TypeArgumentList(
                                                                    SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))
                                                                )
                                                            )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                            AccessorDeclaration(
                                    SyntaxKind.SetAccessorDeclaration
                                )
                               .WithExpressionBody(
                                    ArrowExpressionClause(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            canBeResolvedAccess,
                                            InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("JToken"),
                                                        IdentifierName("FromObject")
                                                    )
                                                )
                                               .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList(
                                                            Argument(
                                                                IdentifierName("value")
                                                            )
                                                        )
                                                    )
                                                )
                                        )
                                    )
                                ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                        }
                    )
                )
            );
        }

        private static ClassDeclarationSyntax CreateContainerClass(ClassDeclarationSyntax syntax)
        {
            TypeSyntax typeName = IdentifierName(syntax.Identifier.Text);
            var classIdentifier = Identifier($"{syntax.Identifier.Text}Container");
            TypeSyntax className = IdentifierName($"{syntax.Identifier.Text}Container");
            if (syntax.Arity > 0)
            {
                typeName = GenericName(syntax.Identifier.Text)
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
                className = GenericName($"{syntax.Identifier.Text}Container")
                   .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName("T"))));
            }

            var typeArgumentList = TypeArgumentList(SingletonSeparatedList(typeName));

            return ClassDeclaration(classIdentifier)
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword)))
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
                                       MethodDeclaration(className, Identifier("Create"))
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
                                       Identifier("Create")
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
                                       Identifier("Create")
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
                                           Identifier("Create")
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
                                       Identifier("Create")
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
                                    )
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
                                               )
                                       )
                               )
                           )
                       )
                      .WithExpressionBody(
                           ArrowExpressionClause(
                               ObjectCreationExpression(syntax is ConversionOperatorDeclarationSyntax d ? d.Type : syntax is MethodDeclarationSyntax m ? m.ReturnType : null!)
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
                      .WithSemicolonToken(
                           Token(SyntaxKind.SemicolonToken)
                       );
            }
        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> CanBeResolved { get; } = new();
            public List<ClassDeclarationSyntax> CanHaveData { get; } = new();
            public List<ClassDeclarationSyntax> CreateContainers { get; } = new();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    if (classDeclarationSyntax.AttributeLists
                                              .SelectMany(z => z.Attributes)
                                              .Any(z => z.Name.ToFullString().Contains("GenerateContainer"))
                    )
                    {
                        CreateContainers.Add(classDeclarationSyntax);
                    }

                    if (
                        classDeclarationSyntax.BaseList != null &&
                        classDeclarationSyntax.SyntaxTree.HasCompilationUnitRoot &&
                        classDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>().Any(z => z.Identifier.Text == "Data")
                    )
                    {
                        if (classDeclarationSyntax.BaseList.Types.Any(z => z.ToString().EndsWith("ICanBeResolved")))
                        {
                            CanBeResolved.Add(classDeclarationSyntax);
                        }

                        if (classDeclarationSyntax.BaseList.Types.Any(z => z.ToString().EndsWith("ICanHaveData")))
                        {
                            CanHaveData.Add(classDeclarationSyntax);
                        }
                    }
                }
            }
        }
    }
}

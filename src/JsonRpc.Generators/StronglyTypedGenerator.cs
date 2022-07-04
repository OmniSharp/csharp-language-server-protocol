using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class StronglyTypedGenerator : IIncrementalGenerator
    {
        private static readonly string[] RequiredUsings =
        {
            "System.Collections.Generic",
            "System.Collections.ObjectModel",
            "System.Collections.Immutable",
            "System.Linq",
        };

        private record AttributeData(INamedTypeSymbol GenerateTypedDataAttributeSymbol, INamedTypeSymbol GenerateContainerAttributeSymbol);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var attributes = context.CompilationProvider
                                    .Select(
                                         (compilation, _) => new AttributeData(
                                                 compilation.GetTypeByMetadataName(
                                                     "OmniSharp.Extensions.LanguageServer.Protocol.Generation.GenerateTypedDataAttribute"
                                                 )!,
                                                 compilation.GetTypeByMetadataName(
                                                     "OmniSharp.Extensions.LanguageServer.Protocol.Generation.GenerateContainerAttribute"
                                                 )!
                                             )
                                     );

            var createContainersSyntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) => syntaxNode switch
                {
                    StructDeclarationSyntax structDeclarationSyntax when structDeclarationSyntax.AttributeLists.ContainsAttribute("GenerateContainer") => true,
                    TypeDeclarationSyntax typeDeclarationSyntax and (ClassDeclarationSyntax or RecordDeclarationSyntax) when typeDeclarationSyntax
                       .AttributeLists.ContainsAttribute("GenerateContainer") => true,
                    _ => false
                }, (syntaxContext, _) => syntaxContext
            );

            var typedParamsCandidatesSyntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) => syntaxNode switch
                {
                    TypeDeclarationSyntax typeDeclarationSyntax and (ClassDeclarationSyntax or RecordDeclarationSyntax) when typeDeclarationSyntax
                       .AttributeLists.ContainsAttribute("GenerateRequestMethods") => true,
                    _ => false
                }, (syntaxContext, _) => syntaxContext
            );

            var canBeResolvedSyntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) =>
                    syntaxNode is TypeDeclarationSyntax { BaseList: { } } typeDeclarationSyntax and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                 && syntaxNode.SyntaxTree.HasCompilationUnitRoot
                 && typeDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>().Any(z => z.Identifier.Text == "Data")
                 && typeDeclarationSyntax.BaseList.Types.Any(z => z.Type.GetSyntaxName() == "ICanBeResolved"),
                (syntaxContext, _) => syntaxContext
            );

            var canHaveDataSyntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) =>
                    syntaxNode is TypeDeclarationSyntax { BaseList: { } } typeDeclarationSyntax and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                 && syntaxNode.SyntaxTree.HasCompilationUnitRoot
                 && typeDeclarationSyntax.Members.OfType<PropertyDeclarationSyntax>().Any(z => z.Identifier.Text == "Data")
                 && typeDeclarationSyntax.BaseList.Types.Any(z => z.Type.GetSyntaxName() == "ICanHaveData"), (syntaxContext, _) => syntaxContext
            );
            
            context.RegisterSourceOutput(createContainersSyntaxProvider.Combine(attributes), GenerateContainerClass);
            context.RegisterSourceOutput(typedParamsCandidatesSyntaxProvider
                                        .Combine(canBeResolvedSyntaxProvider.Select((z, _) => (TypeDeclarationSyntax)z.Node).Collect())
                                        .Combine(canHaveDataSyntaxProvider.Select((z, _) => (TypeDeclarationSyntax)z.Node).Collect())
                                        .Combine(createContainersSyntaxProvider.Select((z, _) => (TypeDeclarationSyntax)z.Node).Collect())
                                        .Select((tuple, token) => (candidate: tuple.Left.Left.Left, resolvedItems: tuple.Left.Left.Right.Concat(tuple.Left.Right).Concat(tuple.Right).ToImmutableArray())), 
                                         GenerateTypedParams);
            context.RegisterSourceOutput(canBeResolvedSyntaxProvider.Combine(attributes), GenerateCanBeResolvedClass);
            context.RegisterSourceOutput(canHaveDataSyntaxProvider.Combine(attributes), GenerateCanHaveDataClass);
        }

        private void GenerateContainerClass(SourceProductionContext context, (GeneratorSyntaxContext syntaxContext, AttributeData attributeData) valueTuple)
        {
            var (syntaxContext, attributeData) = valueTuple;
            var classToContain = (TypeDeclarationSyntax)syntaxContext.Node;
            var typeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(classToContain);
            var attribute = typeSymbol?.GetAttributes()
                                       .FirstOrDefault(
                                            z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, attributeData.GenerateContainerAttributeSymbol)
                                        );
            if (typeSymbol == null || attribute is null) return;

            var containerName = attribute is { ConstructorArguments: { Length: > 0 } arguments } ? arguments[0].Value as string : null;

            var container = CreateContainerClass(classToContain, containerName)
               .AddAttributeLists(
                    AttributeList(
                        SeparatedList(
                            new[]
                            {
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
                     );

            foreach (var ns in RequiredUsings)
            {
                if (cu.Usings.All(z => z.Name.ToFullString() != ns))
                {
                    cu = cu.AddUsings(UsingDirective(ParseName(ns)));
                }
            }

            context.AddSource(
                $"{containerName ?? classToContain.Identifier.Text + "Container"}.cs",
                cu.NormalizeWhitespace().GetText(Encoding.UTF8)
            );
        }

        private void GenerateTypedParams(SourceProductionContext context, (GeneratorSyntaxContext syntaxContext, ImmutableArray<TypeDeclarationSyntax> resolvedItems) valueTuple)
        {
            var (syntaxContext, resolvedItems) = valueTuple;
            var classToContain = (TypeDeclarationSyntax)syntaxContext.Node;
            var typeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(classToContain);
            
            if (typeSymbol == null) return;
            var handlerInterface = typeSymbol.AllInterfaces.FirstOrDefault(z => z.Name == "IRequest" && z.Arity == 1);
            if (handlerInterface is null) return;
            var responseSymbol = handlerInterface?.TypeArguments[0];

            var isTyped = resolvedItems
               .Any(
                    item =>
                    {
                        var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(item);
                        return symbol is not null && SymbolEqualityComparer.Default.Equals(responseSymbol, symbol);
                    }
                );

            // TODO: Start here to finish creating strongly typed params
            var paramsType = CreateContainerClass(classToContain, containerName)
               .AddAttributeLists(
                    AttributeList(
                        SeparatedList(
                            new[]
                            {
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
                     );

            foreach (var ns in RequiredUsings)
            {
                if (cu.Usings.All(z => z.Name.ToFullString() != ns))
                {
                    cu = cu.AddUsings(UsingDirective(ParseName(ns)));
                }
            }

            context.AddSource(
                $"{containerName ?? classToContain.Identifier.Text + "Container"}.cs",
                cu.NormalizeWhitespace().GetText(Encoding.UTF8)
            );
        }

        private void GenerateCanBeResolvedClass(SourceProductionContext context, (GeneratorSyntaxContext syntaxContext, AttributeData attributeData) valueTuple)
        {
            var (syntaxContext, attributeData) = valueTuple;
            var canBeResolved = (TypeDeclarationSyntax)syntaxContext.Node;
            var typeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(canBeResolved)!;

            var dataInterfaceName = IdentifierName("ICanBeResolved");
            CreateTypedClass(
                context, canBeResolved, typeSymbol, dataInterfaceName, attributeData.GenerateTypedDataAttributeSymbol,
                attributeData.GenerateContainerAttributeSymbol, true
            );
        }

        private void GenerateCanHaveDataClass(SourceProductionContext context, (GeneratorSyntaxContext syntaxContext, AttributeData attributeData) valueTuple)
        {
            var (syntaxContext, attributeData) = valueTuple;
            var canBeResolved = (TypeDeclarationSyntax)syntaxContext.Node;
            var typeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(canBeResolved)!;

            var dataInterfaceName = IdentifierName("ICanHaveData");
            CreateTypedClass(
                context, canBeResolved, typeSymbol, dataInterfaceName, attributeData.GenerateTypedDataAttributeSymbol,
                attributeData.GenerateContainerAttributeSymbol, true
            );
        }

        private static void CreateTypedClass(
            SourceProductionContext context,
            TypeDeclarationSyntax candidate,
            INamedTypeSymbol typeSymbol,
            IdentifierNameSyntax dataInterfaceName,
            INamedTypeSymbol? generateTypedDataAttributeSymbol,
            INamedTypeSymbol? generateContainerAttributeSymbol,
            bool includeHandlerIdentity
            )
        {
            var attribute = typeSymbol?.GetAttributes()
                                       .FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateTypedDataAttributeSymbol));
            if (typeSymbol == null || attribute is null) return;
            var container = typeSymbol.GetAttributes()
                                      .FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, generateContainerAttributeSymbol));

            if (!candidate.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, candidate.Identifier.GetLocation(), candidate.Identifier.Text));
                return;
            }

            var property = candidate.Members.OfType<PropertyDeclarationSyntax>().Single(z => z.Identifier.Text == "Data");
            var partialClass = candidate
                              .WithAttributeLists(List<AttributeListSyntax>())
                              .WithBaseList(null)
                              .WithMembers(List<MemberDeclarationSyntax>())
                              .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                              .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                              .WithSemicolonToken(Token(SyntaxKind.None))
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
                                         new[]
                                         {
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

                if (!(container is { NamedArguments: { Length: > 0 } namedArguments } && namedArguments.FirstOrDefault(z => z.Key == "GenerateImplicitConversion") is { Value.Value: false }))
                {
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
                }

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
                     );
            foreach (var ns in RequiredUsings)
            {
                if (cu.Usings.All(z => z.Name.ToFullString() != ns))
                {
                    cu = cu.AddUsings(UsingDirective(ParseName(ns)));
                }
            }

            context.AddSource(
                $"{Path.GetFileNameWithoutExtension(candidate.SyntaxTree.FilePath)}_{candidate.Identifier.Text}Typed.cs",
                cu.NormalizeWhitespace().GetText(Encoding.UTF8)
            );
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
                                                       new ExpressionSyntax[]
                                                       {
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
                         .Where(
                              z => z.AccessorList?.Accessors.Any(
                                  a =>
                                      a.Keyword.IsKind(SyntaxKind.SetKeyword)
                                   || a.Keyword.IsKind(SyntaxKind.InitKeyword)
                              ) == true
                          )
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
                                                   new[]
                                                   {
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
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
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
                                                   new[]
                                                   {
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
                               new[]
                               {
                                   AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                      .WithExpressionBody(
                                           ArrowExpressionClause(
                                               InvocationExpression(
                                                   MemberAccessExpression(
                                                       SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("GetRawData")
                                                   )
                                               )
                                           )
                                       )
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                   AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
                                      .WithExpressionBody(
                                           ArrowExpressionClause(
                                               InvocationExpression(
                                                       MemberAccessExpression(
                                                           SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName("SetRawData")
                                                       )
                                                   )
                                                  .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(IdentifierName("value")))))
                                           )
                                       )
                                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                               }
                           )
                       )
                   );
        }

        private static PropertyDeclarationSyntax GetPropertyImpl(PropertyDeclarationSyntax syntax)
        {
            return syntax.WithAccessorList(
                AccessorList(
                    List(
                        new[]
                        {
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
                           new MemberDeclarationSyntax[]
                           {
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
                                               Parameter(Identifier("items")).WithType(
                                                   GenericName(Identifier("IEnumerable")).WithTypeArgumentList(typeArgumentList)
                                               )
                                           )
                                       )
                                   )
                                  .WithInitializer(
                                       ConstructorInitializer(
                                           SyntaxKind.BaseConstructorInitializer, ArgumentList(SingletonSeparatedList(Argument(IdentifierName("items"))))
                                       )
                                   )
                                  .WithBody(Block()),
                               ConstructorDeclaration(classIdentifier)
                                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                  .WithParameterList(
                                       ParameterList(SingletonSeparatedList(ArrayParameter(typeName).WithModifiers(TokenList(Token(SyntaxKind.ParamsKeyword)))))
                                   )
                                  .WithInitializer(
                                       ConstructorInitializer(
                                           SyntaxKind.BaseConstructorInitializer, ArgumentList(SingletonSeparatedList(Argument(IdentifierName("items"))))
                                       )
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
                                  .WithParameterList(
                                       ParameterList(SingletonSeparatedList(ArrayParameter(typeName).WithModifiers(TokenList(Token(SyntaxKind.ParamsKeyword)))))
                                   ),
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
                var objectName = syntax is ConversionOperatorDeclarationSyntax d ? d.Type : syntax is MethodDeclarationSyntax m ? m.ReturnType : null!;
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
    }
}

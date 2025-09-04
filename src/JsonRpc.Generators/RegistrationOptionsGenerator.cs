using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class RegistrationOptionsGenerator : IIncrementalGenerator
    {
        private static readonly string[] RequiredUsings =
        {
            "OmniSharp.Extensions.LanguageServer.Protocol",
            "OmniSharp.Extensions.LanguageServer.Protocol.Serialization",
            "OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities",
        };

        private record AttributeData(
            INamedTypeSymbol RegistrationOptionsInterfaceSymbol, 
            INamedTypeSymbol TextDocumentRegistrationOptionsInterfaceSymbol,
            INamedTypeSymbol NotebookDocumentRegistrationOptionsInterfaceSymbol,
            INamedTypeSymbol WorkDoneProgressOptionsInterfaceSymbol, 
            INamedTypeSymbol StaticRegistrationOptionsInterfaceSymbol
        );

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var attributes = context.CompilationProvider
                                    .Select(
                                         (compilation, _) =>
                                         {
                                             var registrationOptionsInterfaceSymbol =
                                                 compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions")!;
                                             var textDocumentRegistrationOptionsInterfaceSymbol =
                                                 compilation.GetTypeByMetadataName(
                                                     "OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions"
                                                 )!;
                                             var notebookDocumentRegistrationOptionsInterfaceSymbol =
                                                 compilation.GetTypeByMetadataName(
                                                     "OmniSharp.Extensions.LanguageServer.Protocol.Models.INotebookDocumentRegistrationOptions"
                                                 )!;
                                             var workDoneProgressOptionsInterfaceSymbol =
                                                 compilation.GetTypeByMetadataName(
                                                     "OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions"
                                                 )!;
                                             var staticRegistrationOptionsInterfaceSymbol =
                                                 compilation.GetTypeByMetadataName(
                                                     "OmniSharp.Extensions.LanguageServer.Protocol.Models.IStaticRegistrationOptions"
                                                 )!;
                                             return new AttributeData(
                                                 registrationOptionsInterfaceSymbol, 
                                                 textDocumentRegistrationOptionsInterfaceSymbol,
                                                 notebookDocumentRegistrationOptionsInterfaceSymbol,
                                                 workDoneProgressOptionsInterfaceSymbol,
                                                 staticRegistrationOptionsInterfaceSymbol
                                             );
                                         }
                                     );
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                                             (syntaxNode, _) =>
                                                 syntaxNode is TypeDeclarationSyntax tds and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                                              && tds.AttributeLists
                                                    .SelectMany(z => z.Attributes)
                                                    .Any(z => z.Name.ToFullString().Contains("GenerateRegistrationOptions")),
                                             (syntaxContext, _) => syntaxContext
                                         )
                                        .Combine(context.CompilationProvider)
                                        .Select(
                                             (tuple, _) =>
                                             {
                                                 var (syntaxContext, compilation) = tuple;
                                                 var typeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)syntaxContext.Node);
                                                 if (typeSymbol is not { }) return default!;
                                                 var data = RegistrationOptionAttributes.Parse(
                                                     compilation,
                                                     (TypeDeclarationSyntax)syntaxContext.Node,
                                                     typeSymbol
                                                 );
                                                 return data is not { }
                                                     ? default!
                                                     : ( registrationOptions: (TypeDeclarationSyntax)syntaxContext.Node,
                                                         semanticModel: syntaxContext.SemanticModel, typeSymbol, data: data! );
                                             }
                                         )
                                        .Where(z => z is { data: { } })
                                        .Combine(attributes)
                                        .Select(
                                             (tuple, _) => (
                                                 tuple.Left.registrationOptions, tuple.Left.data, tuple.Left.semanticModel, tuple.Left.typeSymbol,
                                                 attributes: tuple.Right )
                                         )
                ;

            context.RegisterSourceOutput(syntaxProvider, GenerateRegistrationOptions);
            context.RegisterSourceOutput(
                syntaxProvider
                   .Select(
                        (tuple, _) => ( namespaces: tuple.typeSymbol.ContainingNamespace.ToDisplayString(),
                                        AttributeArgument(TypeOfExpression(IdentifierName(tuple.typeSymbol.Name))) )
                    )
                   .Collect(), GenerateAssemblyRegistrationOptions
            );
        }

        private void GenerateRegistrationOptions(
            SourceProductionContext context,
            (TypeDeclarationSyntax registrationOptions, RegistrationOptionAttributes data, SemanticModel semanticModel, INamedTypeSymbol typeSymbol,
                AttributeData attributes) valueTuple
        )
        {
            var (registrationOptions, data, semanticModel, typeSymbol, attributes) = valueTuple;
            try
            {
                if (!registrationOptions.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(GeneratorDiagnostics.MustBePartial, registrationOptions.Identifier.GetLocation(), registrationOptions.Identifier.Text)
                    );
                    return;
                }

                var extendedRegistrationOptions = registrationOptions.WithAttributeLists(List<AttributeListSyntax>())
                                                                     .WithBaseList(
                                                                          BaseList(
                                                                              SingletonSeparatedList<BaseTypeSyntax>(
                                                                                  SimpleBaseType(
                                                                                      ParseName(attributes.RegistrationOptionsInterfaceSymbol.ToDisplayString())
                                                                                  )
                                                                              )
                                                                          )
                                                                      )
                                                                     .WithMembers(List<MemberDeclarationSyntax>());


                var staticRegistrationOptions = registrationOptions.WithIdentifier(Identifier("StaticOptions"))
                                                                   .WithMembers(
                                                                        List<MemberDeclarationSyntax>(
                                                                            registrationOptions.Members.OfType<PropertyDeclarationSyntax>()
                                                                        )
                                                                    )
                                                                   .WithAttributeLists(List<AttributeListSyntax>());

                var staticBaseList =
                    registrationOptions.BaseList?.Types.Where(z => z.Type.GetSyntaxName() != attributes.TextDocumentRegistrationOptionsInterfaceSymbol.Name)
                                       .ToArray() ?? Array.Empty<BaseTypeSyntax>();
                if (staticBaseList.Length > 0)
                {
                    staticRegistrationOptions = staticRegistrationOptions.WithBaseList(BaseList(SeparatedList(staticBaseList)));
                }
                else
                {
                    staticRegistrationOptions = staticRegistrationOptions.WithBaseList(null);
                }

                if (data.KeyExpression is { })
                {
                    var attributeList = AttributeList(
                        SingletonSeparatedList(
                            Attribute(
                                IdentifierName("RegistrationOptionsKey"), AttributeArgumentList(SeparatedList(data.KeyExpression.Select(AttributeArgument)))
                            )
                        )
                    );
                    extendedRegistrationOptions = extendedRegistrationOptions.AddAttributeLists(attributeList);
                    staticRegistrationOptions = staticRegistrationOptions.AddAttributeLists(attributeList);
                }

                if (data.SupportsTextDocumentSelector && !data.ImplementsTextDocumentSelector)
                {
                    if (registrationOptions.BaseList?.Types.Any(
                            z => z.Type.ToFullString().Contains(attributes.TextDocumentRegistrationOptionsInterfaceSymbol.Name)
                        ) != true)
                    {
                        extendedRegistrationOptions = ExtendAndImplementInterface(
                            extendedRegistrationOptions, attributes.TextDocumentRegistrationOptionsInterfaceSymbol
                        );
                    }

                    extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(
                        PropertyDeclaration(NullableType(IdentifierName("TextDocumentSelector")), Identifier("DocumentSelector"))
                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                           .WithAccessorList(
                                AccessorList(
                                    List(
                                        new[]
                                        {
                                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                                        }
                                    )
                                )
                            )
                    );
                }

                if (data.SupportsNotebookDocumentSelector && !data.ImplementsNotebookDocumentSelector)
                {
                    if (registrationOptions.BaseList?.Types.Any(
                            z => z.Type.ToFullString().Contains(attributes.NotebookDocumentRegistrationOptionsInterfaceSymbol.Name)
                        ) != true)
                    {
                        extendedRegistrationOptions = ExtendAndImplementInterface(
                            extendedRegistrationOptions, attributes.NotebookDocumentRegistrationOptionsInterfaceSymbol
                        );
                    }

                    extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(
                        PropertyDeclaration(NullableType(IdentifierName("NotebookDocumentSelector")), Identifier("DocumentSelector"))
                           .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                           .WithAccessorList(
                                AccessorList(
                                    List(
                                        new[]
                                        {
                                            AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                            AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                               .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                                        }
                                    )
                                )
                            )
                    );
                }

                if (data.SupportsWorkDoneProgress && !data.ImplementsWorkDoneProgress)
                {
                    if (registrationOptions.BaseList?.Types.Any(z => z.Type.GetSyntaxName() == attributes.WorkDoneProgressOptionsInterfaceSymbol.Name) != true)
                    {
                        extendedRegistrationOptions = ExtendAndImplementInterface(
                            extendedRegistrationOptions, attributes.WorkDoneProgressOptionsInterfaceSymbol
                        );
                        staticRegistrationOptions = ExtendAndImplementInterface(staticRegistrationOptions, attributes.WorkDoneProgressOptionsInterfaceSymbol);
                    }

                    staticRegistrationOptions = staticRegistrationOptions.AddMembers(GetWorkDoneProperty());
                    extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(GetWorkDoneProperty());
                }

                if (data.SupportsStaticRegistrationOptions && !data.ImplementsStaticRegistrationOptions)
                {
                    if (registrationOptions.BaseList?.Types.Any(z => z.Type.GetSyntaxName() == attributes.StaticRegistrationOptionsInterfaceSymbol.Name)
                     != true)
                    {
                        extendedRegistrationOptions = ExtendAndImplementInterface(
                            extendedRegistrationOptions, attributes.StaticRegistrationOptionsInterfaceSymbol
                        );
                        staticRegistrationOptions = ExtendAndImplementInterface(staticRegistrationOptions, attributes.StaticRegistrationOptionsInterfaceSymbol);
                    }

                    staticRegistrationOptions = staticRegistrationOptions.AddMembers(GetIdProperty());
                    extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(GetIdProperty());
                }

                if (data.RegistrationOptionsConverter is null && CreateConverter(
                        registrationOptions, staticRegistrationOptions.Members.OfType<PropertyDeclarationSyntax>()
                    ) is { } converter)
                {
                    extendedRegistrationOptions = extendedRegistrationOptions.AddAttributeLists(
                                                                                  AttributeList(
                                                                                      SingletonSeparatedList(
                                                                                          Attribute(
                                                                                              IdentifierName("RegistrationOptionsConverterAttribute"),
                                                                                              AttributeArgumentList(
                                                                                                  SingletonSeparatedList(
                                                                                                      AttributeArgument(
                                                                                                          TypeOfExpression(
                                                                                                              IdentifierName(converter.Identifier.Text)
                                                                                                          )
                                                                                                      )
                                                                                                  )
                                                                                              )
                                                                                          )
                                                                                      )
                                                                                  )
                                                                              )
                                                                             .AddMembers(converter);
                }

                extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(staticRegistrationOptions);

                var members = new List<MemberDeclarationSyntax> { extendedRegistrationOptions };

                var cu = CompilationUnit()
                        .WithUsings(registrationOptions.SyntaxTree.GetCompilationUnitRoot().Usings)
                        .AddMembers(
                             NamespaceDeclaration(ParseName(typeSymbol.ContainingNamespace.ToDisplayString()))
                                .WithMembers(List(members))
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

                context.AddSource($"{registrationOptions.Identifier.Text}.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(GeneratorDiagnostics.Exception, registrationOptions.GetLocation(), e.Message, e.StackTrace?.Replace("\n", " ") ?? string.Empty, e.ToString())
                );
                Debug.WriteLine(e);
                Debug.WriteLine(e.StackTrace);
            }
        }

        private void GenerateAssemblyRegistrationOptions(
            SourceProductionContext context, ImmutableArray<(string @namespace, AttributeArgumentSyntax argumentSyntax)> types
        )
        {
            var namespaces = new HashSet<string> { "OmniSharp.Extensions.LanguageServer.Protocol" };
            if (types.Any())
            {
                foreach (var item in types)
                {
                    namespaces.Add(item.@namespace);
                }

                var cu = CompilationUnit()
                        .WithUsings(List(namespaces.OrderBy(z => z).Select(z => UsingDirective(ParseName(z)))))
                        .AddAttributeLists(
                             AttributeList(
                                 AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)),
                                 SingletonSeparatedList(
                                     Attribute(
                                         IdentifierName("AssemblyRegistrationOptions"),
                                         AttributeArgumentList(SeparatedList(types.Select(z => z.argumentSyntax)))
                                     )
                                 )
                             )
                         );

                context.AddSource("AssemblyRegistrationOptions.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
        }


        private static TypeDeclarationSyntax ExtendAndImplementInterface(TypeDeclarationSyntax syntax, ITypeSymbol symbolToExtendFrom)
        {
            return syntax switch
            {
                ClassDeclarationSyntax cd  => cd.AddBaseListTypes(SimpleBaseType(ParseName(symbolToExtendFrom.ToDisplayString()))),
                RecordDeclarationSyntax rd => rd.AddBaseListTypes(SimpleBaseType(ParseName(symbolToExtendFrom.ToDisplayString()))),
                _                          => throw new NotSupportedException()
            };
        }

        private static PropertyDeclarationSyntax GetWorkDoneProperty()
        {
            return PropertyDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("WorkDoneProgress"))
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                  .WithAttributeLists(
                       SingletonList(
                           AttributeList(
                               SingletonSeparatedList(Attribute(IdentifierName("Optional")))
                           )
                       )
                   )
                  .WithAccessorList(CommonElements.GetSetAccessor);
        }

        private static PropertyDeclarationSyntax GetIdProperty()
        {
            return PropertyDeclaration(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))), Identifier("Id"))
                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                  .WithAttributeLists(
                       SingletonList(
                           AttributeList(
                               SingletonSeparatedList(Attribute(IdentifierName("Optional")))
                           )
                       )
                   )
                  .WithAccessorList(CommonElements.GetSetAccessor);
        }

        private static IEnumerable<ExpressionSyntax> GetMapping(IEnumerable<PropertyDeclarationSyntax> properties, IdentifierNameSyntax paramName)
        {
            return properties
                  .Where(
                       z => z.AccessorList?.Accessors.Any(a => a.Keyword.Kind() == SyntaxKind.SetKeyword || a.Keyword.Kind() == SyntaxKind.InitKeyword) == true
                   )
                  .Select(
                       property => AssignmentExpression(
                           SyntaxKind.SimpleAssignmentExpression,
                           IdentifierName(property.Identifier.Text),
                           MemberAccessExpression(
                               SyntaxKind.SimpleMemberAccessExpression,
                               paramName,
                               IdentifierName(property.Identifier.Text)
                           )
                       )
                   );
        }

        private TypeDeclarationSyntax? CreateConverter(TypeDeclarationSyntax syntax, IEnumerable<PropertyDeclarationSyntax> properties)
        {
            var attribute = syntax.AttributeLists
                                  .SelectMany(z => z.Attributes)
                                  .FirstOrDefault(z => z.IsAttribute("GenerateRegistrationOptions") && !z.IsAttribute("RegistrationOptionsConverter"));

            var expression = attribute is { ArgumentList: { Arguments: { Count: > 0 } arguments } } ? arguments[0].Expression : null;
            if (expression is null) return null;

            return ClassDeclaration($"{syntax.Identifier.Text}Converter")
                  .WithBaseList(
                       BaseList(
                           SingletonSeparatedList<BaseTypeSyntax>(
                               SimpleBaseType(
                                   GenericName(Identifier("RegistrationOptionsConverterBase"))
                                      .WithTypeArgumentList(
                                           TypeArgumentList(
                                               SeparatedList(
                                                   new TypeSyntax[]
                                                   {
                                                       IdentifierName(syntax.Identifier.Text),
                                                       IdentifierName("StaticOptions")
                                                   }
                                               )
                                           )
                                       )
                               )
                           )
                       )
                   )
                  .WithMembers(
                       List(
                           new MemberDeclarationSyntax[]
                           {
                               ConstructorDeclaration(Identifier($"{syntax.Identifier.Text}Converter"))
                                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                  .WithBody(Block()),
                               MethodDeclaration(IdentifierName("StaticOptions"), Identifier("Convert"))
                                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                                  .WithParameterList(
                                       ParameterList(
                                           SingletonSeparatedList(
                                               Parameter(Identifier("source"))
                                                  .WithType(IdentifierName(syntax.Identifier.Text))
                                           )
                                       )
                                   )
                                  .WithBody(
                                       Block(
                                           SingletonList<StatementSyntax>(
                                               ReturnStatement(
                                                   ObjectCreationExpression(
                                                           IdentifierName("StaticOptions")
                                                       )
                                                      .WithInitializer(
                                                           InitializerExpression(
                                                               SyntaxKind.ObjectInitializerExpression,
                                                               SeparatedList(
                                                                   GetMapping(properties, IdentifierName("source")).ToArray()
                                                               )
                                                           )
                                                       )
                                               )
                                           )
                                       )
                                   )
                           }
                       )
                   );
        }
    }
}

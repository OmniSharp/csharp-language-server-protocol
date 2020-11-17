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
    public class RegistrationOptionsGenerator : ISourceGenerator
    {
        private static string[] RequiredUsings = new[] {
            "OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities",
            "OmniSharp.Extensions.LanguageServer.Protocol",
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

            var registrationOptionsAttribute = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.RegistrationOptionsAttribute")!;
            var textDocumentAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.TextDocumentAttribute")!;
            var workDoneProgressAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.WorkDoneProgressAttribute")!;
            var registrationOptionsConverterAttributeSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.RegistrationOptionsConverterAttribute")!;
            var registrationOptionsInterfaceSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions")!;
            var textDocumentRegistrationOptionsInterfaceSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions")!;
            var workDoneProgressOptionsInterfaceSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions")!;

            foreach (var registrationOptions in syntaxReceiver.RegistrationOptions)
            {
                var semanticModel = context.Compilation.GetSemanticModel(registrationOptions.SyntaxTree);
                var typeSymbol = semanticModel.GetDeclaredSymbol(registrationOptions);
                var hasAttribute = typeSymbol?.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, registrationOptionsAttribute));
                if (typeSymbol == null || typeSymbol.IsAbstract || hasAttribute != true) continue;

                var converterAttribute = typeSymbol.GetAttributes()
                                                   .FirstOrDefault(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, registrationOptionsConverterAttributeSymbol));

                var extendedRegistrationOptions = registrationOptions
                                                 .WithAttributeLists(List<AttributeListSyntax>())
                                                 .WithBaseList(
                                                      BaseList(
                                                          SingletonSeparatedList<BaseTypeSyntax>(
                                                              SimpleBaseType(ParseName(registrationOptionsInterfaceSymbol.ToDisplayString()))
                                                          )
                                                      )
                                                  )
                                                 .WithMembers(List<MemberDeclarationSyntax>());

                var staticRegistrationOptions = registrationOptions
                                               .WithIdentifier(Identifier($"Static{registrationOptions.Identifier.Text}"))
                                               .WithMembers(List<MemberDeclarationSyntax>(registrationOptions.Members.OfType<PropertyDeclarationSyntax>()))
                                               .WithAttributeLists(List<AttributeListSyntax>());

                if (typeSymbol.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, textDocumentAttributeSymbol)))
                {
                    extendedRegistrationOptions = ExtendAndImplementInterface(extendedRegistrationOptions, textDocumentRegistrationOptionsInterfaceSymbol)
                       .AddMembers(
                            PropertyDeclaration(NullableType(IdentifierName("DocumentSelector")), Identifier("DocumentSelector"))
                               .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
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
                                )
                        );
                }

                if (typeSymbol.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(z.AttributeClass, workDoneProgressAttributeSymbol)))
                {
                    extendedRegistrationOptions = ExtendAndImplementInterface(extendedRegistrationOptions, workDoneProgressOptionsInterfaceSymbol)
                       .AddMembers(GetWorkDoneProperty());
                    staticRegistrationOptions = ExtendAndImplementInterface(staticRegistrationOptions, workDoneProgressOptionsInterfaceSymbol)
                       .AddMembers(GetWorkDoneProperty());
                }

                if (converterAttribute == null)
                {
                    var converter = CreateConverter(registrationOptions);
                    extendedRegistrationOptions = extendedRegistrationOptions
                                                 .AddAttributeLists(
                                                      AttributeList(
                                                          SingletonSeparatedList(
                                                              Attribute(
                                                                  IdentifierName("RegistrationOptionsConverterAttribute"),
                                                                  AttributeArgumentList(
                                                                      SingletonSeparatedList(
                                                                          AttributeArgument(
                                                                              TypeOfExpression(IdentifierName(converter.Identifier.Text))
                                                                          )
                                                                      )
                                                                  )
                                                              )
                                                          )
                                                      )
                                                  )
                                                 .AddMembers(converter);
                }

                var cu = CompilationUnit()
                        .WithUsings(registrationOptions.SyntaxTree.GetCompilationUnitRoot().Usings)
                        .AddMembers(
                             NamespaceDeclaration(ParseName(typeSymbol.ContainingNamespace.ToDisplayString()))
                                .AddMembers(extendedRegistrationOptions, staticRegistrationOptions)
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
                    $"{registrationOptions.Identifier.Text}Container.cs",
                    cu.NormalizeWhitespace().SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }

            static ClassDeclarationSyntax ExtendAndImplementInterface(ClassDeclarationSyntax syntax, ITypeSymbol symbolToExtendFrom)
            {
                return syntax
                   .AddBaseListTypes(SimpleBaseType(ParseName(symbolToExtendFrom.Name)));
            }

            static PropertyDeclarationSyntax GetWorkDoneProperty()
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
                      .WithAccessorList(
                           AccessorList(
                               List(
                                   new[] {
                                       AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                                       AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                                   }
                               )
                           )
                       );
            }
        }

        private static IEnumerable<ExpressionSyntax> GetMapping(ClassDeclarationSyntax syntax, IdentifierNameSyntax paramName)
        {
            return syntax.Members.OfType<PropertyDeclarationSyntax>()
                         .Where(z => z.AccessorList?.Accessors.Any(a => a.Keyword.Kind() == SyntaxKind.SetKeyword || a.Keyword.Kind() == SyntaxKind.InitKeyword) == true)
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

        private ClassDeclarationSyntax CreateConverter(ClassDeclarationSyntax syntax)
        {
            var attribute = syntax.AttributeLists
                                  .SelectMany(z => z.Attributes)
                                  .FirstOrDefault(z => z.Name.ToFullString().Contains("RegistrationOptions") && !z.Name.ToFullString().Contains("RegistrationOptionsConverter"));

            return ClassDeclaration($"{syntax.Identifier.Text}Converter")
                  .WithBaseList(
                       BaseList(
                           SingletonSeparatedList<BaseTypeSyntax>(
                               SimpleBaseType(
                                   GenericName(Identifier("RegistrationOptionsConverterBase"))
                                      .WithTypeArgumentList(
                                           TypeArgumentList(
                                               SeparatedList<TypeSyntax>(
                                                   new SyntaxNodeOrToken[] {
                                                       IdentifierName(syntax.Identifier.Text),
                                                       Token(SyntaxKind.CommaToken),
                                                       IdentifierName($"Static{syntax.Identifier.Text}")
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
                           new MemberDeclarationSyntax[] {
                               ConstructorDeclaration(Identifier("Converter"))
                                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                  .WithInitializer(
                                       ConstructorInitializer(
                                           SyntaxKind.BaseConstructorInitializer,
                                           ArgumentList(SingletonSeparatedList(Argument(attribute!.ArgumentList!.Arguments[0].Expression)))
                                       )
                                   )
                                  .WithBody(Block()),
                               MethodDeclaration(IdentifierName($"Static{syntax.Identifier.Text}"), Identifier("Convert"))
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
                                                           IdentifierName($"Static{syntax.Identifier.Text}")
                                                       )
                                                      .WithInitializer(
                                                           InitializerExpression(
                                                               SyntaxKind.ObjectInitializerExpression,
                                                               SeparatedList(
                                                                   GetMapping(syntax, IdentifierName("source")).ToArray()
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

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> RegistrationOptions { get; } = new();

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
                                              .Any(z => z.Name.ToFullString().Contains("RegistrationOptions"))
                    )
                    {
                        RegistrationOptions.Add(classDeclarationSyntax);
                    }
                }
            }
        }
    }
}

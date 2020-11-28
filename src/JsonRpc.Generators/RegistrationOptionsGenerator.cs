using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxTrivia = Microsoft.CodeAnalysis.SyntaxTrivia;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class RegistrationOptionsGenerator : CachedSourceGenerator<RegistrationOptionsGenerator.SyntaxReceiver, ClassDeclarationSyntax>
    {
        private static string[] RequiredUsings = new[] {
            "OmniSharp.Extensions.LanguageServer.Protocol",
            "OmniSharp.Extensions.LanguageServer.Protocol.Serialization",
            "OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities",
        };

        protected override void Execute(GeneratorExecutionContext context, SyntaxReceiver syntaxReceiver, AddCacheSource<ClassDeclarationSyntax> addCacheSource, ReportCacheDiagnostic<ClassDeclarationSyntax> cacheDiagnostic)
        {
            var options = ( context.Compilation as CSharpCompilation )?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation;

            var registrationOptionsInterfaceSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions")!;
            var textDocumentRegistrationOptionsInterfaceSymbol =
                compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.ITextDocumentRegistrationOptions")!;
            var workDoneProgressOptionsInterfaceSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IWorkDoneProgressOptions")!;
            // TODO:
            var staticRegistrationOptionsInterfaceSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.LanguageServer.Protocol.Models.IStaticRegistrationOptions")!;

            foreach (var registrationOptions in syntaxReceiver.RegistrationOptions)
            {
                try
                {
                    var semanticModel = context.Compilation.GetSemanticModel(registrationOptions.SyntaxTree);
                    var typeSymbol = semanticModel.GetDeclaredSymbol(registrationOptions);

                    if (typeSymbol is not { }) continue;
                    var data = RegistrationOptionAttributes.Parse(context, registrationOptions, typeSymbol);
                    if (data is not { }) continue;

                    if (!registrationOptions.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
                    {
                        cacheDiagnostic(registrationOptions, static r => Diagnostic.Create(GeneratorDiagnostics.MustBePartial, r.Identifier.GetLocation(), r.Identifier.Text));
                        continue;
                    }

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
                                                   .WithIdentifier(Identifier($"StaticOptions"))
                                                   .WithMembers(List<MemberDeclarationSyntax>(registrationOptions.Members.OfType<PropertyDeclarationSyntax>()))
                                                   .WithAttributeLists(List<AttributeListSyntax>());

                    var staticBaseList =
                        registrationOptions.BaseList?.Types.Where(z => z.Type.GetSyntaxName() != textDocumentRegistrationOptionsInterfaceSymbol.Name).ToArray()
                     ?? Array.Empty<BaseTypeSyntax>();
                    if (staticBaseList.Length > 0)
                    {
                        staticRegistrationOptions = staticRegistrationOptions.WithBaseList(BaseList(SeparatedList(staticBaseList)));
                    }
                    else
                    {
                        staticRegistrationOptions = staticRegistrationOptions.WithBaseList(null);
                    }

                    if (data.SupportsDocumentSelector && !data.ImplementsDocumentSelector)
                    {
                        if (registrationOptions.BaseList?.Types.Any(z => z.Type.ToFullString().Contains(textDocumentRegistrationOptionsInterfaceSymbol.Name)) != true)
                        {
                            extendedRegistrationOptions = ExtendAndImplementInterface(extendedRegistrationOptions, textDocumentRegistrationOptionsInterfaceSymbol);
                        }

                        extendedRegistrationOptions = extendedRegistrationOptions
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

                    if (data.SupportsWorkDoneProgress && !data.ImplementsWorkDoneProgress)
                    {
                        if (registrationOptions.BaseList?.Types.Any(z => z.Type.GetSyntaxName() == workDoneProgressOptionsInterfaceSymbol.Name) != true)
                        {
                            extendedRegistrationOptions = ExtendAndImplementInterface(extendedRegistrationOptions, workDoneProgressOptionsInterfaceSymbol);
                            staticRegistrationOptions = ExtendAndImplementInterface(staticRegistrationOptions, workDoneProgressOptionsInterfaceSymbol);
                        }

                        staticRegistrationOptions = staticRegistrationOptions.AddMembers(GetWorkDoneProperty());
                        extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(GetWorkDoneProperty());
                    }

                    if (data.SupportsStaticRegistrationOptions && !data.ImplementsStaticRegistrationOptions)
                    {
                        if (registrationOptions.BaseList?.Types.Any(z => z.Type.GetSyntaxName() == staticRegistrationOptionsInterfaceSymbol.Name) != true)
                        {
                            extendedRegistrationOptions = ExtendAndImplementInterface(extendedRegistrationOptions, staticRegistrationOptionsInterfaceSymbol);
                            staticRegistrationOptions = ExtendAndImplementInterface(staticRegistrationOptions, staticRegistrationOptionsInterfaceSymbol);
                        }

                        staticRegistrationOptions = staticRegistrationOptions.AddMembers(GetIdProperty());
                        extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(GetIdProperty());
                    }

                    if (data.RegistrationOptionsConverter is null
                     && CreateConverter(registrationOptions, staticRegistrationOptions.Members.OfType<PropertyDeclarationSyntax>()) is { } converter)
                    {
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

                    extendedRegistrationOptions = extendedRegistrationOptions.AddMembers(staticRegistrationOptions);

                    var members = new List<MemberDeclarationSyntax>() { extendedRegistrationOptions };

                    var cu = CompilationUnit()
                            .WithUsings(registrationOptions.SyntaxTree.GetCompilationUnitRoot().Usings)
                            .AddMembers(
                                 NamespaceDeclaration(ParseName(typeSymbol.ContainingNamespace.ToDisplayString()))
                                    .WithMembers(List(members))
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
                        $"{registrationOptions.Identifier.Text}.cs",
                        registrationOptions,
                        cu.NormalizeWhitespace().GetText(Encoding.UTF8)
                    );
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Exception, registrationOptions.GetLocation(), e.Message, e.StackTrace ?? string.Empty));
                    Debug.WriteLine(e);
                    Debug.WriteLine(e.StackTrace);
                }
            }

            static ClassDeclarationSyntax ExtendAndImplementInterface(ClassDeclarationSyntax syntax, ITypeSymbol symbolToExtendFrom)
            {
                return syntax
                   .AddBaseListTypes(SimpleBaseType(ParseName(symbolToExtendFrom.ToDisplayString())));
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

            static PropertyDeclarationSyntax GetIdProperty()
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

        private static IEnumerable<ExpressionSyntax> GetMapping(IEnumerable<PropertyDeclarationSyntax> properties, IdentifierNameSyntax paramName)
        {
            return properties
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

        private ClassDeclarationSyntax? CreateConverter(ClassDeclarationSyntax syntax, IEnumerable<PropertyDeclarationSyntax> properties)
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
                                                   new TypeSyntax[] {
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
                           new MemberDeclarationSyntax[] {
                               ConstructorDeclaration(Identifier($"{syntax.Identifier.Text}Converter"))
                                  .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                  .WithInitializer(
                                       ConstructorInitializer(
                                           SyntaxKind.BaseConstructorInitializer,
                                           ArgumentList(SingletonSeparatedList(Argument(expression)))
                                       )
                                   )
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

        public RegistrationOptionsGenerator() : base(() => new SyntaxReceiver(Cache))
        {

        }
        public static CacheContainer<ClassDeclarationSyntax> Cache = new();

        public class SyntaxReceiver : SyntaxReceiverCache<ClassDeclarationSyntax>
        {
            public List<ClassDeclarationSyntax> RegistrationOptions { get; } = new();

            public override string? GetKey(ClassDeclarationSyntax syntax)
            {
                var hasher = new CacheKeyHasher();
                hasher.Append(syntax.SyntaxTree.FilePath);
                hasher.Append(syntax.Identifier.Text);
                hasher.Append(syntax.TypeParameterList);
                hasher.Append(syntax.AttributeLists);
                hasher.Append(syntax.BaseList);
                foreach (var item in syntax.Members.OfType<PropertyDeclarationSyntax>().Select(z => z.Identifier.Text))
                {
                    hasher.Append(item);
                }
                return hasher;
            }

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public override void OnVisitNode(ClassDeclarationSyntax syntaxNode)
            {
                if (syntaxNode.AttributeLists
                              .SelectMany(z => z.Attributes)
                              .Any(z => z.Name.ToFullString().Contains("GenerateRegistrationOptions"))
                )
                {
                    RegistrationOptions.Add(syntaxNode);
                }
            }

            public SyntaxReceiver(CacheContainer<ClassDeclarationSyntax> cache) : base(cache) { }
        }
    }
}

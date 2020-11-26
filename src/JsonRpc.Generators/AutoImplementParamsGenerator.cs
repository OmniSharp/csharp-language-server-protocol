using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class AutoImplementParamsGenerator : ISourceGenerator
    {

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

            foreach (var candidate in syntaxReceiver.Candidates)
            {
                var members = new List<MemberDeclarationSyntax>();
                var model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(candidate);
                if (symbol is null) continue;

                var autoImplementProperties = AutoImplementInterfaces(candidate, symbol).ToArray();
                if (autoImplementProperties is { Length: >0 })
                {
                    var extendedParams = candidate
                                        .WithAttributeLists(List<AttributeListSyntax>())
                                        .WithMembers(List(autoImplementProperties))
                                        .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                                        .WithBaseList(null);
                    members.Add(extendedParams);
                }

                if (members.Count == 0) continue;

                if (!candidate.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(GeneratorDiagnostics.MustBePartial, candidate.Identifier.GetLocation(), candidate.Identifier.Text)
                    );
                }

                var cu = CompilationUnit(
                             List<ExternAliasDirectiveSyntax>(),
                             List(candidate.SyntaxTree.GetCompilationUnitRoot().Usings),
                             List<AttributeListSyntax>(),
                             SingletonList<MemberDeclarationSyntax>(
                                 NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                                    .WithMembers(List(members))
                                    .NormalizeWhitespace()
                                 )
                         )
                        .AddUsings(UsingDirective(ParseName("OmniSharp.Extensions.LanguageServer.Protocol.Serialization")))
                        .WithLeadingTrivia()
                        .WithTrailingTrivia()
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool), Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                        .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed)
                        .NormalizeWhitespace();

                context.AddSource(
                    $"{candidate.Identifier.Text}{( candidate.Arity > 0 ? candidate.Arity.ToString() : "" )}.cs",
                    cu.SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        }

        private static IEnumerable<MemberDeclarationSyntax> AutoImplementInterfaces(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is "IWorkDoneProgressParams" ) == true
             && symbol.GetMembers("WorkDoneToken").IsEmpty)
            {
                yield return PropertyDeclaration(NullableType(IdentifierName("ProgressToken")), Identifier("WorkDoneToken"))
                                          .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Optional"))))))
                                          .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
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

            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is  "IPartialItemsRequest" or "IPartialItemRequest") == true
             && symbol.GetMembers("PartialResultToken").IsEmpty)
            {
                yield return PropertyDeclaration(NullableType(IdentifierName("ProgressToken")), Identifier("PartialResultToken"))
                                          .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Optional"))))))
                                          .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
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

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            private string _attributes;
            public List<ClassDeclarationSyntax> Candidates { get; } = new ();

            public SyntaxReceiver()
            {
                _attributes = "Method,RegistrationOptions";
            }

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax tds && tds.AttributeLists.ContainsAttribute(_attributes))
                {
                    Candidates.Add(tds);
                }
            }
        }
    }
}

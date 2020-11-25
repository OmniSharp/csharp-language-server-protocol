using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class AutoImplementParamsGeneratorStrategy : ICompilationUnitGeneratorStrategy
    {
        public IEnumerable<MemberDeclarationSyntax> Apply(GeneratorData item)
        {
            var members = new List<MemberDeclarationSyntax>();
            var autoImplementProperties = AutoImplementInterfaces(item).ToArray();
            if (autoImplementProperties is { Length: >0 })
            {
                var extendedParams = item.TypeDeclaration
                                         .WithAttributeLists(List<AttributeListSyntax>())
                                         .WithMembers(List(autoImplementProperties))
                                         .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                                         .WithBaseList(null);
                members.Add(extendedParams);
            }

            if (members.Count == 0) yield break;

            if (!item.TypeDeclaration.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
            {
                item.Context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, item.TypeDeclaration.Identifier.GetLocation(), item.TypeDeclaration.Identifier.Text));
                yield break;
            }

            yield return NamespaceDeclaration(ParseName(item.JsonRpcAttributes.ModelNamespace))
                                      .WithMembers(List(members))
                                      .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                                      .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                                      .NormalizeWhitespace();
        }

        private static IEnumerable<MemberDeclarationSyntax> AutoImplementInterfaces(GeneratorData item)
        {
            if (item.TypeDeclaration.BaseList?.Types.Any(z => z.ToFullString().Contains("IWorkDoneProgressParams")) == true
             && item.TypeSymbol.GetMembers("WorkDoneToken").IsEmpty)
            {
                item.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Serialization");
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

            if (item.TypeDeclaration.BaseList?.Types.Any(
                    z =>
                        z.ToFullString().Contains("IPartialItemsRequest")
                     || z.ToFullString().Contains("IPartialItemRequest")
                ) == true
             && item.TypeSymbol.GetMembers("PartialResultToken").IsEmpty)
            {
                item.AdditionalUsings.Add("OmniSharp.Extensions.LanguageServer.Protocol.Serialization");
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
    }
}

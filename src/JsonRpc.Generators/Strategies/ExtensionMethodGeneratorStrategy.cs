using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators.Strategies
{
    internal class ExtensionMethodGeneratorStrategy : ICompilationUnitGeneratorStrategy
    {
        private readonly ImmutableArray<IExtensionMethodGeneratorStrategy> _extensionMethodGeneratorStrategies;

        public ExtensionMethodGeneratorStrategy(ImmutableArray<IExtensionMethodGeneratorStrategy> extensionMethodGeneratorStrategies)
        {
            _extensionMethodGeneratorStrategies = extensionMethodGeneratorStrategies;
        }
        public IEnumerable<MemberDeclarationSyntax> Apply(SourceProductionContext context, GeneratorData item)
        {
            var methods = _extensionMethodGeneratorStrategies.Aggregate(
                new List<MemberDeclarationSyntax>(), (m, strategy) => {
                    try
                    {
                        m.AddRange(strategy.Apply(context, item));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Strategy {strategy.GetType().FullName} failed!");
                        Debug.WriteLine(e);
                        Debug.WriteLine(e.StackTrace);
                    }

                    return m;
                }
            );

            var className = item.JsonRpcAttributes.HandlerName + "Extensions" + ( item.TypeDeclaration.Arity == 0 ? "" : item.TypeDeclaration.Arity.ToString() );

            var obsoleteAttribute = item.TypeDeclaration.AttributeLists
                                        .SelectMany(z => z.Attributes)
                                        .Where(z => z.IsAttribute("Obsolete"))
                                        .ToArray();

            var attributes = List(
                new[] {
                    AttributeList(
                        SeparatedList(
                            new[] {
                                Attribute(ParseName("System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute")),
                                Attribute(ParseName("System.Runtime.CompilerServices.CompilerGeneratedAttribute")),
                            }.Union(obsoleteAttribute)
                        )
                    )
                }
            );

            if (methods.Count == 0) yield break;

            yield return NamespaceDeclaration(ParseName(item.JsonRpcAttributes.HandlerNamespace))
                                      .WithMembers(
                                           SingletonList<MemberDeclarationSyntax>(
                                                ClassDeclaration(className)
                                                            .WithAttributeLists(attributes)
                                                            .WithModifiers(
                                                                 TokenList(
                                                                     new [] {
                                                                         Token(item.TypeSymbol.DeclaredAccessibility == Accessibility.Public ? SyntaxKind.PublicKeyword : SyntaxKind.InternalKeyword),
                                                                         Token(SyntaxKind.StaticKeyword),
                                                                         Token(SyntaxKind.PartialKeyword)
                                                                     }
                                                                 )
                                                             )
                                                            .WithMembers(List(methods))
                                                            .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                                                            .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                                           )
                                       );
        }
    }
}

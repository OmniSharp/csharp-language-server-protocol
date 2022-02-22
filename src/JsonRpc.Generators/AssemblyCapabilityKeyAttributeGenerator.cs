using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class AssemblyCapabilityKeyAttributeGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) =>
                {
                    if (syntaxNode.Parent is TypeDeclarationSyntax) return false;
                    if (syntaxNode is TypeDeclarationSyntax { Arity: 0, BaseList: { } bl } typeDeclarationSyntax
                            and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                     && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
                     && typeDeclarationSyntax.AttributeLists.ContainsAttribute("CapabilityKey")
                     && bl.Types.Any(
                            z => z.Type switch
                            {
                                SimpleNameSyntax
                                {
                                    Identifier: { Text: "ICapability" or "DynamicCapability" or "IDynamicCapability" or "LinkSupportCapability" }, Arity: 0
                                } => true,
                                _ => false
                            }
                        ))
                    {
                        return true;
                    }

                    return false;
                },
                (syntaxContext, _) =>
                {
                    var namespaces = new HashSet<string> { "OmniSharp.Extensions.LanguageServer.Protocol" };
                    var tds = (TypeDeclarationSyntax)syntaxContext.Node;

                    foreach (var item in syntaxContext.Node.SyntaxTree.GetCompilationUnitRoot()
                                                      .Usings.Where(z => z.Alias == null)
                                                      .Select(z => z.Name.ToFullString()))
                    {
                        namespaces.Add(item);
                    }

                    var typeSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node)!;

                    return ( namespaces,
                             Attribute(
                                 IdentifierName("AssemblyCapabilityKey"),
                                 AttributeArgumentList(
                                     SeparatedList(
                                         new[] { AttributeArgument(TypeOfExpression(ParseName(typeSymbol.ToDisplayString()))), }.Concat(
                                             tds.AttributeLists.GetAttribute("CapabilityKey")!.ArgumentList!.Arguments
                                         )
                                     )
                                 )
                             ) );
                }
            ).Collect();

            context.RegisterSourceOutput(syntaxProvider, GenerateAssemblyCapabilityKeys);
        }

        private void GenerateAssemblyCapabilityKeys(
            SourceProductionContext context, ImmutableArray<(HashSet<string> namespaces, AttributeSyntax attribute)> types
        )
        {
            var namespaces = types.Aggregate(
                new HashSet<string>(), (set, tuple) =>
                {
                    foreach (var name in tuple.namespaces)
                    {
                        set.Add(name);
                    }

                    return set;
                }
            );
            if (types.Any())
            {
                var cu = CompilationUnit()
                        .WithUsings(List(namespaces.OrderBy(z => z).Select(z => UsingDirective(ParseName(z)))))
                        .AddAttributeLists(
                             AttributeList(AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)), SeparatedList(types.Select(z => z.attribute)))
                         );

                context.AddSource("AssemblyCapabilityKeys.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
        }
    }
}

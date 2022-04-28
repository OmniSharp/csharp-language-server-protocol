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
    public class AssemblyJsonRpcHandlersAttributeGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxProvider = context
                                .SyntaxProvider
                                .CreateSyntaxProvider(
                                     (syntaxNode, _) =>
                                     {
                                         if (syntaxNode.Parent is TypeDeclarationSyntax) return false;
                                         if (syntaxNode is TypeDeclarationSyntax { Arity: 0, BaseList: { } bl } typeDeclarationSyntax
                                                 and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                                          && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword)
                                          && typeDeclarationSyntax.AttributeLists.ContainsAttribute("Method")
                                          && bl.Types.Any(
                                                 z => z.Type switch
                                                 {
                                                     SimpleNameSyntax { Identifier: { Text: "IJsonRpcNotificationHandler" }, Arity: 0 or 1 } => true,
                                                     SimpleNameSyntax { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 }            => true,
                                                     SimpleNameSyntax { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 or 2 }      => true,
                                                     SimpleNameSyntax { Identifier: { Text: "IJsonRpcHandler" }, Arity: 0 }                  => true,
                                                     _                                                                                       => false
                                                 }
                                             ))
                                         {
                                             return true;
                                         }

                                         if (syntaxNode is InterfaceDeclarationSyntax { Arity: 0, BaseList: { } bl2 } interfaceDeclarationSyntax
                                          && interfaceDeclarationSyntax.AttributeLists.ContainsAttribute("Method")
                                          && bl2.Types.Any(
                                                 z => z.Type switch
                                                 {
                                                     SimpleNameSyntax { Identifier: { Text: "IJsonRpcNotificationHandler" }, Arity: 0 or 1 } => true,
                                                     SimpleNameSyntax { Identifier: { Text: "ICanBeResolvedHandler" }, Arity: 1 }            => true,
                                                     SimpleNameSyntax { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 or 2 }      => true,
                                                     SimpleNameSyntax { Identifier: { Text: "IJsonRpcHandler" }, Arity: 0 }                  => true,
                                                     _                                                                                       => false
                                                 }
                                             ))
                                         {
                                             return true;
                                         }

                                         return false;
                                     },
                                     (syntaxContext, _) => AttributeArgument(
                                         TypeOfExpression(
                                             ParseName(syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node)!.ToDisplayString())
                                         )
                                     )
                                 )
                                .Collect();

            context.RegisterSourceOutput(syntaxProvider, GenerateAssemblyJsonRpcHandlers);
        }

        private void GenerateAssemblyJsonRpcHandlers(SourceProductionContext context, ImmutableArray<AttributeArgumentSyntax> types)
        {
            var namespaces = new HashSet<string> { "OmniSharp.Extensions.JsonRpc" };
            if (types.Any())
            {
                var cu = CompilationUnit()
                   .WithUsings(List(namespaces.OrderBy(z => z).Select(z => UsingDirective(ParseName(z)))));
                while (types.Length > 0)
                {
                    var innerTypes = types.Take(10).ToArray();
                    types = types.Skip(10).ToImmutableArray();
                    cu = cu.AddAttributeLists(
                        AttributeList(
                            AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)),
                            SingletonSeparatedList(Attribute(IdentifierName("AssemblyJsonRpcHandlers"), AttributeArgumentList(SeparatedList(innerTypes))))
                        )
                    );
                }

                context.AddSource("AssemblyJsonRpcHandlers.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
        }
    }
}

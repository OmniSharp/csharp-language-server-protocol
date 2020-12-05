using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class AssemblyCapabilityKeyAttributeGenerator : CachedSourceGenerator<AssemblyCapabilityKeyAttributeGenerator.SyntaxReceiver, TypeDeclarationSyntax>
    {
        protected override void Execute(
            GeneratorExecutionContext context, SyntaxReceiver syntaxReceiver, AddCacheSource<TypeDeclarationSyntax> addCacheSource,
            ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic
        )
        {
            var namespaces = new HashSet<string>() { "OmniSharp.Extensions.LanguageServer.Protocol" };
            var types = syntaxReceiver.FoundNodes
                                      .Concat(syntaxReceiver.Handlers)
                                      .Select(
                                           options => {
                                               var semanticModel = context.Compilation.GetSemanticModel(options.SyntaxTree);
                                               foreach (var item in options.SyntaxTree.GetCompilationUnitRoot()
                                                                           .Usings
                                                                           .Where(z => z.Alias == null)
                                                                           .Select(z => z.Name.ToFullString()))
                                               {
                                                   namespaces.Add(item);
                                               }

                                               var typeSymbol = semanticModel.GetDeclaredSymbol(options)!;

                                               return SyntaxFactory.Attribute(
                                                   SyntaxFactory.IdentifierName("AssemblyCapabilityKey"), SyntaxFactory.AttributeArgumentList(
                                                       SyntaxFactory.SeparatedList(
                                                           new[] {
                                                               SyntaxFactory.AttributeArgument(SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseName(typeSymbol.ToDisplayString()))),
                                                           }.Concat(options.AttributeLists.GetAttribute("CapabilityKey")!.ArgumentList!.Arguments)
                                                       )
                                                   )
                                               );
                                           }
                                       )
                                      .ToArray();
            if (types.Any())
            {
                var cu = SyntaxFactory.CompilationUnit()
                                      .WithUsings(SyntaxFactory.List(namespaces.OrderBy(z => z).Select(z => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(z)))))
                                      .AddAttributeLists(SyntaxFactory.AttributeList(target: SyntaxFactory.AttributeTargetSpecifier(SyntaxFactory.Token(SyntaxKind.AssemblyKeyword)), SyntaxFactory.SeparatedList(types)))
                                      .WithLeadingTrivia(SyntaxFactory.Comment(Preamble.GeneratedByATool))
                                      .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

                context.AddSource("AssemblyCapabilityKeys.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
        }

        public AssemblyCapabilityKeyAttributeGenerator() : base(() => new SyntaxReceiver(Cache))
        {
        }

        public static CacheContainer<TypeDeclarationSyntax> Cache = new();

        public class SyntaxReceiver : SyntaxReceiverCache<TypeDeclarationSyntax>
        {
            public List<TypeDeclarationSyntax> Handlers { get; } = new();

            public override string? GetKey(TypeDeclarationSyntax syntax)
            {
                var hasher = new CacheKeyHasher();
                hasher.Append(syntax.SyntaxTree.FilePath);
                hasher.Append(syntax.Keyword.Text);
                hasher.Append(syntax.Identifier.Text);
                hasher.Append(syntax.TypeParameterList);
                hasher.Append(syntax.AttributeLists);
                hasher.Append(syntax.BaseList);

                return hasher;
            }

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public override void OnVisitNode(TypeDeclarationSyntax syntaxNode)
            {
                if (syntaxNode.Parent is TypeDeclarationSyntax) return;
                if (syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax
                 && syntaxNode.Arity == 0
                 && !syntaxNode.Modifiers.Any(SyntaxKind.AbstractKeyword)
                 && syntaxNode.AttributeLists.ContainsAttribute("CapabilityKey")
                 && syntaxNode.BaseList is { } bl && bl.Types.Any(
                        z => z.Type switch {
                            SimpleNameSyntax { Identifier: { Text: "ICapability" }, Arity: 0 } => true,
                            _                                                                  => false
                        }
                    ))
                {
                    Handlers.Add(syntaxNode);
                }
            }

            public SyntaxReceiver(CacheContainer<TypeDeclarationSyntax> cache) : base(cache)
            {
            }
        }
    }
}

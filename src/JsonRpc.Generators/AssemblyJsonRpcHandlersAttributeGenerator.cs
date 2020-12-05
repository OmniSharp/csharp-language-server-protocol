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

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class AssemblyJsonRpcHandlersAttributeGenerator : CachedSourceGenerator<AssemblyJsonRpcHandlersAttributeGenerator.SyntaxReceiver, TypeDeclarationSyntax>
    {
        protected override void Execute(
            GeneratorExecutionContext context, SyntaxReceiver syntaxReceiver, AddCacheSource<TypeDeclarationSyntax> addCacheSource,
            ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic
        )
        {
            var namespaces = new HashSet<string>() { "OmniSharp.Extensions.JsonRpc" };
            var types = syntaxReceiver.FoundNodes
                                      .Concat(syntaxReceiver.Handlers)
                                      .Select(
                                           options => {
                                               var semanticModel = context.Compilation.GetSemanticModel(options.SyntaxTree);
                                               var typeSymbol = semanticModel.GetDeclaredSymbol(options)!;

                                               return AttributeArgument(TypeOfExpression(ParseName(typeSymbol.ToDisplayString())));
                                           }
                                       )
                                      .ToArray();
            if (types.Any())
            {
                var cu = CompilationUnit()
                        .WithUsings(List(namespaces.OrderBy(z => z).Select(z => UsingDirective(ParseName(z)))))
                        .AddAttributeLists(
                             AttributeList(
                                 target: AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)),
                                 SingletonSeparatedList(Attribute(IdentifierName("AssemblyJsonRpcHandlers"), AttributeArgumentList(SeparatedList(types))))
                             )
                         )
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool))
                        .WithTrailingTrivia(CarriageReturnLineFeed);

                context.AddSource("AssemblyJsonRpcHandlers.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
        }

        public AssemblyJsonRpcHandlersAttributeGenerator() : base(() => new SyntaxReceiver(Cache))
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
                 && syntaxNode.AttributeLists.ContainsAttribute("Method")
                 && syntaxNode.BaseList is { } bl && bl.Types.Any(
                        z => z.Type switch {
                            SimpleNameSyntax { Identifier: { Text: "IJsonRpcNotificationHandler" }, Arity: 0 or 1 } => true,
                            SimpleNameSyntax { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 or 2 }      => true,
                            SimpleNameSyntax { Identifier: { Text: "IJsonRpcHandler" }, Arity: 0 }                  => true,
                            _                                                                                       => false
                        }
                    ))
                {
                    Handlers.Add(syntaxNode);
                }

                if (syntaxNode is InterfaceDeclarationSyntax
                 && syntaxNode.Arity == 0
                 && syntaxNode.AttributeLists.ContainsAttribute("Method")
                 && syntaxNode.BaseList is { } bl2 && bl2.Types.Any(
                        z => z.Type switch {
                            SimpleNameSyntax { Identifier: { Text: "IJsonRpcNotificationHandler" }, Arity: 0 or 1 } => true,
                            SimpleNameSyntax { Identifier: { Text: "IJsonRpcRequestHandler" }, Arity: 1 or 2 }      => true,
                            SimpleNameSyntax { Identifier: { Text: "IJsonRpcHandler" }, Arity: 0 }                  => true,
                            _                                                                                       => false
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

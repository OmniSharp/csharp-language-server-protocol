using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Cache;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.CommonElements;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class AutoImplementParamsGenerator : CachedSourceGenerator<AutoImplementParamsGenerator.SyntaxReceiver, TypeDeclarationSyntax>
    {
        protected override void Execute(
            GeneratorExecutionContext context, SyntaxReceiver syntaxReceiver, AddCacheSource<TypeDeclarationSyntax> addCacheSource,
            ReportCacheDiagnostic<TypeDeclarationSyntax> cacheDiagnostic
        )
        {
            foreach (var candidate in syntaxReceiver.Candidates)
            {
                var members = new List<MemberDeclarationSyntax>();
                var model = context.Compilation.GetSemanticModel(candidate.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(candidate);
                if (symbol is null) continue;

                var autoImplementProperties = AutoImplementInterfaces(candidate, symbol).ToArray();
                if (autoImplementProperties is { Length: > 0 })
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
                    cacheDiagnostic(candidate, static c => Diagnostic.Create(GeneratorDiagnostics.MustBePartial, c.Identifier.GetLocation(), c.Identifier.Text));
                }

                var cu = CompilationUnit(
                             List<ExternAliasDirectiveSyntax>(),
                             List(candidate.SyntaxTree.GetCompilationUnitRoot().Usings),
                             List<AttributeListSyntax>(),
                             SingletonList<MemberDeclarationSyntax>(
                                 NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                                    .WithMembers(List(members))
                             )
                         )
                        .AddUsings(UsingDirective(ParseName("OmniSharp.Extensions.LanguageServer.Protocol.Serialization")))
                        .WithLeadingTrivia()
                        .WithTrailingTrivia()
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool), Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                        .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)), CarriageReturnLineFeed);

                addCacheSource(
                    $"{Path.GetFileNameWithoutExtension(candidate.SyntaxTree.FilePath)}_{candidate.Identifier.Text}{( candidate.Arity > 0 ? candidate.Arity.ToString() : "" )}.cs",
                    candidate,
                    cu.NormalizeWhitespace().GetText(Encoding.UTF8)
                );
            }
        }

        private static IEnumerable<MemberDeclarationSyntax> AutoImplementInterfaces(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is "IWorkDoneProgressParams") == true
             && symbol.GetMembers("WorkDoneToken").IsEmpty)
            {
                yield return PropertyDeclaration(NullableType(IdentifierName("ProgressToken")), Identifier("WorkDoneToken"))
                            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Optional"))))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithAccessorList(GetInitAccessor);
            }

            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is "IPartialItemsRequest" or "IPartialItemRequest") == true
             && symbol.GetMembers("PartialResultToken").IsEmpty)
            {
                yield return PropertyDeclaration(NullableType(IdentifierName("ProgressToken")), Identifier("PartialResultToken"))
                            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Optional"))))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithAccessorList(GetInitAccessor);
            }

            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is "IHandlerIdentity") == true
             && symbol.GetMembers("__identity").IsEmpty)
            {
                yield return PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("__identity"))
                            .WithAttributeLists(
                                 List(
                                     new[] {
                                         AttributeList(
                                             SeparatedList(
                                                 new[] {
                                                     Attribute(IdentifierName("JsonProperty"))
                                                        .WithArgumentList(
                                                             AttributeArgumentList(
                                                                 SeparatedList(
                                                                     new[] {
                                                                         AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("$$__handler_id__$$"))),
                                                                         AttributeArgument(
                                                                                 MemberAccessExpression(
                                                                                     SyntaxKind.SimpleMemberAccessExpression, IdentifierName("DefaultValueHandling"),
                                                                                     IdentifierName("Ignore")
                                                                                 )
                                                                             )
                                                                            .WithNameEquals(NameEquals(IdentifierName("DefaultValueHandling")))
                                                                     }
                                                                 )
                                                             )
                                                         ),
                                                     Attribute(IdentifierName("EditorBrowsable"))
                                                        .WithArgumentList(
                                                             AttributeArgumentList(
                                                                 SingletonSeparatedList(
                                                                     AttributeArgument(
                                                                         MemberAccessExpression(
                                                                             SyntaxKind.SimpleMemberAccessExpression, IdentifierName("EditorBrowsableState"),
                                                                             IdentifierName("Never")
                                                                         )
                                                                     )
                                                                 )
                                                             )
                                                         )
                                                 }
                                             )
                                         )
                                     }
                                 )
                             )
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithAccessorList(GetInitAccessor);
            }
        }

        public AutoImplementParamsGenerator() : base(() => new SyntaxReceiver(Cache))
        {
        }

        public static CacheContainer<TypeDeclarationSyntax> Cache = new();

        public class SyntaxReceiver : SyntaxReceiverCache<TypeDeclarationSyntax>
        {
            private readonly string _attributes;
            private readonly string _interfaces;
            public List<TypeDeclarationSyntax> Candidates { get; } = new();

            public SyntaxReceiver(CacheContainer<TypeDeclarationSyntax> cacheContainer) : base(cacheContainer)
            {
                _attributes = "Method,RegistrationOptions";
                _interfaces = "IPartialItemsRequest,IPartialItemRequest,IWorkDoneProgressParams,IHandlerIdentity";
            }

            public override string? GetKey(TypeDeclarationSyntax syntax)
            {
                var hasher = new CacheKeyHasher();
                hasher.Append(syntax.SyntaxTree.FilePath);
                hasher.Append(syntax.Keyword.Text);
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
            public override void OnVisitNode(TypeDeclarationSyntax syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax or RecordDeclarationSyntax
                 && ( syntaxNode.AttributeLists.ContainsAttribute(_attributes)
                   || syntaxNode.BaseList?.Types.Any(type => type.Type.GetSyntaxName() is { } n && _interfaces.Contains(n)) == true
                    )
                )
                {
                    Candidates.Add(syntaxNode);
                }
            }
        }
    }
}

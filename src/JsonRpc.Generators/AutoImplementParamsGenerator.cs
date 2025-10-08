using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.CommonElements;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class AutoImplementParamsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var _attributes = "Method,RegistrationOptions";
            var _interfaces = "IPartialItemsRequest,IPartialItemsWithInitialValueRequest,IPartialItemRequest,IPartialItemWithInitialValueRequest,IWorkDoneProgressParams,IHandlerIdentity";

            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                (syntaxNode, _) =>
                {
                    if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax and (ClassDeclarationSyntax or RecordDeclarationSyntax)
                     && ( typeDeclarationSyntax.AttributeLists.ContainsAttribute(_attributes)
                       || typeDeclarationSyntax.BaseList?.Types.Any(type => type.Type.GetSyntaxName() is { } n && _interfaces.Contains(n)) == true
                        )
                       )
                    {
                        return true;
                    }

                    return false;
                }, (syntaxContext, _) => { return syntaxContext; }
            );


            context.RegisterSourceOutput(syntaxProvider, GenerateAutoImplementedInterfaces);
        }

        private static void GenerateAutoImplementedInterfaces(SourceProductionContext context, GeneratorSyntaxContext syntaxContext)
        {
            var candidate = (TypeDeclarationSyntax)syntaxContext.Node;
            var members = new List<MemberDeclarationSyntax>();
            var model = syntaxContext.SemanticModel;
            var symbol = model.GetDeclaredSymbol(candidate);
            if (symbol is null) return;

            var autoImplementProperties = AutoImplementInterfaces(candidate, symbol).ToArray();
            if (autoImplementProperties is { Length: > 0 })
            {
                var extendedParams = candidate
                                    .WithAttributeLists(List<AttributeListSyntax>())
                                    .WithMembers(List(autoImplementProperties))
                                    .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>())
                                    .WithBaseList(null)
                                    .WithOpenBraceToken(Token(SyntaxKind.OpenBraceToken))
                                    .WithCloseBraceToken(Token(SyntaxKind.CloseBraceToken))
                                    .WithSemicolonToken(Token(SyntaxKind.None));
                members.Add(extendedParams);
            }

            if (members.Count == 0) return;

            if (!candidate.Modifiers.Any(z => z.IsKind(SyntaxKind.PartialKeyword)))
            {
                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.MustBePartial, candidate.Identifier.GetLocation(), candidate.Identifier.Text));
            }

            var cu = CompilationUnit(
                         List<ExternAliasDirectiveSyntax>(),
                         List(
                             candidate.SyntaxTree.GetCompilationUnitRoot().Usings.Concat(
                                 new[] { UsingDirective(ParseName("Newtonsoft.Json")), UsingDirective(ParseName("System.ComponentModel")) }
                             )
                         ), List<AttributeListSyntax>(),
                         SingletonList<MemberDeclarationSyntax>(
                             NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                                .WithMembers(List(members))
                         )
                     )
                    .AddUsings(UsingDirective(ParseName("OmniSharp.Extensions.LanguageServer.Protocol.Serialization")))
                    .WithLeadingTrivia()
                    .WithTrailingTrivia()
                    .WithLeadingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)))
                    .WithTrailingTrivia(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true)));

            context.AddSource(
                $"{Path.GetFileNameWithoutExtension(candidate.SyntaxTree.FilePath)}_{candidate.Identifier.Text}{( candidate.Arity > 0 ? candidate.Arity.ToString() : "" )}.cs",
                cu.NormalizeWhitespace().GetText(Encoding.UTF8)
            );
        }

        private static IEnumerable<MemberDeclarationSyntax> AutoImplementInterfaces(BaseTypeDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is "IWorkDoneProgressParams") == true
             && symbol.GetMembers("WorkDoneToken").IsEmpty)
            {
                yield return PropertyDeclaration(NullableType(IdentifierName("ProgressToken")), Identifier("WorkDoneToken"))
                            .WithAttributeLists(SingletonList(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("Optional"))))))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                            .WithAccessorList(GetInitAccessor);
            }

            if (syntax.BaseList?.Types.Any(z => z.Type.GetSyntaxName() is "IPartialItemsRequest" or "IPartialItemRequest" or "IPartialItemsWithInitialValueRequest" or "IPartialItemWithInitialValueRequest") == true
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
                                     new[]
                                     {
                                         AttributeList(
                                             SeparatedList(
                                                 new[]
                                                 {
                                                     Attribute(IdentifierName("JsonProperty"))
                                                        .WithArgumentList(
                                                             AttributeArgumentList(
                                                                 SeparatedList(
                                                                     new[]
                                                                     {
                                                                         AttributeArgument(
                                                                             LiteralExpression(
                                                                                 SyntaxKind.StringLiteralExpression, Literal("$$__handler_id__$$")
                                                                             )
                                                                         ),
                                                                         AttributeArgument(
                                                                                 MemberAccessExpression(
                                                                                     SyntaxKind.SimpleMemberAccessExpression,
                                                                                     IdentifierName("DefaultValueHandling"),
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
                                                                             SyntaxKind.SimpleMemberAccessExpression, ParseName("EditorBrowsableState"),
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
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using OmniSharp.Extensions.JsonRpc.Generators.Strategies;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static OmniSharp.Extensions.JsonRpc.Generators.Helpers;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class GenerateHandlerMethodsGenerator : ISourceGenerator
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

            var actionContextStrategies = ImmutableArray.Create<IExtensionMethodContextGeneratorStrategy>(
                new WarnIfResponseRouterIsNotProvidedStrategy(),
                new OnNotificationMethodGeneratorWithoutRegistrationOptionsStrategy(),
                new OnNotificationMethodGeneratorWithRegistrationOptionsStrategy(),
                new OnRequestMethodGeneratorWithoutRegistrationOptionsStrategy(),
                new OnRequestMethodGeneratorWithRegistrationOptionsStrategy(),
                new SendMethodNotificationStrategy(),
                new SendMethodRequestStrategy()
            );
            var actionStrategies = ImmutableArray.Create<IExtensionMethodGeneratorStrategy>(
                new EnsureNamespaceStrategy(),
                new HandlerRegistryActionContextRunnerStrategy(actionContextStrategies),
                new RequestProxyActionContextRunnerStrategy(actionContextStrategies)
            );

            var options = ( context.Compilation as CSharpCompilation )?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation;

            var generateHandlerMethodsAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateHandlerMethodsAttribute");
            var generateRequestMethodsAttributeSymbol = compilation.GetTypeByMetadataName("OmniSharp.Extensions.JsonRpc.Generation.GenerateRequestMethodsAttribute");

//            context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"generateHandlerMethodsAttributeSymbol: {generateHandlerMethodsAttributeSymbol.ToDisplayString()}"));
//            context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"generateRequestMethodsAttributeSymbol: {generateRequestMethodsAttributeSymbol.ToDisplayString()}"));

            foreach (var candidateClass in syntaxReceiver.Candidates)
            {
//                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"candidate: {candidateClass.Identifier.ToFullString()}"));
                // can this be async???
                context.CancellationToken.ThrowIfCancellationRequested();

                var model = compilation.GetSemanticModel(candidateClass.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(candidateClass);
                if (symbol is null) continue;

                var methods = new List<MemberDeclarationSyntax>();
                var additionalUsings = new HashSet<string> {
                    "System",
                    "System.Collections.Generic",
                    "System.Threading",
                    "System.Threading.Tasks",
                    "MediatR",
                    "Microsoft.Extensions.DependencyInjection",
                };

                ExtensionMethodData? actionItem = null;
                if (IsNotification(symbol))
                {
                var requestType = GetRequestType(candidateClass, symbol);
                    actionItem = new NotificationItem(
                        candidateClass,symbol,
                                         JsonRpcAttributes.Parse(context, candidateClass, symbol, additionalUsings),
                                         LspAttributes.Parse(context, candidateClass, symbol),
                                         DapAttributes.Parse(context, candidateClass, symbol),
                                        requestType,
                                         GetCapability(candidateClass, symbol),
                                         GetRegistrationOptions(candidateClass, symbol),
                                         additionalUsings,
                                         model,
                                         context
                    );
                }

                if (IsRequest(symbol))
                {
                    var requestType = GetRequestType(candidateClass, symbol);
                    actionItem = new RequestItem(
                        candidateClass,symbol,
                        JsonRpcAttributes.Parse(context, candidateClass, symbol, additionalUsings),
                        LspAttributes.Parse(context, candidateClass, symbol),
                        DapAttributes.Parse(context, candidateClass, symbol),
                        requestType,
                        GetResponseType(candidateClass, symbol),
                        GetCapability(candidateClass, symbol),
                        GetRegistrationOptions(candidateClass, symbol),
                        GetPartialItem(candidateClass, symbol, requestType),
                        GetPartialItems(candidateClass, symbol, requestType),
                        additionalUsings,
                        model,
                        context
                    );
                }

                if (actionItem is null) continue;

                methods = actionStrategies.Aggregate(methods,  (m, strategy) => {
                    try
                    {
                        m.AddRange(strategy.Apply(actionItem));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"Strategy {strategy.GetType().FullName} failed!");
                        Debug.WriteLine(e);
                        Debug.WriteLine(e.StackTrace);
                    }

                    return m;
                });

                if (!methods.Any()) continue;

                var className = GetExtensionClassName(symbol);

                var existingUsings = candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                   .Usings
                                                   .Select(x => x.WithoutTrivia())
                                                   .Union(
                                                        additionalUsings
                                                           .Except(
                                                                candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                                              .Usings.Select(z => z.Name.ToFullString())
                                                            )
                                                           .Distinct()
                                                           .Select(z => UsingDirective(IdentifierName(z)))
                                                    )
                                                   .OrderBy(x => x.Name.ToFullString())
                                                   .ToImmutableArray();

                var obsoleteAttribute = candidateClass.AttributeLists
                                                      .SelectMany(z => z.Attributes)
                                                      .Where(z => z.Name.ToFullString() == nameof(ObsoleteAttribute) || z.Name.ToFullString() == "Obsolete")
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

                var isInternal = candidateClass.Modifiers.Any(z => z.IsKind(SyntaxKind.InternalKeyword));

                var cu = CompilationUnit(
                             List<ExternAliasDirectiveSyntax>(),
                             List(existingUsings),
                             List<AttributeListSyntax>(),
                             List<MemberDeclarationSyntax>(
                                 new[] {
                                     NamespaceDeclaration(ParseName(symbol.ContainingNamespace.ToDisplayString()))
                                        .WithMembers(
                                             SingletonList<MemberDeclarationSyntax>(
                                                 ClassDeclaration(className)
                                                    .WithAttributeLists(attributes)
                                                    .WithModifiers(
                                                         TokenList(
                                                             new[] { isInternal ? Token(SyntaxKind.InternalKeyword) : Token(SyntaxKind.PublicKeyword) }.Concat(
                                                                 new[] {
                                                                     Token(SyntaxKind.StaticKeyword),
                                                                     Token(SyntaxKind.PartialKeyword)
                                                                 }
                                                             )
                                                         )
                                                     )
                                                    .WithMembers(List(methods))
                                                    .WithLeadingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))))
                                                    .WithTrailingTrivia(TriviaList(Trivia(NullableDirectiveTrivia(Token(SyntaxKind.RestoreKeyword), true))))
                                                    .NormalizeWhitespace()
                                             )
                                         )
                                 }
                             )
                         )
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool))
                        .WithTrailingTrivia(CarriageReturnLineFeed)
                        .NormalizeWhitespace();

                context.AddSource(
                    $"JsonRpc_Handlers_{candidateClass.Identifier.ToFullString().Replace(".", "_")}.cs",
                    cu.SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        }


        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        internal class SyntaxReceiver : ISyntaxReceiver
        {
            public List<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                    classDeclarationSyntax.AttributeLists.Count > 0
                )
                {
                    Candidates.Add(classDeclarationSyntax);
                }

                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is InterfaceDeclarationSyntax interfaceDeclarationSyntax &&
                    interfaceDeclarationSyntax.AttributeLists.Count > 0
                )
                {
                    Candidates.Add(interfaceDeclarationSyntax);
                }
            }
        }
    }
}

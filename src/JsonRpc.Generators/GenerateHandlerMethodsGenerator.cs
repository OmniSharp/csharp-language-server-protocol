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

            var compilationUnitStrategies = CreateCompilationUnitGeneratorStrategies();

            var options = ( context.Compilation as CSharpCompilation )?.SyntaxTrees[0].Options as CSharpParseOptions;
            var compilation = context.Compilation;

            foreach (var candidateClass in syntaxReceiver.Candidates)
            {
//                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"candidate: {candidateClass.Identifier.ToFullString()}"));
                // can this be async???
                context.CancellationToken.ThrowIfCancellationRequested();

                var additionalUsings = new HashSet<string> {
                    "System",
                    "System.Collections.Generic",
                    "System.Threading",
                    "System.Threading.Tasks",
                    "MediatR",
                    "Microsoft.Extensions.DependencyInjection"
                };

                GeneratorData? actionItem = null;

                try
                {
                    actionItem = GeneratorData.Create(context, candidateClass, additionalUsings);
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Exception, candidateClass.GetLocation(), e.Message, e.StackTrace ?? string.Empty));
                    Debug.WriteLine(e);
                    Debug.WriteLine(e.StackTrace);
                }

                if (actionItem is null) continue;

                var members = compilationUnitStrategies.Aggregate(
                    new List<MemberDeclarationSyntax>(), (m, strategy) => {
                        try
                        {
                            m.AddRange(strategy.Apply(actionItem));
                        }
                        catch (Exception e)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    GeneratorDiagnostics.Exception, candidateClass.GetLocation(), $"Strategy {strategy.GetType().FullName} failed!" + " - " + e.Message,
                                    e.StackTrace ?? string.Empty
                                )
                            );
                            Debug.WriteLine($"Strategy {strategy.GetType().FullName} failed!");
                            Debug.WriteLine(e);
                            Debug.WriteLine(e.StackTrace);
                        }

                        return m;
                    }
                );

                if (!members.Any()) continue;

                var existingUsings = candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                   .Usings
                                                   .Select(x => x.WithoutTrivia())
                                                   .Union(
                                                        additionalUsings
                                                           .Except(
                                                                candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                                              .Usings
                                                                              .Where(z => z.Alias == null)
                                                                              .Select(z => z.Name.ToFullString())
                                                            )
                                                           .Distinct()
                                                           .Select(z => UsingDirective(IdentifierName(z)))
                                                    )
                                                   .OrderBy(x => x.Name.ToFullString())
                                                   .ToImmutableArray();

                var cu = CompilationUnit(
                             List<ExternAliasDirectiveSyntax>(),
                             List(existingUsings),
                             List<AttributeListSyntax>(),
                             List(members)
                         )
                        .WithLeadingTrivia(Comment(Preamble.GeneratedByATool))
                        .WithTrailingTrivia(CarriageReturnLineFeed)
                        .NormalizeWhitespace();

                context.AddSource(
                    $"{candidateClass.Identifier.ToFullString().Trim()}{(candidateClass.Arity > 0 ? candidateClass.Arity.ToString() : "")}.cs",
                    cu.SyntaxTree.GetRoot().GetText(Encoding.UTF8)
                );
            }
        }

        private static ImmutableArray<ICompilationUnitGeneratorStrategy> CreateCompilationUnitGeneratorStrategies()
        {
            var actionContextStrategies = ImmutableArray.Create<IExtensionMethodContextGeneratorStrategy>(
                new WarnIfResponseRouterIsNotProvidedStrategy(),
                new OnNotificationMethodGeneratorWithoutRegistrationOptionsStrategy(),
                new OnNotificationMethodGeneratorWithRegistrationOptionsStrategy(),
                new OnRequestMethodGeneratorWithoutRegistrationOptionsStrategy(false),
                new OnRequestMethodGeneratorWithoutRegistrationOptionsStrategy(true),
                new OnRequestTypedResolveMethodGeneratorWithoutRegistrationOptionsStrategy(),
                new OnRequestMethodGeneratorWithRegistrationOptionsStrategy(false),
                new OnRequestMethodGeneratorWithRegistrationOptionsStrategy(true),
                new OnRequestTypedResolveMethodGeneratorWithRegistrationOptionsStrategy(),
                new SendMethodNotificationStrategy(),
                new SendMethodRequestStrategy()
            );
            var actionStrategies = ImmutableArray.Create<IExtensionMethodGeneratorStrategy>(
                new EnsureNamespaceStrategy(),
                new HandlerRegistryActionContextRunnerStrategy(actionContextStrategies),
                new RequestProxyActionContextRunnerStrategy(actionContextStrategies),
                new TypedDelegatingHandlerStrategy()
            );
            var compilationUnitStrategies = ImmutableArray.Create<ICompilationUnitGeneratorStrategy>(
                new AutoImplementParamsGeneratorStrategy(),
                new HandlerGeneratorStrategy(),
                new ExtensionMethodGeneratorStrategy(actionStrategies)
            );
            return compilationUnitStrategies;
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

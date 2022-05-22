using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OmniSharp.Extensions.JsonRpc.Generators.Contexts;
using OmniSharp.Extensions.JsonRpc.Generators.Strategies;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace OmniSharp.Extensions.JsonRpc.Generators
{
    [Generator]
    public class GenerateHandlerMethodsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var _attributes = "GenerateHandler,GenerateRequestMethods,GenerateHandlerMethods";
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                                             (syntaxNode, _) =>
                                                 syntaxNode is TypeDeclarationSyntax tds
                                                     and (ClassDeclarationSyntax or RecordDeclarationSyntax or InterfaceDeclarationSyntax)
                                              && tds.AttributeLists.ContainsAttribute(_attributes), (syntaxContext, _) => syntaxContext
                                         )
                                        .Combine(context.CompilationProvider)
                                        .Select(
                                             (tuple, _) =>
                                             {
                                                 var (syntaxContext, compilaiton) = tuple;
                                                 var additionalUsings = new HashSet<string>
                                                 {
                                                     "System",
                                                     "System.Collections.Generic",
                                                     "System.Threading",
                                                     "System.Threading.Tasks",
                                                     "MediatR",
                                                     "Microsoft.Extensions.DependencyInjection"
                                                 };

                                                 GeneratorData? actionItem = null;
                                                 Diagnostic? diagnostic = null;

                                                 try
                                                 {
                                                     actionItem = GeneratorData.Create(
                                                         compilaiton, (TypeDeclarationSyntax)syntaxContext.Node, syntaxContext.SemanticModel, additionalUsings
                                                     );
                                                     if (actionItem is null)
                                                     {
                                                         diagnostic = Diagnostic.Create(
                                                             GeneratorDiagnostics.MustBeARequestOrNotification, syntaxContext.Node.GetLocation(),
                                                             ( (TypeDeclarationSyntax)syntaxContext.Node ).Identifier.Text
                                                         );
                                                     }
                                                 }
                                                 catch (Exception e)
                                                 {
                                                     diagnostic = Diagnostic.Create(
                                                         GeneratorDiagnostics.Exception, syntaxContext.Node.GetLocation(), e.Message,
                                                         e.StackTrace?.Replace("\n", " ") ?? string.Empty, e.ToString()
                                                     );
                                                     Debug.WriteLine(e);
                                                     Debug.WriteLine(e.StackTrace);
                                                 }

                                                 return ( actionItem, diagnostic, additionalUsings );
                                             }
                                         );

            context.RegisterSourceOutput(syntaxProvider, GenerateHandlerMethods);
            context.RegisterSourceOutput(
                syntaxProvider.Where(z => z.actionItem is { }).SelectMany((z, _) => z.actionItem!.AssemblyJsonRpcHandlersAttributeArguments).Collect(),
                GenerateAssemblyJsonRpcHandlers
            );
        }

        private void GenerateHandlerMethods(
            SourceProductionContext context, (GeneratorData? actionItem, Diagnostic? diagnostic, HashSet<string> additionalUsings) valueTuple
        )
        {
            var (actionItem, diagnostic, additionalUsings) = valueTuple;
            //                context.ReportDiagnostic(Diagnostic.Create(GeneratorDiagnostics.Message, null, $"candidate: {candidateClass.Identifier.ToFullString()}"));
            // can this be async???
            context.CancellationToken.ThrowIfCancellationRequested();

            if (actionItem is null)
            {
                if (diagnostic is { }) context.ReportDiagnostic(diagnostic);
                return;
            }

            var candidateClass = actionItem.TypeDeclaration;

            var members = CompilationUnitGeneratorStrategies.Aggregate(
                new List<MemberDeclarationSyntax>(), (m, strategy) =>
                {
                    try
                    {
                        m.AddRange(strategy.Apply(context, actionItem));
                    }
                    catch (Exception e)
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                GeneratorDiagnostics.Exception, candidateClass.GetLocation(),
                                $"Strategy {strategy.GetType().FullName} failed!" + " - " + e.Message, e.StackTrace?.Replace("\n", " ") ?? string.Empty
                            )
                        );
                        Debug.WriteLine($"Strategy {strategy.GetType().FullName} failed!");
                        Debug.WriteLine(e);
                        Debug.WriteLine(e.StackTrace);
                    }

                    return m;
                }
            );

            if (!members.Any()) return;

            var namespacesMapping = new Dictionary<string, string[]>
            {
                ["OmniSharp.Extensions.DebugAdapter"] = new[]
                {
                    "OmniSharp.Extensions.DebugAdapter.Protocol", "OmniSharp.Extensions.DebugAdapter.Protocol.Models",
                    "OmniSharp.Extensions.DebugAdapter.Protocol.Events", "OmniSharp.Extensions.DebugAdapter.Protocol.Requests"
                },
                ["OmniSharp.Extensions.LanguageProtocol"] = new[]
                    { "OmniSharp.Extensions.LanguageServer.Protocol", "OmniSharp.Extensions.LanguageServer.Protocol.Models" },
            };

            foreach (var assembly in actionItem.Compilation.References.Select(actionItem.Compilation.GetAssemblyOrModuleSymbol)
                                               .OfType<IAssemblySymbol>()
                                               .Concat(new[] { actionItem.Compilation.Assembly }))
            {
                if (namespacesMapping.TryGetValue(assembly.Name, out var additionalNamespaceUsings))
                {
                    foreach (var item in additionalNamespaceUsings)
                    {
                        additionalUsings.Add(item);
                    }
                }
            }

            var existingUsings = candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                               .Usings.Select(x => x.WithoutTrivia())
                                               .Union(
                                                    additionalUsings.Except(
                                                                         candidateClass.SyntaxTree.GetCompilationUnitRoot()
                                                                                       .Usings.Where(z => z.Alias == null)
                                                                                       .Select(z => z.Name.ToFullString())
                                                                     )
                                                                    .Except(new[] { "<global namespace>" }) // I think there is a better way... but for now..
                                                                    .Distinct()
                                                                    .Select(z => UsingDirective(IdentifierName(z)))
                                                )
                                               .OrderBy(x => x.Name.ToFullString())
                                               .ToImmutableArray();

            var cu = CompilationUnit(List<ExternAliasDirectiveSyntax>(), List(existingUsings), List<AttributeListSyntax>(), List(members));

            context.AddSource(
                $"{candidateClass.Identifier.Text}{( candidateClass.Arity > 0 ? candidateClass.Arity.ToString() : "" )}.cs",
                cu.NormalizeWhitespace().GetText(Encoding.UTF8)
            );
        }

        private void GenerateAssemblyJsonRpcHandlers(SourceProductionContext context, ImmutableArray<AttributeArgumentSyntax> handlers)
        {
            var namespaces = new HashSet<string> { "OmniSharp.Extensions.JsonRpc" };
            if (handlers.Any())
            {
                var cu = CompilationUnit()
                   .WithUsings(List(namespaces.OrderBy(z => z).Select(z => UsingDirective(ParseName(z)))));
                while (handlers.Length > 0)
                {
                    var innerTypes = handlers.Take(10).ToArray();
                    handlers = handlers.Skip(10).ToImmutableArray();
                    cu = cu.AddAttributeLists(
                        AttributeList(
                            AttributeTargetSpecifier(Token(SyntaxKind.AssemblyKeyword)),
                            SingletonSeparatedList(Attribute(IdentifierName("AssemblyJsonRpcHandlers"), AttributeArgumentList(SeparatedList(innerTypes))))
                        )
                    );
                }

                context.AddSource("GeneratedAssemblyJsonRpcHandlers.cs", cu.NormalizeWhitespace().GetText(Encoding.UTF8));
            }
        }

        private static readonly ImmutableArray<ICompilationUnitGeneratorStrategy> CompilationUnitGeneratorStrategies = GetCompilationUnitGeneratorStrategies();

        private static ImmutableArray<ICompilationUnitGeneratorStrategy> GetCompilationUnitGeneratorStrategies()
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
                new HandlerGeneratorStrategy(),
                new ExtensionMethodGeneratorStrategy(actionStrategies)
            );
            return compilationUnitStrategies;
        }
    }
}

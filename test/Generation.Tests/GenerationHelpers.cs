using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NSubstitute;
//using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.JsonRpc.Generators;
//using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Xunit;

namespace Generation.Tests
{
    public static class GenerationHelpers
    {
        static GenerationHelpers()
        {
            // this "core assemblies hack" is from https://stackoverflow.com/a/47196516/4418060
            var coreAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            var coreAssemblyNames = new[] {
                "mscorlib.dll",
                "netstandard.dll",
                "System.dll",
                "System.Core.dll",
#if NETCOREAPP
                "System.Private.CoreLib.dll",
#endif
                "System.Runtime.dll",
            };
            var coreMetaReferences =
                coreAssemblyNames.Select(x => MetadataReference.CreateFromFile(Path.Combine(coreAssemblyPath, x)));
            var otherAssemblies = new[] {
                typeof(CSharpCompilation).Assembly,
                typeof(GenerateHandlerMethodsAttribute).Assembly,
//                typeof(IDebugAdapterClientRegistry).Assembly,
                typeof(Unit).Assembly,
//                typeof(ILanguageServerRegistry).Assembly,
            };
            MetadataReferences = coreMetaReferences
                                .Concat<MetadataReference>(otherAssemblies.Distinct().Select(x => MetadataReference.CreateFromFile(x.Location)))
                                .ToImmutableArray();
        }

        internal const string CrLf = "\r\n";
        internal const string Lf = "\n";
        internal const string DefaultFilePathPrefix = "Test";
        internal const string CSharpDefaultFileExt = "cs";
        internal const string TestProjectName = "TestProject";

        internal static readonly string NormalizedPreamble = NormalizeToLf(Preamble.GeneratedByATool + Lf);

        internal static readonly ImmutableArray<MetadataReference> MetadataReferences;

        public static async Task AssertGeneratedAsExpected<T>(string source, string expected) where T : ISourceGenerator, new()
        {
            var generatedTree = await GenerateAsync<T>(source);
            // normalize line endings to just LF
            var generatedText = NormalizeToLf(generatedTree.GetText().ToString());
            // and append preamble to the expected
            var expectedText = NormalizedPreamble + NormalizeToLf(expected).Trim();
            generatedText.Should().Be(expectedText);
        }

        public static async Task<string> Generate<T>(string source) where T : ISourceGenerator, new()
        {
            var generatedTree = await GenerateAsync<T>(source);
            // normalize line endings to just LF
            var generatedText = NormalizeToLf(generatedTree.GetText().ToString());
            // and append preamble to the expected
            return generatedText;
        }

        public static string NormalizeToLf(string input) => input.Replace(CrLf, Lf);

        public static async Task<SyntaxTree> GenerateAsync<T>(string source) where T : ISourceGenerator, new()
        {
            var document = CreateProject(source).Documents.Single();
            var tree = await document.GetSyntaxTreeAsync();
            if (tree is null)
            {
                throw new InvalidOperationException("Could not get the syntax tree of the sources");
            }

            var compilation = (CSharpCompilation?) await document.Project.GetCompilationAsync();
            if (compilation is null)
            {
                throw new InvalidOperationException("Could not compile the sources");
            }

//            var diagnostics = compilation.GetDiagnostics();
//            Assert.Empty(diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning));

            // TODO: fix this junk
            var optionsProvider = Substitute.For<AnalyzerConfigOptionsProvider>();

            var generator = new T();
            var receiver = new GenerateHandlerMethodsGenerator.SyntaxReceiver();
            var visitor = new NotSureWhatToCallYou(receiver);

            foreach (var compilationSyntaxTree in compilation.SyntaxTrees)
            {
                visitor.Visit(compilationSyntaxTree.GetRoot());
            }

            //, ISyntaxReceiver? syntaxReceiver, DiagnosticBag diagnostics, CancellationToken cancellationToken = default
            var context = (SourceGeneratorContext) Activator.CreateInstance(
                typeof(SourceGeneratorContext),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null,
                new object[] {
                    compilation,
                    ImmutableArray<AdditionalText>.Empty,
                    optionsProvider,
                    // This needs to be generic somehow...
                    receiver,
                    // TODO: dynamic proxy... sigh
                    Activator.CreateInstance(
                        typeof(SourceGeneratorContext).Assembly.GetType("Microsoft.CodeAnalysis.DiagnosticBag")!,
                        Array.Empty<object>()
                    ),
                    CancellationToken.None
                },
                CultureInfo.InvariantCulture
            );

            generator.Execute(context);

            // WARNING JANK AHEAD
            var additionalSourcesProperty = context.GetType().GetProperty("AdditionalSources", BindingFlags.NonPublic | BindingFlags.Instance);
            var additionalSources = additionalSourcesProperty.GetValue(context);
            var toImmutableAndFreeMethod = additionalSources.GetType().GetMethod("ToImmutableAndFree", BindingFlags.NonPublic | BindingFlags.Instance);
            var generatedSourceTextArray = toImmutableAndFreeMethod.Invoke(additionalSources, Array.Empty<object>()) as IEnumerable;
            var generatedSourceTextType = typeof(SourceGeneratorContext).Assembly.GetType("Microsoft.CodeAnalysis.GeneratedSourceText");
            var generatedSourceTextProperty = generatedSourceTextType.GetProperty("Text");
            var sourceTexts = generatedSourceTextArray
                             .OfType<object>()
                             .Select(z => generatedSourceTextProperty.GetValue(z) as SourceText)
                             .ToArray();

            var resultText = sourceTexts.Single();
            return CSharpSyntaxTree.ParseText(resultText);
        }

        public static Project CreateProject(params string[] sources)
        {
            var projectId = ProjectId.CreateNewId(TestProjectName);
            var solution = new AdhocWorkspace()
                          .CurrentSolution
                          .AddProject(projectId, TestProjectName, TestProjectName, LanguageNames.CSharp)
                          .WithProjectCompilationOptions(
                               projectId,
                               new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                           )
                          .WithProjectParseOptions(
                               projectId,
                               new CSharpParseOptions(preprocessorSymbols: new[] { "SOMETHING_ACTIVE" })
                           )
                          .AddMetadataReferences(projectId, MetadataReferences);

            var count = 0;
            foreach (var source in sources)
            {
                var newFileName = DefaultFilePathPrefix + count + "." + CSharpDefaultFileExt;
                var documentId = DocumentId.CreateNewId(projectId, newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            var project = solution.GetProject(projectId);
            if (project is null)
            {
                throw new InvalidOperationException($"The ad hoc workspace does not contain a project with the id {projectId.Id}");
            }

            return project;
        }
    }

    class NotSureWhatToCallYou : CSharpSyntaxWalker
    {
        private readonly ISyntaxReceiver _syntaxReceiver;

        public NotSureWhatToCallYou(ISyntaxReceiver syntaxReceiver)
        {
            _syntaxReceiver = syntaxReceiver;
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node == null) return;
            _syntaxReceiver.OnVisitSyntaxNode(node);
            base.Visit(node);
        }
    }
}

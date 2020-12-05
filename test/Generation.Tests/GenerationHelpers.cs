using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.JsonRpc.Generators;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
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
                typeof(IDebugAdapterClientRegistry).Assembly,
                typeof(Unit).Assembly,
                typeof(JToken).Assembly,
                typeof(ILanguageServerRegistry).Assembly,
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
            var generatedText = NormalizeToLf(generatedTree.GetText().ToString()).Trim();
            // and append preamble to the expected
            var expectedText = NormalizedPreamble + NormalizeToLf(expected).Trim();
//            Assert.Equal(expectedText, generatedText);
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

            var compilation = (CSharpCompilation) (await document.Project.GetCompilationAsync())!;
            if (compilation is null)
            {
                throw new InvalidOperationException("Could not compile the sources");
            }

            var diagnostics = compilation.GetDiagnostics();
//            Assert.Empty(diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning));

            ISourceGenerator generator = new T();

            var driver = CSharpGeneratorDriver.Create(
                ImmutableArray.Create(generator),
                ImmutableArray<AdditionalText>.Empty,
                compilation.SyntaxTrees[0].Options as CSharpParseOptions
            );

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics);
            Assert.Empty(diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning));

            // the syntax tree added by the generator will be the last one in the compilation
            return outputCompilation.SyntaxTrees.Last(z => !z.FilePath.Contains("AssemblyRegistrationOptions") && !z.FilePath.Contains("GeneratedAssemblyJsonRpcHandlers"));
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

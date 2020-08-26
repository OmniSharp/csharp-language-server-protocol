using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using CodeGeneration.Roslyn.Engine;
using FluentAssertions;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Extensions.DebugAdapter.Protocol.Client;
using OmniSharp.Extensions.JsonRpc.Generation;
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
                typeof(CodeGenerationAttributeAttribute).Assembly,
                typeof(GenerateHandlerMethodsAttribute).Assembly,
                typeof(IDebugAdapterClientRegistry).Assembly,
                typeof(Unit).Assembly,
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

        internal static readonly string NormalizedPreamble = NormalizeToLf(DocumentTransform.GeneratedByAToolPreamble + Lf);

        internal static readonly ImmutableArray<MetadataReference> MetadataReferences;

        public static async Task AssertGeneratedAsExpected(string source, string expected)
        {
            var generatedTree = await GenerateAsync(source);
            // normalize line endings to just LF
            var generatedText = NormalizeToLf(generatedTree.GetText().ToString());
            // and append preamble to the expected
            var expectedText = NormalizedPreamble + NormalizeToLf(expected).Trim();
            generatedText.Should().Be(expectedText);
        }

        public static async Task<string> Generate(string source)
        {
            var generatedTree = await GenerateAsync(source);
            // normalize line endings to just LF
            var generatedText = NormalizeToLf(generatedTree.GetText().ToString());
            // and append preamble to the expected
            return generatedText;
        }

        public static string NormalizeToLf(string input) => input.Replace(CrLf, Lf);

        public static async Task<SyntaxTree> GenerateAsync(string source)
        {
            var document = CreateProject(source).Documents.Single();
            var tree = await document.GetSyntaxTreeAsync();
            if (tree is null)
            {
                throw new InvalidOperationException("Could not get the syntax tree of the sources");
            }

            var compilation = (CSharpCompilation) await document.Project.GetCompilationAsync();
            if (compilation is null)
            {
                throw new InvalidOperationException("Could not compile the sources");
            }

            var diagnostics = compilation.GetDiagnostics();
            Assert.Empty(diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning));
            var progress = new Progress<Diagnostic>();
            var result = await DocumentTransform.TransformAsync(compilation, tree, null, Assembly.Load, progress, CancellationToken.None);
            return result;
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
}

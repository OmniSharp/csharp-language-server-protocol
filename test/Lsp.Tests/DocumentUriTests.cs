using System;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class DocumentUriTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DocumentUriTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(DocumentUriTestData.FileSystemPaths))]
        public void Should_Handle_VSCode_Style_File_System_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(DocumentUriTestData.StringUris))]
        public void Should_Handle_VSCode_Style_String_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            new DocumentUri(uri).Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(DocumentUriTestData.Uris))]
        public void Should_Handle_VSCode_Style_Uris(Uri uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.From(uri).Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(DocumentUriTestData.FileSystemToFileUri))]
        public void Should_Normalize_VSCode_Style_FileSystem_Paths(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.FromFileSystemPath(uri).Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(DocumentUriTestData.FileUriToFileSystem))]
        public void Should_Normalize_VSCode_Style_Uris(Uri uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            DocumentUri.GetFileSystemPath(uri).Should().Be(expected.GetFileSystemPath());
        }
    }
}

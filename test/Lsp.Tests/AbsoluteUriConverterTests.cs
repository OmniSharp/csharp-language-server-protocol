using System.Text.Json;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class AbsoluteUriConverterTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AbsoluteUriConverterTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(DocumentUriTests.WindowsPathStringUris))]
        [ClassData(typeof(DocumentUriTests.WindowsPathAltStringUris))]
        [ClassData(typeof(DocumentUriTests.UncPathStringUris))]
        [ClassData(typeof(DocumentUriTests.UnixPathStringUris))]
        public void Should_Deserialize_VSCode_Style_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            JsonSerializer.Deserialize<DocumentUri>($"\"{uri}\"").Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(DocumentUriTests.WindowsPathStringUris))]
        [ClassData(typeof(DocumentUriTests.WindowsPathAltStringUris))]
        [ClassData(typeof(DocumentUriTests.UncPathStringUris))]
        [ClassData(typeof(DocumentUriTests.UnixPathStringUris))]
        public void Should_Serialize_VSCode_Style_Uris(string uri, DocumentUri expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            JsonSerializer.Serialize(new DocumentUri(uri)).Trim('"').Should().Be(expected.ToString());
        }
    }
}

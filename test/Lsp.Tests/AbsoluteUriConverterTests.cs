using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests
{
    public class AbsoluteUriConverterTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AbsoluteUriConverterTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        [Theory]
        [ClassData(typeof(DocumentUriTests.WindowsPathStringUris))]
        [ClassData(typeof(DocumentUriTests.WindowsPathAltStringUris))]
        [ClassData(typeof(DocumentUriTests.UncPathStringUris))]
        [ClassData(typeof(DocumentUriTests.UnixPathStringUris))]
        public void Should_Deserialize_VSCode_Style_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            var serializer = new LspSerializer();
            serializer.DeserializeObject<DocumentUri>($"\"{uri}\"").ToString().Should().Be(expected);
        }

        [Theory]
        [ClassData(typeof(DocumentUriTests.WindowsPathStringUris))]
        [ClassData(typeof(DocumentUriTests.WindowsPathAltStringUris))]
        [ClassData(typeof(DocumentUriTests.UncPathStringUris))]
        [ClassData(typeof(DocumentUriTests.UnixPathStringUris))]
        public void Should_Serialize_VSCode_Style_Uris(string uri, string expected)
        {
            _testOutputHelper.WriteLine($"Given: {uri}");
            _testOutputHelper.WriteLine($"Expected: {expected}");
            var serializer = new LspSerializer();
            serializer.SerializeObject(DocumentUri.Parse(uri)).Trim('"').Should().Be(expected);
        }
    }
}

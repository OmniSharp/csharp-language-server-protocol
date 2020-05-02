using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using Xunit;

namespace Lsp.Tests
{
    public class DocumentUriTests
    {
        [Theory]
        [InlineData("file:///c%3A/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("file:///c%3a/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("c%3A\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs",
            "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs")]
        [InlineData("c%3a\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs",
            "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs")]
        [InlineData("c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs",
            "c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs")]
        public void Should_Handle_VSCode_Style_Uris(string uri, string expected)
        {
            DocumentUri.NormalizePath(uri).Should().Be(expected);
        }

        [Theory]
        [InlineData("c%3A\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("c%3a\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("c:\\Users\\mb\\src\\gh\\Cake.Json\\src\\Cake.Json\\Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        public void Should_Handle_VSCode_Style_Paths(string uri, string expected)
        {
            DocumentUri.FromFileSystemPath(uri).Should().Be(new Uri(expected));
        }
    }

    public class AbsoluteUriConverterTests
    {
        [Theory]
        [InlineData("file:///c%3A/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("file:///c%3a/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        public void Should_Deserialize_VSCode_Style_Uris(string uri, string expected)
        {
            var serializer = new JsonSerializerSettings() {
                Converters = {new AbsoluteUriConverter()}
            };
            JsonConvert.DeserializeObject<Uri>($"\"{uri}\"", serializer).Should().Be(expected);
        }
        
        [Theory]
        [InlineData("file:///c%3A/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("file:///c%3a/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        [InlineData("file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs",
            "file:///c:/Users/mb/src/gh/Cake.Json/src/Cake.Json/Namespaces.cs")]
        public void Should_Serialize_VSCode_Style_Uris(string uri, string expected)
        {
            var serializer = new JsonSerializerSettings() {
                Converters = {new AbsoluteUriConverter()}
            };
            JsonConvert.SerializeObject(new Uri(uri), serializer).Trim('"').Should().Be(expected);
        }
    }
}

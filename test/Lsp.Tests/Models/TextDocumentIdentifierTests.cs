using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentIdentifierTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentIdentifier(new Uri("file:///abc/123/d.cs"));
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<TextDocumentIdentifier>(expected);
            deresult.Should().BeEquivalentTo(model);
        }

        [Fact]
        public void Should_Fail_To_Serialize_When_Given_A_Non_Relative_Uri()
        {
            var serializer = new Serializer(ClientVersion.Lsp3);
            var model = new TextDocumentIdentifier()
            {
                Uri = new Uri("./abc23.cs", UriKind.Relative),
            };

            Action a = () => serializer.SerializeObject(model);
            a.Should().Throw<JsonSerializationException>();
        }

        [Fact]
        public void Should_Fail_To_Deserialize_When_Given_A_Non_Relative_Uri()
        {
            var serializer = new Serializer(ClientVersion.Lsp3);
            var json = @"{
                ""uri"":""./0b0jnxg2.kgh.ps1""
            }";

            Action a = () => serializer.DeserializeObject<TextDocumentIdentifier>(json);
            a.Should().Throw<JsonSerializationException>();
        }

        [Fact]
        public void Should_Deserialize_For_Example_Value()
        {
            var serializer = new Serializer(ClientVersion.Lsp3);
            var result = serializer.DeserializeObject<TextDocumentIdentifier>(@"{
                ""uri"":""file:///Users/tyler/Code/PowerShell/vscode/PowerShellEditorServices/test/PowerShellEditorServices.Test.E2E/bin/Debug/netcoreapp3.1/0b0jnxg2.kgh.ps1""
            }");

            result.Uri.Should().Be(new Uri("file:///Users/tyler/Code/PowerShell/vscode/PowerShellEditorServices/test/PowerShellEditorServices.Test.E2E/bin/Debug/netcoreapp3.1/0b0jnxg2.kgh.ps1", UriKind.Absolute));
        }
    }
}

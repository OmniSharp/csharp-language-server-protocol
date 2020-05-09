using System;
using FluentAssertions;
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
        public void Should_Deserialize_For_Example_Value()
        {
            var serializer = new Serializer(ClientVersion.Lsp3);
            var result = serializer.DeserializeObject<TextDocumentIdentifier>(@"{
                ""uri"":""file:///Users/tyler/Code/PowerShell/vscode/PowerShellEditorServices/test/PowerShellEditorServices.Test.E2E/bin/Debug/netcoreapp3.1/0b0jnxg2.kgh.ps1""
            }");

            result.Uri.Should().Be(new DocumentUri("file:///Users/tyler/Code/PowerShell/vscode/PowerShellEditorServices/test/PowerShellEditorServices.Test.E2E/bin/Debug/netcoreapp3.1/0b0jnxg2.kgh.ps1"));
        }
    }
}

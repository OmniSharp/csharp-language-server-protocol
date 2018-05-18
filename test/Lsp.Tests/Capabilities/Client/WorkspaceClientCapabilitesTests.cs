using System;
using FluentAssertions;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class WorkspaceClientCapabilitiesTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new WorkspaceClientCapabilities() {
                ApplyEdit = true,
                WorkspaceEdit = new WorkspaceEditCapability() { DocumentChanges = true },
                DidChangeConfiguration = new DidChangeConfigurationCapability() { DynamicRegistration = true },
                DidChangeWatchedFiles = new DidChangeWatchedFilesCapability() { DynamicRegistration = true },
                ExecuteCommand = new ExecuteCommandCapability() { DynamicRegistration = true },
                Symbol = new WorkspaceSymbolCapability() { DynamicRegistration = true },
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<WorkspaceClientCapabilities>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void EmptyTest(string expected)
        {
            var model = new WorkspaceClientCapabilities();

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = new Serializer(ClientVersion.Lsp3).DeserializeObject<WorkspaceClientCapabilities>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

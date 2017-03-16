using System;
using FluentAssertions;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class WorkspaceClientCapabilitesTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new WorkspaceClientCapabilites() {
                ApplyEdit = true,
                DidChangeConfiguration = new DynamicCapability() { DynamicRegistration = true },
                DidChangeWatchedFiles = new DynamicCapability() { DynamicRegistration = true },
                ExecuteCommand = new DynamicCapability() { DynamicRegistration = true },
                Symbol = new DynamicCapability() { DynamicRegistration = true },
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<WorkspaceClientCapabilites>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}

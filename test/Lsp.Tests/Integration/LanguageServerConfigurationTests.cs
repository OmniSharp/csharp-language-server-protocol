using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class LanguageServerConfigurationTests : LanguageProtocolTestBase
    {
        public LanguageServerConfigurationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Not_Support_Configuration_It_Not_Configured()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, o => {});
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string>() {["key"] = "value"});
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration.AsEnumerable().Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Update_Configuration_On_Server()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string>() {["key"] = "value"});
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
        }

        [Fact]
        public async Task Should_Update_Scoped_Configuration()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            var scopedConfiguration = await server.Configuration.GetScopedConfiguration(DocumentUri.From("/my/file.cs"));

            configuration.Update("mysection", new Dictionary<string, string>() {["key"] = "value"});
            await server.Configuration.WaitForChange(CancellationToken);
            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>() {["key"] = "scopedvalue"});
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            scopedConfiguration["mysection:key"].Should().Be("scopedvalue");
        }

        [Fact]
        public async Task Should_Fallback_To_Original_Configuration()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            var scopedConfiguration = await server.Configuration.GetScopedConfiguration(DocumentUri.From("/my/file.cs"));

            configuration.Update("mysection", new Dictionary<string, string>() {["key"] = "value"});
            await server.Configuration.WaitForChange(CancellationToken);
            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>() {["key"] = "scopedvalue"});
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            scopedConfiguration["mysection:key"].Should().Be("scopedvalue");

            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>() {});
            await scopedConfiguration.WaitForChange(CancellationToken);

            scopedConfiguration["mysection:key"].Should().Be("value");
        }

        [Fact]
        public async Task Should_Only_Update_Configuration_Items_That_Are_Defined()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string>() {["key"] = "value"});
            await server.Configuration.WaitForChange(CancellationToken);
            configuration.Update("notmysection", new Dictionary<string, string>() {["key"] = "value"});
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["notmysection:key"].Should().BeNull();
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
        }

        private void ConfigureServer(LanguageServerOptions options)
        {
            options.WithConfigurationSection("mysection");
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.Extensions;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
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
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, o => { });
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration.AsEnumerable().Should().BeEmpty();
        }

        [Fact]
        public async Task Should_Update_Configuration_On_Server()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().Be("key");
        }

        [Fact]
        public async Task Should_Update_Configuration_On_Server_After_Starting()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, options => {});
            server.Configuration.AsEnumerable().Should().BeEmpty();
            server.Configuration.AddSection("mysection", "othersection");

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().Be("key");
        }

        [Fact]
        public async Task Should_Update_Configuration_Should_Stop_Watching_Sections()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().Be("key");

            server.Configuration.RemoveSection("othersection");
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().BeNull();
        }

        [Fact]
        public async Task Should_Update_Scoped_Configuration()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            var scopedConfiguration = await server.Configuration.GetScopedConfiguration(DocumentUri.From("/my/file.cs"), CancellationToken);

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["key"] = "scopedvalue" });
            configuration.Update("othersection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["value"] = "scopedkey" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            scopedConfiguration["mysection:key"].Should().Be("scopedvalue");
            server.Configuration["othersection:value"].Should().Be("key");
            scopedConfiguration["othersection:value"].Should().Be("scopedkey");
        }

        [Fact]
        public async Task Should_Fallback_To_Original_Configuration()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            var scopedConfiguration = await server.Configuration.GetScopedConfiguration(DocumentUri.From("/my/file.cs"), CancellationToken);

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["key"] = "scopedvalue" });
            configuration.Update("othersection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["value"] = "scopedkey" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            scopedConfiguration["mysection:key"].Should().Be("scopedvalue");
            server.Configuration["othersection:value"].Should().Be("key");
            scopedConfiguration["othersection:value"].Should().Be("scopedkey");

            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>());
            configuration.Update("othersection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>());
            await scopedConfiguration.WaitForChange(CancellationToken);

            await Task.Delay(2000);

            scopedConfiguration["mysection:key"].Should().Be("value");
            scopedConfiguration["othersection:value"].Should().Be("key");
        }

        [Fact]
        public async Task Should_Only_Update_Configuration_Items_That_Are_Defined()
        {
            var (client, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            await server.Configuration.WaitForChange(CancellationToken);
            configuration.Update("notmysection", new Dictionary<string, string> { ["key"] = "value" });
            await server.Configuration.WaitForChange(CancellationToken);

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["notmysection:key"].Should().BeNull();
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
        }

        private void ConfigureServer(LanguageServerOptions options) => options.WithConfigurationSection("mysection").WithConfigurationSection("othersection");
    }
}

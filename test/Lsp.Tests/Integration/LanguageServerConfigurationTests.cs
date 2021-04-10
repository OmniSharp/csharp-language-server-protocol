using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.Extensions;
using NSubstitute.ReceivedExtensions;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using Serilog.Events;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class LanguageServerConfigurationTests : LanguageProtocolTestBase
    {
        public LanguageServerConfigurationTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [RetryFact]
        public async Task Should_Not_Support_Configuration_It_Not_Configured()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, o => { });
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration.AsEnumerable().Should().BeEmpty();
        }

        [RetryFact]
        public async Task Should_Allow_Null_Response()
        {
            var (client, server) = await Initialize(
                options => {
                    options.WithCapability(new DidChangeConfigurationCapability());
                    options.OnConfiguration(@params => Task.FromResult(new Container<JToken>(@params.Items.Select(z => (JToken?)null))));
                    ConfigureClient(options);
                }, ConfigureServer);

            Func<Task> a = () => server.Configuration.GetConfiguration(new ConfigurationItem() { Section = "mysection" });
            a.Should().NotThrow();
        }

        [RetryFact]
        public async Task Should_Update_Configuration_On_Server()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().Be("key");
        }

        [RetryFact]
        public async Task Should_Update_Configuration_On_Server_After_Starting()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, options => {});
            server.Configuration.AsEnumerable().Should().BeEmpty();
            server.Configuration.AddSection("mysection", "othersection");

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().Be("key");
        }

        [RetryFact]
        public async Task Should_Update_Configuration_Should_Stop_Watching_Sections()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().Be("key");

            server.Configuration.RemoveSection("othersection");
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["othersection:value"].Should().BeNull();
        }

        [RetryFact]
        public async Task Should_Update_Scoped_Configuration()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            var scopedConfiguration = await server.Configuration.GetScopedConfiguration(DocumentUri.From("/my/file.cs"), CancellationToken);

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["key"] = "scopedvalue" });
            configuration.Update("othersection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["value"] = "scopedkey" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            scopedConfiguration["mysection:key"].Should().Be("scopedvalue");
            server.Configuration["othersection:value"].Should().Be("key");
            scopedConfiguration["othersection:value"].Should().Be("scopedkey");
        }

        [RetryFact]
        public async Task Should_Fallback_To_Original_Configuration()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            var scopedConfiguration = await server.Configuration.GetScopedConfiguration(DocumentUri.From("/my/file.cs"), CancellationToken);

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            configuration.Update("othersection", new Dictionary<string, string> { ["value"] = "key" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["key"] = "scopedvalue" });
            configuration.Update("othersection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string> { ["value"] = "scopedkey" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            scopedConfiguration["mysection:key"].Should().Be("scopedvalue");
            server.Configuration["othersection:value"].Should().Be("key");
            scopedConfiguration["othersection:value"].Should().Be("scopedkey");

            configuration.Update("mysection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>());
            configuration.Update("othersection", DocumentUri.From("/my/file.cs"), new Dictionary<string, string>());
            await scopedConfiguration.WaitForChange(CancellationToken);
            await SettleNext();

            await TestHelper.DelayUntil(() => scopedConfiguration["mysection:key"] == "value", CancellationToken);

            scopedConfiguration["mysection:key"].Should().Be("value");
            scopedConfiguration["othersection:value"].Should().Be("key");
        }

        [RetryFact]
        public async Task Should_Only_Update_Configuration_Items_That_Are_Defined()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);
            server.Configuration.AsEnumerable().Should().BeEmpty();

            configuration.Update("mysection", new Dictionary<string, string> { ["key"] = "value" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            configuration.Update("notmysection", new Dictionary<string, string> { ["key"] = "value" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            server.Configuration["mysection:key"].Should().Be("value");
            server.Configuration["notmysection:key"].Should().BeNull();
        }

        [RetryFact]
        public async Task Should_Support_Configuration_Binding()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, ConfigureServer);

            configuration.Update("mysection", new Dictionary<string, string> { ["host"] = "localhost", ["port"] = "443" });
            configuration.Update("notmysection", new Dictionary<string, string> { ["host"] = "127.0.0.1", ["port"] = "123" });
            await TestHelper.DelayUntil(() => server.Configuration.AsEnumerable().Any(z => z.Key.StartsWith("mysection")), CancellationToken);

            var data = new BinderSourceUrl();
            server.Configuration.Bind("mysection", data);
            data.Host.Should().Be("localhost");
            data.Port.Should().Be(443);

            configuration.Update("mysection", new Dictionary<string, string> { ["host"] = "127.0.0.1", ["port"] = "80" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            data = new BinderSourceUrl();
            server.Configuration.Bind("mysection", data);
            data.Host.Should().Be("127.0.0.1");
            data.Port.Should().Be(80);
        }

        [RetryFact]
        public async Task Should_Support_Options()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, options => {
                ConfigureServer(options);
                options.Services.Configure<BinderSourceUrl>("mysection");
            });

            configuration.Update("mysection", new Dictionary<string, string> { ["host"] = "localhost", ["port"] = "443" });
            configuration.Update("notmysection", new Dictionary<string, string> { ["host"] = "127.0.0.1", ["port"] = "123" });
            await TestHelper.DelayUntil(() => server.Configuration.AsEnumerable().Any(z => z.Key.StartsWith("mysection")), CancellationToken);

            //
            var options = server.GetService<IOptions<BinderSourceUrl>>();
            options.Value.Host.Should().Be("localhost");
            options.Value.Port.Should().Be(443);

            configuration.Update("mysection", new Dictionary<string, string> { ["host"] = "127.0.0.1", ["port"] = "80" });
            await server.Configuration.WaitForChange(CancellationToken);
            await SettleNext();

            options.Value.Host.Should().Be("localhost");
            options.Value.Port.Should().Be(443);
        }

        [RetryFact]
        public async Task Should_Support_Options_Monitor()
        {
            var (_, server, configuration) = await InitializeWithConfiguration(ConfigureClient, options => {
                ConfigureServer(options);
                options.Services.Configure<BinderSourceUrl>("mysection");
            });

            var options = server.GetService<IOptionsMonitor<BinderSourceUrl>>();
            var sub = Substitute.For<Action<BinderSourceUrl>>();
            options.OnChange(sub);

            configuration.Update("mysection", new Dictionary<string, string> { ["host"] = "localhost", ["port"] = "443" });
            configuration.Update("notmysection", new Dictionary<string, string> { ["host"] = "127.0.0.1", ["port"] = "123" });
            await options.WaitForChange(CancellationToken);
            await SettleNext();

            // IOptionsMonitor<> is registered as a singleton, so this will update
            options.CurrentValue.Host.Should().Be("localhost");
            options.CurrentValue.Port.Should().Be(443);
            sub.Received(1).Invoke(Arg.Any<BinderSourceUrl>());

            configuration.Update("mysection", new Dictionary<string, string> { ["host"] = "127.0.0.1", ["port"] = "80" });
            await options.WaitForChange(CancellationToken);
            await SettleNext();

            options.CurrentValue.Host.Should().Be("127.0.0.1");
            options.CurrentValue.Port.Should().Be(80);
            sub.Received(2).Invoke(Arg.Any<BinderSourceUrl>());
        }

        class BinderSourceUrl
        {
            public string Host { get; set; }
            public int Port { get; set; }
        }

        private void ConfigureClient(LanguageClientOptions options)
        {
        }

        private void ConfigureServer(LanguageServerOptions options) => options.WithConfigurationSection("mysection").WithConfigurationSection("othersection");
    }
}

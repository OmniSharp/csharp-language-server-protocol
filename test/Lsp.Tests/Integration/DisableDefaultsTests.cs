using System;
using System.Collections.Generic;
using System.Composition;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.Exceptions;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Shared;
using TestingUtils;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class DisableDefaultsTests : LanguageProtocolTestBase
    {
        public DisableDefaultsTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper))
        {
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public async Task Should_Disable_Registration_Manager(bool enabled)
        {
            var (client, _) = await Initialize(
                options => options.UseDefaultRegistrationManager(enabled),
                options => { }
            );

            var clientManager = client.Services.GetRequiredService<SharedHandlerCollection>();
            clientManager.ContainsHandler(typeof(IRegisterCapabilityHandler)).Should().Be(enabled);
            clientManager.ContainsHandler(typeof(IUnregisterCapabilityHandler)).Should().Be(enabled);
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public async Task Should_Disable_Workspace_Folder_Manager(bool enabled)
        {
            var (client, server) = await Initialize(
                options => options.UseDefaultWorkspaceFolderManager(enabled),
                options => options.UseDefaultWorkspaceFolderManager(enabled)
            );

            var clientManager = client.Services.GetRequiredService<SharedHandlerCollection>();
            clientManager.ContainsHandler(typeof(IWorkspaceFoldersHandler)).Should().Be(enabled);

            var serverManager = server.Services.GetRequiredService<SharedHandlerCollection>();
            serverManager.ContainsHandler(typeof(IDidChangeWorkspaceFoldersHandler)).Should().Be(enabled);
        }

        [Fact]
        public async Task Should_Allow_Custom_Workspace_Folder_Manager_Delegate()
        {
            var action = Substitute.For<Action<DidChangeWorkspaceFoldersParams>>();
            var (client, server) = await Initialize(
                options => {},
                options => options
                          .UseDefaultWorkspaceFolderManager(false)
                          .OnDidChangeWorkspaceFolders(action, new object())
            );

            var config = client.Services.GetRequiredService<TestConfigurationProvider>();
            config.Update("mysection", new Dictionary<string, string>() { ["data"] = "value" });

            client.WorkspaceFoldersManager.Add(new WorkspaceFolder() { Name = "foldera", Uri = "/some/path" });

            await TestHelper.DelayUntil(
                () => {
                    try
                    {
                        action.Received(1).Invoke(Arg.Any<DidChangeWorkspaceFoldersParams>());
                        return true;
                    }
                    catch (ReceivedCallsException e)
                    {
                        return false;
                    }
                }, CancellationToken
            );
        }

        [Fact]
        public async Task Should_Allow_Custom_Workspace_Folder_Manager_Delegate_Without_Disabling_Default_Handlers()
        {
            var action = Substitute.For<Action<DidChangeWorkspaceFoldersParams>>();
            var (client, server) = await Initialize(
                options => {},
                options => options
                          .OnDidChangeWorkspaceFolders(action, new object())
            );

            var config = client.Services.GetRequiredService<TestConfigurationProvider>();
            config.Update("mysection", new Dictionary<string, string>() { ["data"] = "value" });

            client.WorkspaceFoldersManager.Add(new WorkspaceFolder() { Name = "foldera", Uri = "/some/path" });

            await TestHelper.DelayUntil(
                () => {
                    try
                    {
                        action.Received(1).Invoke(Arg.Any<DidChangeWorkspaceFoldersParams>());
                        return true;
                    }
                    catch (ReceivedCallsException e)
                    {
                        return false;
                    }
                }, CancellationToken
            );
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public async Task Should_Disable_Configuration(bool enabled)
        {
            var (_, server) = await Initialize(
                options => { },
                options => options.UseDefaultServerConfiguration(enabled)
            );

            var serverManager = server.Services.GetRequiredService<SharedHandlerCollection>();
            serverManager.ContainsHandler(typeof(IDidChangeConfigurationHandler)).Should().Be(enabled);
        }

        [Fact]
        public async Task Should_Allow_Custom_Configuration_Delegate()
        {
            var action = Substitute.For<Action<DidChangeConfigurationParams>>();
            var (client, server) = await Initialize(
                options => options
                          .WithCapability(new DidChangeConfigurationCapability() { DynamicRegistration = true })
                          .WithServices(z => z.AddSingleton<TestConfigurationProvider>()),
                options => options
                          .UseDefaultServerConfiguration(false)
                          .WithConfigurationSection("mysection")
                          .OnDidChangeConfiguration(action, new object())
            );

            var clientManager = client.Services.GetRequiredService<SharedHandlerCollection>();
            clientManager.ContainsHandler(typeof(IConfigurationHandler)).Should().BeTrue();

            var serverManager = server.Services.GetRequiredService<SharedHandlerCollection>();
            serverManager.ContainsHandler(typeof(IDidChangeConfigurationHandler)).Should().BeTrue();

            var config = client.Services.GetRequiredService<TestConfigurationProvider>();
            config.Update("mysection", new Dictionary<string, string>() { ["data"] = "value" });

            await TestHelper.DelayUntil(
                () => {
                    try
                    {
                        action.Received(1).Invoke(Arg.Is<DidChangeConfigurationParams>(z => Equals(z.Settings, JValue.CreateNull())));
                        return true;
                    }
                    catch (ReceivedCallsException e)
                    {
                        return false;
                    }
                }, CancellationToken
            );
        }

        [Fact]
        public async Task Should_Allow_Custom_Configuration_Without_Disabling_Default_Handlers()
        {
            var action = Substitute.For<Action<DidChangeConfigurationParams>>();
            var (client, server) = await Initialize(
                options => options
                          .WithCapability(
                               new DidChangeConfigurationCapability() {
                                   DynamicRegistration = true
                               }
                           )
                          .WithServices(z => z.AddSingleton<TestConfigurationProvider>()),
                options => options
                          .WithConfigurationSection("mysection")
                          .OnDidChangeConfiguration(action, new object())
            );

            var clientManager = client.Services.GetRequiredService<SharedHandlerCollection>();
            clientManager.ContainsHandler(typeof(IConfigurationHandler)).Should().BeTrue();
            var config = client.Services.GetRequiredService<TestConfigurationProvider>();
            config.Update("mysection", new Dictionary<string, string>() { ["data"] = "value" });

            await TestHelper.DelayUntil(
                () => {
                    try
                    {
                        action.Received(1).Invoke(Arg.Is<DidChangeConfigurationParams>(z => Equals(z.Settings, JValue.CreateNull())));
                        return true;
                    }
                    catch (ReceivedCallsException e)
                    {
                        return false;
                    }
                }, CancellationToken
            );
        }
    }
}

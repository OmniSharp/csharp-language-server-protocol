using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Tests.Integration
{
    public class ExecuteCommandTests : LanguageProtocolTestBase
    {
        public ExecuteCommandTests(ITestOutputHelper outputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(outputHelper, LogEventLevel.Verbose))
        {
        }

        [Fact]
        public async Task Should_Execute_A_Command()
        {
            var command = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(x => {
                        return Task.FromResult(new CompletionList(new CompletionItem() {
                            Command = new Command() {
                                Name = "execute-a",
                                Arguments = JArray.FromObject(new object[] { 1, "2", false })
                            }
                        }));
                    }, new CompletionRegistrationOptions() {
                    });

                    options.OnExecuteCommand(command, new ExecuteCommandRegistrationOptions() {
                        Commands = new Container<string>("execute-a")
                    });
                });

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            await client.ExecuteCommand(item.Command);

            await command.Received(1).Invoke(Arg.Any<ExecuteCommandParams>());
        }

        [Fact]
        public async Task Should_Execute_The_Correct_Command()
        {
            var commanda = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var commandb = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(x => {
                        return Task.FromResult(new CompletionList(new CompletionItem() {
                            Command = new Command() {
                                Name = "execute-b",
                                Arguments = JArray.FromObject(new object[] { 1, "2", false })
                            }
                        }));
                    }, new CompletionRegistrationOptions() {
                    });

                    options.OnExecuteCommand(commanda, new ExecuteCommandRegistrationOptions() {
                        Commands = new Container<string>("execute-a")
                    });

                    options.OnExecuteCommand(commandb, new ExecuteCommandRegistrationOptions() {
                        Commands = new Container<string>("execute-b")
                    });
                });

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            await client.ExecuteCommand(item.Command);

            await commanda.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
            await commandb.Received(1).Invoke(Arg.Any<ExecuteCommandParams>());
        }

        [Fact]
        public async Task Should_Fail_To_Execute_A_Command_When_No_Command_Is_Defined()
        {
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(x => {
                        return Task.FromResult(new CompletionList(new CompletionItem() {
                            Command = new Command() {
                                Name = "execute-a",
                                Arguments = JArray.FromObject(new object[] { 1, "2", false })
                            }
                        }));
                    }, new CompletionRegistrationOptions() {
                    });
                });

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            Func<Task> action = () => client.ExecuteCommand(item.Command);
            await action.Should().ThrowAsync<MethodNotSupportedException>();
        }

        [Fact]
        public async Task Should_Fail_To_Execute_A_Command_When_No_Command_Name_Is_Given()
        {
            var command = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(x => {
                        return Task.FromResult(new CompletionList(new CompletionItem() {
                            Command = new Command() {
                                Arguments = JArray.FromObject(new object[] { 1, "2", false })
                            }
                        }));
                    }, new CompletionRegistrationOptions() {
                    });

                    options.OnExecuteCommand(command, new ExecuteCommandRegistrationOptions() {
                        Commands = new Container<string>("execute-a")
                    });
                });

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            Func<Task> action = () => client.ExecuteCommand(item.Command);
            await action.Should().ThrowAsync<MethodNotSupportedException>();

            await command.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
        }

        [Fact]
        public async Task Should_Fail_To_Execute_A_Command()
        {
            var commandc = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var commandb = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var (client, server) = await Initialize(
                options => { }, options => {
                    options.OnCompletion(x => {
                        return Task.FromResult(new CompletionList(new CompletionItem() {
                            Command = new Command() {
                                Name = "execute-a",
                                Arguments = JArray.FromObject(new object[] {1, "2", false})
                            }
                        }));
                    }, new CompletionRegistrationOptions() {
                    });

                    options.OnExecuteCommand(commandb, new ExecuteCommandRegistrationOptions() {
                        Commands = new Container<string>("execute-b")
                    });

                    options.OnExecuteCommand(commandc, new ExecuteCommandRegistrationOptions() {
                        Commands = new Container<string>("execute-c")
                    });
                });

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            Func<Task> action = () => client.ExecuteCommand(item.Command);
            await action.Should().ThrowAsync<MethodNotSupportedException>();

            await commandc.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
            await commandb.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
        }
    }
}

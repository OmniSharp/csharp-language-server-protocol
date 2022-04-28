using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Lsp.Integration.Tests
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
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1, "2", false)
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand(
                        command, (_, _) => new ExecuteCommandRegistrationOptions
                        {
                            Commands = new Container<string>("execute-a")
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            await client.ExecuteCommand(item.Command!);

            await command.Received(1).Invoke(Arg.Any<ExecuteCommandParams>());
        }

        [Fact]
        public async Task Should_Execute_The_Correct_Command()
        {
            var commanda = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var commandb = Substitute.For<Func<ExecuteCommandParams, ExecuteCommandCapability, CancellationToken, Task>>();
            var (client, _) = await Initialize(
                options =>
                {
                    options.WithCapability(
                        new ExecuteCommandCapability
                        {
                            DynamicRegistration = false
                        }
                    );
                }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-b", 1, "2", false)
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand(
                        commanda, (_, _) => new ExecuteCommandRegistrationOptions
                        {
                            Commands = new Container<string>("execute-a")
                        }
                    );

                    options.OnExecuteCommand(
                        commandb, (_, _) => new ExecuteCommandRegistrationOptions
                        {
                            Commands = new Container<string>("execute-b")
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            await client.ExecuteCommand(item.Command!);

            await commanda.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
            await commandb.Received(1).Invoke(Arg.Any<ExecuteCommandParams>(), Arg.Any<ExecuteCommandCapability>(), Arg.Any<CancellationToken>());
            var arg = commandb.ReceivedCalls().Single().GetArguments()[1];
            arg.Should().BeOfType<ExecuteCommandCapability>();
        }

        [Fact]
        public async Task Should_Fail_To_Execute_A_Command_When_No_Command_Is_Defined()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1, "2", false)
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().ThrowAsync<MethodNotSupportedException>();
        }

        [Fact]
        public async Task Should_Fail_To_Execute_A_Command_When_No_Command_Name_Is_Given()
        {
            var command = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = new Command
                                        {
                                            Arguments = JArray.FromObject(new object[] { 1, "2", false })
                                        }
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand(
                        command, (_, _) => new ExecuteCommandRegistrationOptions
                        {
                            Commands = new Container<string>("execute-a")
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().ThrowAsync<MethodNotSupportedException>();

            await command.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
        }

        [Fact]
        public async Task Should_Fail_To_Execute_A_Command()
        {
            var commandc = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var commandb = Substitute.For<Func<ExecuteCommandParams, Task>>();
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1, "2", false)
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand(
                        commandb, (_, _) => new ExecuteCommandRegistrationOptions
                        {
                            Commands = new Container<string>("execute-b")
                        }
                    );

                    options.OnExecuteCommand(
                        commandc, (_, _) => new ExecuteCommandRegistrationOptions
                        {
                            Commands = new Container<string>("execute-c")
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().ThrowAsync<MethodNotSupportedException>();

            await commandc.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
            await commandb.Received(0).Invoke(Arg.Any<ExecuteCommandParams>());
        }

        [Fact]
        public async Task Should_Execute_1_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1)
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int>(
                        "execute-a", i =>
                        {
                            i.Should().Be(1);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_2_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1, "2")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string>(
                        "execute-a", (i, s) =>
                        {
                            i.Should().Be(1);
                            s.Should().Be("2");

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_3_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1, "2", true)
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool>(
                        "execute-a", (i, s, arg3) =>
                        {
                            i.Should().Be(1);
                            s.Should().Be("2");
                            arg3.Should().BeTrue();

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_4_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a", 1, "2", true, new Range(( 0, 1 ), ( 1, 1 )))
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range>(
                        "execute-a", (i, s, arg3, arg4) =>
                        {
                            i.Should().Be(1);
                            s.Should().Be("2");
                            arg3.Should().BeTrue();
                            arg4.Should().Be(new Range(( 0, 1 ), ( 1, 1 )));

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_5_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create(
                                            "execute-a", 1, "2", true, new Range(( 0, 1 ), ( 1, 1 )),
                                            new Dictionary<string, string> { ["a"] = "123", ["b"] = "456" }
                                        )
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range, Dictionary<string, string>>(
                        "execute-a", (i, s, arg3, arg4, arg5) =>
                        {
                            i.Should().Be(1);
                            s.Should().Be("2");
                            arg3.Should().BeTrue();
                            arg4.Should().Be(new Range(( 0, 1 ), ( 1, 1 )));
                            arg5.Should().ContainKeys("a", "b");

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_6_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create(
                                            "execute-a", 1, "2", true, new Range(( 0, 1 ), ( 1, 1 )),
                                            new Dictionary<string, string> { ["a"] = "123", ["b"] = "456" },
                                            Guid.NewGuid()
                                        )
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range, Dictionary<string, string>, Guid>(
                        "execute-a", (i, s, arg3, arg4, arg5, arg6) =>
                        {
                            i.Should().Be(1);
                            s.Should().Be("2");
                            arg3.Should().BeTrue();
                            arg4.Should().Be(new Range(( 0, 1 ), ( 1, 1 )));
                            arg5.Should().ContainKeys("a", "b");
                            arg6.Should().NotBeEmpty();

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_1_With_Missing_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int>(
                        "execute-a", i =>
                        {
                            i.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_2_With_Missing_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string>(
                        "execute-a", (i, s) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_3_With_Missing_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool>(
                        "execute-a", (i, s, arg3) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_4_With_Missing_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range>(
                        "execute-a", (i, s, arg3, arg4) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);
                            arg4.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_5_With_Missing_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range, Dictionary<string, string>>(
                        "execute-a", (i, s, arg3, arg4, arg5) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);
                            arg4.Should().Be(default);
                            arg5.Should().BeNull();

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_6_With_Missing_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range, Dictionary<string, string>, Guid>(
                        "execute-a", (i, s, arg3, arg4, arg5, arg6) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);
                            arg4.Should().Be(default);
                            arg5.Should().BeNull();
                            arg6.Should().BeEmpty();

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_1_Null_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int>(
                        "execute-a", i =>
                        {
                            i.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_2_Null_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string>(
                        "execute-a", (i, s) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_3_Null_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool>(
                        "execute-a", (i, s, arg3) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_4_Null_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range>(
                        "execute-a", (i, s, arg3, arg4) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);
                            arg4.Should().Be(default);

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_5_Null_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range, Dictionary<string, string>>(
                        "execute-a", (i, s, arg3, arg4, arg5) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);
                            arg4.Should().Be(default);
                            arg5.Should().BeNull();

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Should_Execute_6_Null_Args()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.OnCompletion(
                        x =>
                        {
                            return Task.FromResult(
                                new CompletionList(
                                    new CompletionItem
                                    {
                                        Command = Command.Create("execute-a")
                                    }
                                )
                            );
                        }, (_, _) => new CompletionRegistrationOptions()
                    );

                    options.OnExecuteCommand<int, string, bool, Range, Dictionary<string, string>, Guid>(
                        "execute-a", (i, s, arg3, arg4, arg5, arg6) =>
                        {
                            i.Should().Be(default);
                            s.Should().Be(default);
                            arg3.Should().Be(default);
                            arg4.Should().Be(default);
                            arg5.Should().BeNull();
                            arg6.Should().BeEmpty();

                            return Task.CompletedTask;
                        }
                    );
                }
            );

            var items = await client.RequestCompletion(new CompletionParams());

            var item = items.Items.Single();

            item.Command.Should().NotBeNull();

            var action = () => client.ExecuteCommand(item.Command!);
            await action.Should().NotThrowAsync();
        }
    }
}

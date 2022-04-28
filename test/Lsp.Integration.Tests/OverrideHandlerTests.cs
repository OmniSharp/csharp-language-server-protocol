using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Newtonsoft.Json.Linq;
using NSubstitute;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Testing;
using OmniSharp.Extensions.LanguageProtocol.Testing;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using Xunit;
using Xunit.Abstractions;

namespace Lsp.Integration.Tests
{
    public class OverrideHandlerTests : LanguageProtocolTestBase
    {
        public OverrideHandlerTests(ITestOutputHelper testOutputHelper) : base(new JsonRpcTestOptions().ConfigureForXUnit(testOutputHelper))
        {
        }

        [Fact]
        public async Task Should_Support_Custom_Execute_Command_Handlers()
        {
            var (client, _) = await Initialize(
                options => { }, options => { options.AddHandler<CustomExecuteCommandHandler>(); }
            );

            var response = await client.SendRequest(
                new CustomExecuteCommandParams
                {
                    Command = "mycommand",
                }, CancellationToken
            );

            response.Should().BeEquivalentTo(JToken.FromObject(new { someValue = "custom" }));
        }

        [Fact]
        public async Task Should_Support_Mixed_Execute_Command_Handlers()
        {
            var (client, _) = await Initialize(
                options => { }, options =>
                {
                    options.AddHandler<CustomExecuteCommandHandler>();
                    options.OnExecuteCommand<JObject>("myothercommand", (a, ct) => Unit.Task);
                }
            );

            var normalResponse = await client.SendRequest(
                new ExecuteCommandParams
                {
                    Command = "myothercommand",
                    Arguments = new JArray(new JObject())
                }, CancellationToken
            );

            var customResponse = await client.SendRequest(
                new CustomExecuteCommandParams
                {
                    Command = "mycommand",
                }, CancellationToken
            );

            normalResponse.Should().Be(Unit.Value);
            customResponse.Should().BeEquivalentTo(JToken.FromObject(new { someValue = "custom" }));
        }
    }

    [Method(WorkspaceNames.ExecuteCommand)]
    public class CustomExecuteCommandHandler : IJsonRpcRequestHandler<CustomExecuteCommandParams, JToken>,
                                               IRegistration<ExecuteCommandRegistrationOptions, ExecuteCommandCapability>
    {
        // ReSharper disable once NotAccessedField.Local
        private ExecuteCommandCapability? _capability;

        private readonly ExecuteCommandRegistrationOptions _executeCommandRegistrationOptions = new ExecuteCommandRegistrationOptions
        {
            WorkDoneProgress = true,
            Commands = new Container<string>("mycommand")
        };

        public Task<JToken> Handle(CustomExecuteCommandParams request, CancellationToken cancellationToken)
        {
            return Task.FromResult(JToken.FromObject(new { someValue = "custom" }));
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities)
        {
            _capability = capability;
            return _executeCommandRegistrationOptions;
        }
    }

    [Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    public partial record CustomExecuteCommandParams : IRequest<JToken>, IWorkDoneProgressParams, IExecuteCommandParams // required for routing
    {
        /// <summary>
        /// The identifier of the actual command handler.
        /// </summary>
        public string Command { get; init; }

        /// <summary>
        /// Arguments that the command should be invoked with.
        /// </summary>
        [Optional]
        public JArray? Arguments { get; init; }
    }
}

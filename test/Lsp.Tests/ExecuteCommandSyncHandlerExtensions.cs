using System.Threading;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace Lsp.Tests
{
    internal static class ExecuteCommandSyncExtensions
    {
        public static IExecuteCommandHandler With(Container<string> commands) => Substitute.For<IExecuteCommandHandler>().With(commands);

        public static IExecuteCommandHandler With(this IExecuteCommandHandler handler, Container<string> commands)
        {
            handler.GetRegistrationOptions(Arg.Any<ExecuteCommandCapability>()).Returns(new ExecuteCommandRegistrationOptions { Commands = commands });

            handler.Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            return handler;
        }
    }
}

using System.Threading;
using MediatR;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Lsp.Tests
{
    internal static class ExecuteCommandSyncExtensions
    {
        public static IExecuteCommandHandler With(Container<string> commands)
        {
            return Substitute.For<IExecuteCommandHandler>().With(commands);
        }

        public static IExecuteCommandHandler With(this IExecuteCommandHandler handler, Container<string> commands)
        {
            ((IExecuteCommandHandler)handler).GetRegistrationOptions().Returns(new ExecuteCommandRegistrationOptions { Commands = commands });

            handler.Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>()).Returns(Unit.Value);

            return handler;
        }
    }
}

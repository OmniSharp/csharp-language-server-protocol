using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Lsp.Tests
{
    internal static class ExecuteCommandSyncHandlerExtensions
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

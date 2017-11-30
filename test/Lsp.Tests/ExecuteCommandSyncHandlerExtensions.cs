using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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

            handler.Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            return handler;
        }
    }
}

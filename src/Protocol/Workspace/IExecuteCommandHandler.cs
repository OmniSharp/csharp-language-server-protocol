using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Serial]
    [Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
    public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions, ExecuteCommandCapability>
    {
    }

    public abstract class ExecuteCommandHandler : AbstractHandlers.Base<ExecuteCommandRegistrationOptions, ExecuteCommandCapability>, IExecuteCommandHandler
    {
        public abstract Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken);
    }

    public abstract class ExecuteCommandHandlerBase<T> : ExecuteCommandHandler
    {
        private readonly string _command;
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer)
        {
            _command = command;
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            return Handle(arg1!, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, CancellationToken cancellationToken);
        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability) => new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
    }

    public abstract class ExecuteCommandHandlerBase<T, T2> : ExecuteCommandHandler
    {
        private readonly string _command;
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer)
        {
            _command = command;
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            T2 arg2 = default;
            if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
            return Handle(arg1!, arg2!, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, CancellationToken cancellationToken);
        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability) => new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3> : ExecuteCommandHandler
    {
        private readonly string _command;
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer)
        {
            _command = command;
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            T2 arg2 = default;
            if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
            T3 arg3 = default;
            if (args.Count > 2) arg3 = args[2].ToObject<T3>(_serializer.JsonSerializer);
            return Handle(arg1!, arg2!, arg3!, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken);
        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability) => new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3, T4> : ExecuteCommandHandler
    {
        private readonly string _command;
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer)
        {
            _command = command;
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            T2 arg2 = default;
            if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
            T3 arg3 = default;
            if (args.Count > 2) arg3 = args[2].ToObject<T3>(_serializer.JsonSerializer);
            T4 arg4 = default;
            if (args.Count > 3) arg4 = args[3].ToObject<T4>(_serializer.JsonSerializer);
            return Handle(arg1!, arg2!, arg3!, arg4!, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken);
        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability) => new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3, T4, T5> : ExecuteCommandHandler
    {
        private readonly string _command;
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer)
        {
            _command = command;
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            T2 arg2 = default;
            if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
            T3 arg3 = default;
            if (args.Count > 2) arg3 = args[2].ToObject<T3>(_serializer.JsonSerializer);
            T4 arg4 = default;
            if (args.Count > 3) arg4 = args[3].ToObject<T4>(_serializer.JsonSerializer);
            T5 arg5 = default;
            if (args.Count > 4) arg5 = args[4].ToObject<T5>(_serializer.JsonSerializer);
            return Handle(arg1!, arg2!, arg3!, arg4!, arg5!, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken);
        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability) => new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3, T4, T5, T6> : ExecuteCommandHandler
    {
        private readonly string _command;
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer)
        {
            _command = command;
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            T2 arg2 = default;
            if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
            T3 arg3 = default;
            if (args.Count > 2) arg3 = args[2].ToObject<T3>(_serializer.JsonSerializer);
            T4 arg4 = default;
            if (args.Count > 3) arg4 = args[3].ToObject<T4>(_serializer.JsonSerializer);
            T5 arg5 = default;
            if (args.Count > 4) arg5 = args[4].ToObject<T5>(_serializer.JsonSerializer);
            T6 arg6 = default;
            if (args.Count > 5) arg6 = args[5].ToObject<T6>(_serializer.JsonSerializer);
            return Handle(arg1!, arg2!, arg3!, arg4!, arg5!, arg6!, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken);
        protected override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability) => new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
    }

    public static partial class ExecuteCommandExtensions
    {
        public static Task ExecuteCommand(this IWorkspaceLanguageClient mediator, Command @params, CancellationToken cancellationToken = default)
            => mediator.SendRequest(new ExecuteCommandParams { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

        public static Task ExecuteCommand(this ILanguageClient mediator, Command @params, CancellationToken cancellationToken = default)
            => mediator.SendRequest(new ExecuteCommandParams { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

        public static ILanguageServerRegistry OnExecuteCommand<T>(
            this ILanguageServerRegistry registry, string command, Func<T, ExecuteCommandCapability, CancellationToken, Task> handler
        ) => registry.AddHandler(_ => new Handler<T>(command, handler, _.GetRequiredService<ISerializer>()));

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, string command, Func<T, CancellationToken, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T>(
                    command,
                    (arg1, capability, token) => handler(arg1, token),
                    _.GetRequiredService<ISerializer>()
                )
            );

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, string command, Func<T, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T>(
                    command,
                    (arg1, capability, token) => handler(arg1),
                    _.GetRequiredService<ISerializer>()
                )
            );

        private class Handler<T> : ExecuteCommandHandlerBase<T>
        {
            private readonly Func<T, ExecuteCommandCapability, CancellationToken, Task> _handler;

            public Handler(string command, Func<T, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer) : base(command, serializer) =>
                _handler = handler;

            public override async Task<Unit> Handle(T arg1, CancellationToken cancellationToken)
            {
                await _handler(arg1, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, ExecuteCommandCapability, CancellationToken, Task> handler
        ) => registry.AddHandler(_ => new Handler<T, T2>(command, handler, _.GetRequiredService<ISerializer>()));

        public static ILanguageServerRegistry OnExecuteCommand<T, T2>(this ILanguageServerRegistry registry, string command, Func<T, T2, CancellationToken, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T, T2>(
                    command,
                    (arg1, arg2, capability, token) => handler(arg1, arg2, token),
                    _.GetRequiredService<ISerializer>()
                )
            );

        public static ILanguageServerRegistry OnExecuteCommand<T, T2>(this ILanguageServerRegistry registry, string command, Func<T, T2, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T, T2>(
                    command,
                    (arg1, arg2, capability, token) => handler(arg1, arg2),
                    _.GetRequiredService<ISerializer>()
                )
            );

        private class Handler<T, T2> : ExecuteCommandHandlerBase<T, T2>
        {
            private readonly Func<T, T2, ExecuteCommandCapability, CancellationToken, Task> _handler;

            public Handler(string command, Func<T, T2, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer) : base(command, serializer) =>
                _handler = handler;

            public override async Task<Unit> Handle(T arg1, T2 arg2, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task> handler
        ) => registry.AddHandler(_ => new Handler<T, T2, T3>(command, handler, _.GetRequiredService<ISerializer>()));

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, CancellationToken, Task> handler
        ) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3>(
                    command,
                    (arg1, arg2, arg3, capability, token) => handler(arg1, arg2, arg3, token),
                    _.GetRequiredService<ISerializer>()
                )
            );

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3>(
                    command,
                    (arg1, arg2, arg3, capability, token) => handler(arg1, arg2, arg3),
                    _.GetRequiredService<ISerializer>()
                )
            );

        private class Handler<T, T2, T3> : ExecuteCommandHandlerBase<T, T2, T3>
        {
            private readonly Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task> _handler;

            public Handler(string command, Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer) : base(command, serializer) =>
                _handler = handler;

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task> handler
        ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4>(command, handler, _.GetRequiredService<ISerializer>()));

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, CancellationToken, Task> handler
        ) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3, T4>(
                    command,
                    (arg1, arg2, arg3, arg4, capability, token) => handler(arg1, arg2, arg3, arg4, token),
                    _.GetRequiredService<ISerializer>()
                )
            );

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3, T4>(
                    command,
                    (arg1, arg2, arg3, arg4, capability, token) => handler(arg1, arg2, arg3, arg4),
                    _.GetRequiredService<ISerializer>()
                )
            );

        private class Handler<T, T2, T3, T4> : ExecuteCommandHandlerBase<T, T2, T3, T4>
        {
            private readonly Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task> _handler;

            public Handler(string command, Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer) : base(command, serializer) =>
                _handler = handler;

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, arg4, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task> handler
        ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5>(command, handler, _.GetRequiredService<ISerializer>()));

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, CancellationToken, Task> handler
        ) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3, T4, T5>(
                    command,
                    (arg1, arg2, arg3, arg4, arg5, capability, token) => handler(arg1, arg2, arg3, arg4, arg5, token),
                    _.GetRequiredService<ISerializer>()
                )
            );

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, Task> handler) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3, T4, T5>(
                    command,
                    (arg1, arg2, arg3, arg4, arg5, capability, token) => handler(arg1, arg2, arg3, arg4, arg5),
                    _.GetRequiredService<ISerializer>()
                )
            );

        private class Handler<T, T2, T3, T4, T5> : ExecuteCommandHandlerBase<T, T2, T3, T4, T5>
        {
            private readonly Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task> _handler;

            public Handler(string command, Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer) :
                base(command, serializer) => _handler = handler;

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, arg4, arg5, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task> handler
        ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5, T6>(command, handler, _.GetRequiredService<ISerializer>()));

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, T6, CancellationToken, Task> handler
        ) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3, T4, T5, T6>(
                    command,
                    (arg1, arg2, arg3, arg4, arg5, arg6, capability, token) => handler(arg1, arg2, arg3, arg4, arg5, arg6, token),
                    _.GetRequiredService<ISerializer>()
                )
            );

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6>(
            this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, T6, Task> handler
        ) =>
            registry.AddHandler(
                _ => new Handler<T, T2, T3, T4, T5, T6>(
                    command,
                    (arg1, arg2, arg3, arg4, arg5, arg6, capability, token) => handler(arg1, arg2, arg3, arg4, arg5, arg6),
                    _.GetRequiredService<ISerializer>()
                )
            );

        private class Handler<T, T2, T3, T4, T5, T6> : ExecuteCommandHandlerBase<T, T2, T3, T4, T5, T6>
        {
            private readonly Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task> _handler;

            public Handler(string command, Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer) : base(
                command, serializer
            ) => _handler = handler;

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, arg4, arg5, arg6, Capability, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }
        }
    }
}

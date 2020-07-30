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
    [Serial, Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
    public interface IExecuteCommandHandler : IJsonRpcRequestHandler<ExecuteCommandParams>, IRegistration<ExecuteCommandRegistrationOptions>, ICapability<ExecuteCommandCapability> { }

    public abstract class ExecuteCommandHandler : IExecuteCommandHandler
    {
        private readonly ExecuteCommandRegistrationOptions _options;

        public ExecuteCommandHandler(ExecuteCommandRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public ExecuteCommandRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ExecuteCommandCapability capability) => Capability = capability;
        protected ExecuteCommandCapability Capability { get; private set; }
    }

    public abstract class ExecuteCommandHandlerBase<T> : ExecuteCommandHandler
    {
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer) : base(new ExecuteCommandRegistrationOptions() { Commands = new Container<string>(command) })
        {
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            return Handle(arg1, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, CancellationToken cancellationToken);
    }

    public abstract class ExecuteCommandHandlerBase<T, T2> : ExecuteCommandHandler
    {
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer) : base(new ExecuteCommandRegistrationOptions() { Commands = new Container<string>(command) })
        {
            _serializer = serializer;
        }

        public sealed override Task<Unit> Handle(ExecuteCommandParams request, CancellationToken cancellationToken)
        {
            var args = request.Arguments ?? new JArray();
            T arg1 = default;
            if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
            T2 arg2 = default;
            if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
            return Handle(arg1, arg2, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, CancellationToken cancellationToken);
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3> : ExecuteCommandHandler
    {
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer) : base(new ExecuteCommandRegistrationOptions() { Commands = new Container<string>(command) })
        {
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
            return Handle(arg1, arg2, arg3, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken);
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3, T4> : ExecuteCommandHandler
    {
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer) : base(new ExecuteCommandRegistrationOptions() { Commands = new Container<string>(command) })
        {
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
            return Handle(arg1, arg2, arg3, arg4, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken);
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3, T4, T5> : ExecuteCommandHandler
    {
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer) : base(new ExecuteCommandRegistrationOptions() { Commands = new Container<string>(command) })
        {
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
            return Handle(arg1, arg2, arg3, arg4, arg5, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken);
    }

    public abstract class ExecuteCommandHandlerBase<T, T2, T3, T4, T5, T6> : ExecuteCommandHandler
    {
        private readonly ISerializer _serializer;

        public ExecuteCommandHandlerBase(string command, ISerializer serializer) : base(new ExecuteCommandRegistrationOptions() { Commands = new Container<string>(command) })
        {
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
            return Handle(arg1, arg2, arg3, arg4, arg5, arg6, cancellationToken);
        }

        public abstract Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken);
    }

    public static partial class ExecuteCommandExtensions
    {
        public static Task ExecuteCommand(this IWorkspaceLanguageClient mediator, Command @params, CancellationToken cancellationToken = default)
            => mediator.ExecuteCommand(new ExecuteCommandParams() { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

        public static Task ExecuteCommand(this ILanguageClient mediator, Command @params, CancellationToken cancellationToken = default)
            => mediator.ExecuteCommand(new ExecuteCommandParams() { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

        public static ILanguageServerRegistry OnExecuteCommand<T>(this ILanguageServerRegistry registry, string command, Func<T, Task> handler)
        {
            return registry.AddHandler(_ => new Handler<T>(command, handler, _.GetRequiredService<ISerializer>()));
        }

        class Handler<T> : ExecuteCommandHandlerBase<T>
        {
            private readonly Func<T, Task> _handler;

            public Handler(string command, Func<T, Task> handler, ISerializer serializer) : base(command, serializer)
            {
                _handler = handler;
            }

            public override async Task<Unit> Handle(T arg1, CancellationToken cancellationToken)
            {
                await _handler(arg1);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2>(this ILanguageServerRegistry registry, string command, Func<T, T2, Task> handler)
        {
            return registry.AddHandler(_ => new Handler<T, T2>(command, handler, _.GetRequiredService<ISerializer>()));
        }

        class Handler<T, T2> : ExecuteCommandHandlerBase<T, T2>
        {
            private readonly Func<T, T2, Task> _handler;

            public Handler(string command, Func<T, T2, Task> handler, ISerializer serializer) : base(command, serializer)
            {
                _handler = handler;
            }

            public override async Task<Unit> Handle(T arg1, T2 arg2, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, Task> handler)
        {
            return registry.AddHandler(_ => new Handler<T, T2, T3>(command, handler, _.GetRequiredService<ISerializer>()));
        }

        class Handler<T, T2, T3> : ExecuteCommandHandlerBase<T, T2, T3>
        {
            private readonly Func<T, T2, T3, Task> _handler;

            public Handler(string command, Func<T, T2, T3, Task> handler, ISerializer serializer) : base(command, serializer)
            {
                _handler = handler;
            }

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, Task> handler)
        {
            return registry.AddHandler(_ => new Handler<T, T2, T3, T4>(command, handler, _.GetRequiredService<ISerializer>()));
        }

        class Handler<T, T2, T3, T4> : ExecuteCommandHandlerBase<T, T2, T3, T4>
        {
            private readonly Func<T, T2, T3, T4, Task> _handler;

            public Handler(string command, Func<T, T2, T3, T4, Task> handler, ISerializer serializer) : base(command, serializer)
            {
                _handler = handler;
            }

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, arg4);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, Task> handler)
        {
            return registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5>(command, handler, _.GetRequiredService<ISerializer>()));
        }

        class Handler<T, T2, T3, T4, T5> : ExecuteCommandHandlerBase<T, T2, T3, T4, T5>
        {
            private readonly Func<T, T2, T3, T4, T5, Task> _handler;

            public Handler(string command, Func<T, T2, T3, T4, T5, Task> handler, ISerializer serializer) : base(command, serializer)
            {
                _handler = handler;
            }

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, arg4, arg5);
                return Unit.Value;
            }
        }

        public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, T6, Task> handler)
        {
            return registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5, T6>(command, handler, _.GetRequiredService<ISerializer>()));
        }

        class Handler<T, T2, T3, T4, T5, T6> : ExecuteCommandHandlerBase<T, T2, T3, T4, T5, T6>
        {
            private readonly Func<T, T2, T3, T4, T5, T6, Task> _handler;

            public Handler(string command, Func<T, T2, T3, T4, T5, T6, Task> handler, ISerializer serializer) : base(command, serializer)
            {
                _handler = handler;
            }

            public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken)
            {
                await _handler(arg1, arg2, arg3, arg4, arg5, arg6);
                return Unit.Value;
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using ISerializer = OmniSharp.Extensions.JsonRpc.ISerializer;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Serial]
        [Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace" /*, AllowDerivedRequests = true*/),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(ExecuteCommandRegistrationOptions)), Capability(typeof(ExecuteCommandCapability))]
        public partial record ExecuteCommandParams : IJsonRpcRequest, IWorkDoneProgressParams, IExecuteCommandParams
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

        [Serial]
        [Method(WorkspaceNames.ExecuteCommand, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace" /*, AllowDerivedRequests = true*/),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(ExecuteCommandRegistrationOptions)), Capability(typeof(ExecuteCommandCapability))]
        public partial record ExecuteCommandParams<T> : IRequest<T>, IWorkDoneProgressParams, IExecuteCommandParams
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

        /// <summary>
        /// Execute command registration options.
        /// </summary>
        [GenerateRegistrationOptions(nameof(ServerCapabilities.ExecuteCommandProvider))]
        [RegistrationOptionsConverter(typeof(ExecuteCommandRegistrationOptionsConverter))]
        [RegistrationName(WorkspaceNames.ExecuteCommand)]
        public partial class ExecuteCommandRegistrationOptions : IWorkDoneProgressOptions
        {
            /// <summary>
            /// The commands to be executed on the server
            /// </summary>
            public Container<string> Commands { get; set; } = null!;

            class ExecuteCommandRegistrationOptionsConverter : RegistrationOptionsConverterBase<ExecuteCommandRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public ExecuteCommandRegistrationOptionsConverter(IHandlersManager handlersManager)
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(ExecuteCommandRegistrationOptions source)
                {
                    var allRegistrationOptions = _handlersManager.Descriptors
                                                                 .OfType<ILspHandlerDescriptor>()
                                                                 .Where(z => z.HasRegistration)
                                                                 .Select(z => z.RegistrationOptions)
                                                                 .OfType<ExecuteCommandRegistrationOptions>()
                                                                 .ToArray();
                    return new() {
                        Commands = allRegistrationOptions.SelectMany(z => z.Commands).ToArray(),
//                    WorkDoneProgress = allRegistrationOptions.Any(x => x.WorkDoneProgress)
                    };
                }
            }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.ExecuteCommand))]
        public class ExecuteCommandCapability : DynamicCapability
        {
        }
    }

    namespace Workspace
    {
        public abstract class ExecuteTypedCommandHandlerBase<T> : ExecuteCommandHandlerBase
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedCommandHandlerBase(string command, ISerializer serializer)
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

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedResponseCommandHandlerBase<T, TResponse> : ExecuteCommandHandlerBase<TResponse>
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedResponseCommandHandlerBase(string command, ISerializer serializer)
            {
                _command = command;
                _serializer = serializer;
            }

            public sealed override Task<TResponse> Handle(ExecuteCommandParams<TResponse> request, CancellationToken cancellationToken)
            {
                var args = request.Arguments ?? new JArray();
                T arg1 = default;
                if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
                return Handle(arg1!, cancellationToken);
            }

            public abstract Task<TResponse> Handle(T arg1, CancellationToken cancellationToken);

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedCommandHandlerBase<T, T2> : ExecuteCommandHandlerBase
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedCommandHandlerBase(string command, ISerializer serializer)
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

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedResponseCommandHandlerBase<T, T2, TResponse> : ExecuteCommandHandlerBase<TResponse>
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedResponseCommandHandlerBase(string command, ISerializer serializer)
            {
                _command = command;
                _serializer = serializer;
            }

            public sealed override Task<TResponse> Handle(ExecuteCommandParams<TResponse> request, CancellationToken cancellationToken)
            {
                var args = request.Arguments ?? new JArray();
                T arg1 = default;
                if (args.Count > 0) arg1 = args[0].ToObject<T>(_serializer.JsonSerializer);
                T2 arg2 = default;
                if (args.Count > 1) arg2 = args[1].ToObject<T2>(_serializer.JsonSerializer);
                return Handle(arg1!, arg2!, cancellationToken);
            }

            public abstract Task<TResponse> Handle(T arg1, T2 arg2, CancellationToken cancellationToken);

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedCommandHandlerBase<T, T2, T3> : ExecuteCommandHandlerBase
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedCommandHandlerBase(string command, ISerializer serializer)
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

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedResponseCommandHandlerBase<T, T2, T3, TResponse> : ExecuteCommandHandlerBase<TResponse>
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedResponseCommandHandlerBase(string command, ISerializer serializer)
            {
                _command = command;
                _serializer = serializer;
            }

            public sealed override Task<TResponse> Handle(ExecuteCommandParams<TResponse> request, CancellationToken cancellationToken)
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

            public abstract Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken);

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedCommandHandlerBase<T, T2, T3, T4> : ExecuteCommandHandlerBase
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedCommandHandlerBase(string command, ISerializer serializer)
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

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedResponseCommandHandlerBase<T, T2, T3, T4, TResponse> : ExecuteCommandHandlerBase<TResponse>
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedResponseCommandHandlerBase(string command, ISerializer serializer)
            {
                _command = command;
                _serializer = serializer;
            }

            public sealed override Task<TResponse> Handle(ExecuteCommandParams<TResponse> request, CancellationToken cancellationToken)
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

            public abstract Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken);

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedCommandHandlerBase<T, T2, T3, T4, T5> : ExecuteCommandHandlerBase
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedCommandHandlerBase(string command, ISerializer serializer)
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

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedResponseCommandHandlerBase<T, T2, T3, T4, T5, TResponse> : ExecuteCommandHandlerBase<TResponse>
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedResponseCommandHandlerBase(string command, ISerializer serializer)
            {
                _command = command;
                _serializer = serializer;
            }

            public sealed override Task<TResponse> Handle(ExecuteCommandParams<TResponse> request, CancellationToken cancellationToken)
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

            public abstract Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken);

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedCommandHandlerBase<T, T2, T3, T4, T5, T6> : ExecuteCommandHandlerBase
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedCommandHandlerBase(string command, ISerializer serializer)
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

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public abstract class ExecuteTypedResponseCommandHandlerBase<T, T2, T3, T4, T5, T6, TResponse> : ExecuteCommandHandlerBase<TResponse>
        {
            private readonly string _command;
            private readonly ISerializer _serializer;

            public ExecuteTypedResponseCommandHandlerBase(string command, ISerializer serializer)
            {
                _command = command;
                _serializer = serializer;
            }

            public sealed override Task<TResponse> Handle(ExecuteCommandParams<TResponse> request, CancellationToken cancellationToken)
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

            public abstract Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken);

            protected internal override ExecuteCommandRegistrationOptions CreateRegistrationOptions(ExecuteCommandCapability capability, ClientCapabilities clientCapabilities) =>
                new ExecuteCommandRegistrationOptions { Commands = new Container<string>(_command) };
        }

        public static partial class ExecuteTypedCommandExtensions
        {
            public static Task ExecuteCommand(
                this IWorkspaceLanguageClient mediator,
                Command @params,
                CancellationToken cancellationToken = default
            )
                => mediator.SendRequest(new ExecuteCommandParams { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

            public static Task ExecuteCommand(
                this ILanguageClient mediator,
                Command @params,
                CancellationToken cancellationToken = default
            )
                => mediator.SendRequest(new ExecuteCommandParams { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

            public static Task<TResponse> ExecuteCommandWithResponse<TResponse>(
                this IWorkspaceLanguageClient mediator,
                Command @params,
                CancellationToken cancellationToken = default
            )
                => mediator.SendRequest(new ExecuteCommandParams<TResponse> { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

            public static Task<TResponse> ExecuteCommandWithResponse<TResponse>(
                this ILanguageClient mediator,
                Command @params,
                CancellationToken cancellationToken = default
            )
                => mediator.SendRequest(new ExecuteCommandParams<TResponse> { Arguments = @params.Arguments, Command = @params.Name }, cancellationToken);

            public static ILanguageServerRegistry OnExecuteCommand<T>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, ExecuteCommandCapability, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, CancellationToken, Task> handler
            ) =>
                registry.AddHandler(_ => new Handler<T>(command, (arg1, _, token) => handler(arg1, token), _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, Task> handler
            ) => registry.AddHandler(_ => new Handler<T>(command, (arg1, _, _) => handler(arg1), _.GetRequiredService<ISerializer>()));

            private class Handler<T> : ExecuteTypedCommandHandlerBase<T>
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
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, ExecuteCommandCapability, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2>(command, (arg1, arg2, _, token) => handler(arg1, arg2, token), _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2>(command, (arg1, arg2, _, _) => handler(arg1, arg2), _.GetRequiredService<ISerializer>()));

            private class Handler<T, T2> : ExecuteTypedCommandHandlerBase<T, T2>
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
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3>(command, (arg1, arg2, arg3, _, token) => handler(arg1, arg2, arg3, token), _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3>(this ILanguageServerRegistry registry, string command, Func<T, T2, T3, Task> handler) =>
                registry.AddHandler(_ => new Handler<T, T2, T3>(command, (arg1, arg2, arg3, _, _) => handler(arg1, arg2, arg3), _.GetRequiredService<ISerializer>()));

            private class Handler<T, T2, T3> : ExecuteTypedCommandHandlerBase<T, T2, T3>
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
            ) => registry.AddHandler(
                _ => new Handler<T, T2, T3, T4>(command, (arg1, arg2, arg3, arg4, _, token) => handler(arg1, arg2, arg3, arg4, token), _.GetRequiredService<ISerializer>())
            );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, Task> handler
            )
                =>
                    registry.AddHandler(
                        _ => new Handler<T, T2, T3, T4>(command, (arg1, arg2, arg3, arg4, _, _) => handler(arg1, arg2, arg3, arg4), _.GetRequiredService<ISerializer>())
                    );

            private class Handler<T, T2, T3, T4> : ExecuteTypedCommandHandlerBase<T, T2, T3, T4>
            {
                private readonly Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task> _handler;

                public Handler(
                    string command, Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task> handler, ISerializer serializer
                ) : base(command, serializer) =>
                    _handler = handler;

                public override async Task<Unit> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken)
                {
                    await _handler(arg1, arg2, arg3, arg4, Capability, cancellationToken).ConfigureAwait(false);
                    return Unit.Value;
                }
            }

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, CancellationToken, Task> handler
            ) => registry.AddHandler(
                _ => new Handler<T, T2, T3, T4, T5>(
                    command, (arg1, arg2, arg3, arg4, arg5, _, token) => handler(arg1, arg2, arg3, arg4, arg5, token), _.GetRequiredService<ISerializer>()
                )
            );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, Task> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, T5>(command, (arg1, arg2, arg3, arg4, arg5, _, _) => handler(arg1, arg2, arg3, arg4, arg5), _.GetRequiredService<ISerializer>())
                );

            private class Handler<T, T2, T3, T4, T5> : ExecuteTypedCommandHandlerBase<T, T2, T3, T4, T5>
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
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5, T6>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, T6, CancellationToken, Task> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, T5, T6>(
                        command, (arg1, arg2, arg3, arg4, arg5, arg6, _, token) => handler(arg1, arg2, arg3, arg4, arg5, arg6, token), _.GetRequiredService<ISerializer>()
                    )
                );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6>(
                this ILanguageServerRegistry registry, string command, Func<T, T2, T3, T4, T5, T6, Task> handler
            ) => registry.AddHandler(
                _ => new Handler<T, T2, T3, T4, T5, T6>(
                    command, (arg1, arg2, arg3, arg4, arg5, arg6, _, _) => handler(arg1, arg2, arg3, arg4, arg5, arg6), _.GetRequiredService<ISerializer>()
                )
            );

            private class Handler<T, T2, T3, T4, T5, T6> : ExecuteTypedCommandHandlerBase<T, T2, T3, T4, T5, T6>
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

        public static partial class ExecuteTypedResponseCommandExtensions
        {
            public static ILanguageServerRegistry OnExecuteCommand<T, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, TResponse>(command, (arg1, _, token) => handler(arg1, token), _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, TResponse>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, TResponse>(
                        command,
                        (arg1, _, _) => handler(arg1),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            private class Handler<T, TResponse> : ExecuteTypedResponseCommandHandlerBase<T, TResponse>
            {
                private readonly Func<T, ExecuteCommandCapability, CancellationToken, Task<TResponse>> _handler;

                public Handler(string command, Func<T, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler, ISerializer serializer) : base(command, serializer) =>
                    _handler = handler;

                public override Task<TResponse> Handle(T arg1, CancellationToken cancellationToken) => _handler(arg1, Capability, cancellationToken);
            }

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, TResponse>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, CancellationToken, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, TResponse>(
                        command,
                        (arg1, arg2, _, token) => handler(arg1, arg2, token),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, TResponse>(
                        command,
                        (arg1, arg2, _, _) => handler(arg1, arg2),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            private class Handler<T, T2, TResponse> : ExecuteTypedResponseCommandHandlerBase<T, T2, TResponse>
            {
                private readonly Func<T, T2, ExecuteCommandCapability, CancellationToken, Task<TResponse>> _handler;

                public Handler(string command, Func<T, T2, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler, ISerializer serializer) : base(
                    command, serializer
                ) =>
                    _handler = handler;

                public override Task<TResponse> Handle(T arg1, T2 arg2, CancellationToken cancellationToken) => _handler(arg1, arg2, Capability, cancellationToken);
            }

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3, TResponse>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, CancellationToken, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, TResponse>(
                        command,
                        (arg1, arg2, arg3, _, token) => handler(arg1, arg2, arg3, token),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, TResponse>(
                        command,
                        (arg1, arg2, arg3, _, _) => handler(arg1, arg2, arg3),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            private class Handler<T, T2, T3, TResponse> : ExecuteTypedResponseCommandHandlerBase<T, T2, T3, TResponse>
            {
                private readonly Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task<TResponse>> _handler;

                public Handler(string command, Func<T, T2, T3, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler, ISerializer serializer) : base(
                    command, serializer
                ) =>
                    _handler = handler;

                public override Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, CancellationToken cancellationToken)
                {
                    return _handler(arg1, arg2, arg3, Capability, cancellationToken);
                }
            }

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, TResponse>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, CancellationToken, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, TResponse>(
                        command,
                        (arg1, arg2, arg3, arg4, _, token) => handler(arg1, arg2, arg3, arg4, token),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, TResponse>(
                        command,
                        (arg1, arg2, arg3, arg4, _, _) => handler(arg1, arg2, arg3, arg4),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            private class Handler<T, T2, T3, T4, TResponse> : ExecuteTypedResponseCommandHandlerBase<T, T2, T3, T4, TResponse>
            {
                private readonly Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task<TResponse>> _handler;

                public Handler(
                    string command, Func<T, T2, T3, T4, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler, ISerializer serializer
                ) : base(command, serializer) =>
                    _handler = handler;

                public override Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken cancellationToken)
                {
                    return _handler(arg1, arg2, arg3, arg4, Capability, cancellationToken);
                }
            }

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5, TResponse>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, CancellationToken, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, T5, TResponse>(
                        command,
                        (arg1, arg2, arg3, arg4, arg5, _, token) => handler(arg1, arg2, arg3, arg4, arg5, token),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, T5, TResponse>(
                        command,
                        (arg1, arg2, arg3, arg4, arg5, _, _) => handler(arg1, arg2, arg3, arg4, arg5),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            private class Handler<T, T2, T3, T4, T5, TResponse> : ExecuteTypedResponseCommandHandlerBase<T, T2, T3, T4, T5, TResponse>
            {
                private readonly Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task<TResponse>> _handler;

                public Handler(string command, Func<T, T2, T3, T4, T5, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler, ISerializer serializer) :
                    base(command, serializer) => _handler = handler;

                public override Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken cancellationToken)
                {
                    return _handler(arg1, arg2, arg3, arg4, arg5, Capability, cancellationToken);
                }
            }

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler
            ) => registry.AddHandler(_ => new Handler<T, T2, T3, T4, T5, T6, TResponse>(command, handler, _.GetRequiredService<ISerializer>()));

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, T6, CancellationToken, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, T5, T6, TResponse>(
                        command,
                        (arg1, arg2, arg3, arg4, arg5, arg6, _, token) => handler(arg1, arg2, arg3, arg4, arg5, arg6, token),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            public static ILanguageServerRegistry OnExecuteCommand<T, T2, T3, T4, T5, T6, TResponse>(
                this ILanguageServerRegistry registry,
                string command,
                Func<T, T2, T3, T4, T5, T6, Task<TResponse>> handler
            ) =>
                registry.AddHandler(
                    _ => new Handler<T, T2, T3, T4, T5, T6, TResponse>(
                        command,
                        (arg1, arg2, arg3, arg4, arg5, arg6, _, _) => handler(arg1, arg2, arg3, arg4, arg5, arg6),
                        _.GetRequiredService<ISerializer>()
                    )
                );

            private class Handler<T, T2, T3, T4, T5, T6, TResponse> : ExecuteTypedResponseCommandHandlerBase<T, T2, T3, T4, T5, T6, TResponse>
            {
                private readonly Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task<TResponse>> _handler;

                public Handler(string command, Func<T, T2, T3, T4, T5, T6, ExecuteCommandCapability, CancellationToken, Task<TResponse>> handler, ISerializer serializer) : base(
                    command, serializer
                ) => _handler = handler;

                public override Task<TResponse> Handle(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, CancellationToken cancellationToken)
                {
                    return _handler(arg1, arg2, arg3, arg4, arg5, arg6, Capability, cancellationToken);
                }
            }
        }
    }
}

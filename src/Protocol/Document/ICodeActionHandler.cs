using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.CodeAction, Direction.ClientToServer)]
    public interface ICodeActionHandler : IJsonRpcRequestHandler<CodeActionParams, CommandOrCodeActionContainer>,
        IRegistration<CodeActionRegistrationOptions>, ICapability<CodeActionCapability>
    {
    }

    public abstract class CodeActionHandler : ICodeActionHandler
    {
        private readonly CodeActionRegistrationOptions _options;

        public CodeActionHandler(CodeActionRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CodeActionRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<CommandOrCodeActionContainer> Handle(CodeActionParams request,
            CancellationToken cancellationToken);

        public virtual void SetCapability(CodeActionCapability capability) => Capability = capability;
        protected CodeActionCapability Capability { get; private set; }
    }

    public static class CodeActionExtensions
    {
        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CodeActionCapability, CancellationToken, Task<CommandOrCodeActionContainer>>
                handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                new LanguageProtocolDelegatingHandlers.Request<CodeActionParams, CommandOrCodeActionContainer, CodeActionCapability,
                    CodeActionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, CancellationToken, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                    CodeActionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Func<CodeActionParams, Task<CommandOrCodeActionContainer>> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                new LanguageProtocolDelegatingHandlers.RequestRegistration<CodeActionParams, CommandOrCodeActionContainer,
                    CodeActionRegistrationOptions>(handler, registrationOptions));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CommandOrCodeAction>>, CodeActionCapability,
                CancellationToken> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CommandOrCodeAction, CodeActionCapability, CodeActionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new CommandOrCodeActionContainer(x)));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CommandOrCodeAction>>, CodeActionCapability>
                handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CommandOrCodeAction, CodeActionCapability, CodeActionRegistrationOptions>(handler,
                        registrationOptions, _.GetService<IProgressManager>(), x => new CommandOrCodeActionContainer(x)));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CommandOrCodeAction>>, CancellationToken> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CommandOrCodeAction, CodeActionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new CommandOrCodeActionContainer(x)));
        }

        public static IDisposable OnCodeAction(
            this ILanguageServerRegistry registry,
            Action<CodeActionParams, IObserver<IEnumerable<CommandOrCodeAction>>> handler,
            CodeActionRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new CodeActionRegistrationOptions();
            return registry.AddHandler(TextDocumentNames.CodeAction,
                _ =>
                    new LanguageProtocolDelegatingHandlers.PartialResults<CodeActionParams, CommandOrCodeActionContainer,
                        CommandOrCodeAction, CodeActionRegistrationOptions>(handler, registrationOptions,
                        _.GetService<IProgressManager>(), x => new CommandOrCodeActionContainer(x)));
        }

        public static IRequestProgressObservable<IEnumerable<CommandOrCodeAction>, CommandOrCodeActionContainer> RequestCodeAction(
            this ITextDocumentLanguageClient mediator,
            CodeActionParams @params,
            CancellationToken cancellationToken = default)
        {
            return mediator.ProgressManager.MonitorUntil(@params, x => new CommandOrCodeActionContainer(x), cancellationToken);
        }
    }
}

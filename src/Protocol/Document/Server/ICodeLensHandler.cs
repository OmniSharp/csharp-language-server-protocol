using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.CodeLens)]
    public interface ICodeLensHandler : IJsonRpcRequestHandler<CodeLensParams, CodeLensContainer>, IRegistration<CodeLensRegistrationOptions>, ICapability<CodeLensCapability> { }

    [Parallel, Method(DocumentNames.CodeLensResolve)]
    public interface ICodeLensResolveHandler : ICanBeResolvedHandler<CodeLens> { }

    public abstract class CodeLensHandler : ICodeLensHandler, ICodeLensResolveHandler
    {
        private readonly CodeLensRegistrationOptions _options;
        public CodeLensHandler(CodeLensRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken);
        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CodeLens value);
        public abstract void SetCapability(CodeLensCapability capability);
    }

    public static class CodeLensHandlerExtensions
    {
        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler = null,
            Func<CodeLens, bool> canResolve = null,
            CodeLensRegistrationOptions registrationOptions = null,
            Action<CodeLensCapability> setCapability = null)
        {
            if (registrationOptions != null)
            {
                registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            }
            return registry.AddHandlers(new DelegatingHandler(handler, resolveHandler, canResolve, setCapability, registrationOptions));
        }

        class DelegatingHandler : CodeLensHandler
        {
            private readonly Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> _handler;
            private readonly Func<CodeLens, CancellationToken, Task<CodeLens>> _resolveHandler;
            private readonly Func<CodeLens, bool> _canResolve;
            private readonly Action<CodeLensCapability> _setCapability;

            public DelegatingHandler(
                Func<CodeLensParams, CancellationToken, Task<CodeLensContainer>> handler,
                Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
                Func<CodeLens, bool> canResolve,
                Action<CodeLensCapability> setCapability,
                CodeLensRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<CodeLensContainer> Handle(CodeLensParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(CodeLens value) => _canResolve.Invoke(value);
            public override void SetCapability(CodeLensCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}

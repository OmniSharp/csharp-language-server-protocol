using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    [BuiltIn]
    internal class LanguageClientRegistrationManager : IRegisterCapabilityHandler, IUnregisterCapabilityHandler, IRegistrationManager, IDisposable
    {
        private readonly ISerializer _serializer;
        private readonly ILspHandlerTypeDescriptorProvider _handlerTypeDescriptorProvider;
        private readonly ILogger<LanguageClientRegistrationManager> _logger;
        private readonly ConcurrentDictionary<string, Registration> _registrations;
        private readonly ReplaySubject<IEnumerable<Registration>> _registrationSubject = new ReplaySubject<IEnumerable<Registration>>(1);

        public LanguageClientRegistrationManager(
            ISerializer serializer,
            ILspHandlerTypeDescriptorProvider handlerTypeDescriptorProvider,
            ILogger<LanguageClientRegistrationManager> logger
        )
        {
            _serializer = serializer;
            _handlerTypeDescriptorProvider = handlerTypeDescriptorProvider;
            _logger = logger;
            _registrations = new ConcurrentDictionary<string, Registration>(StringComparer.OrdinalIgnoreCase);
        }

        async Task<Unit> IRequestHandler<RegistrationParams, Unit>.Handle(RegistrationParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            lock (this)
            {
                Register(request.Registrations.ToArray());
            }

            if (!_registrationSubject.IsDisposed)
            {
                _registrationSubject.OnNext(_registrations.Values);
            }

            return Unit.Value;
        }

        async Task<Unit> IRequestHandler<UnregistrationParams, Unit>.Handle(UnregistrationParams request, CancellationToken cancellationToken)
        {
            await Task.Yield();
            lock (this)
            {
                foreach (var item in request.Unregisterations ?? new UnregistrationContainer())
                {
                    _registrations.TryRemove(item.Id, out _);
                }
            }

            if (!_registrationSubject.IsDisposed)
            {
                _registrationSubject.OnNext(_registrations.Values);
            }

            return Unit.Value;
        }

        public void RegisterCapabilities(ServerCapabilities serverCapabilities)
        {
            foreach (var registrationOptions in LspHandlerDescriptorHelpers.GetStaticRegistrationOptions(serverCapabilities))
            {
                var method = _handlerTypeDescriptorProvider.GetMethodForRegistrationOptions(registrationOptions);
                if (method == null)
                {
                    _logger.LogWarning("Unable to find method for given {@RegistrationOptions}", registrationOptions);
                    continue;
                }

                if (registrationOptions.Id != null)
                {
                    var reg = new Registration {
                        Id = registrationOptions.Id,
                        Method = method,
                        RegisterOptions = registrationOptions
                    };
                    _registrations.AddOrUpdate(registrationOptions.Id, x => reg, (a, b) => reg);
                }
            }

            if (serverCapabilities.Workspace != null)
            {
                foreach (var registrationOptions in LspHandlerDescriptorHelpers.GetStaticRegistrationOptions(serverCapabilities.Workspace))
                {
                    var method = _handlerTypeDescriptorProvider.GetMethodForRegistrationOptions(registrationOptions);
                    if (method == null)
                    {
                        // TODO: Log this
                        continue;
                    }

                    if (registrationOptions.Id != null)
                    {
                        var reg = new Registration {
                            Id = registrationOptions.Id,
                            Method = method,
                            RegisterOptions = registrationOptions
                        };
                        _registrations.AddOrUpdate(registrationOptions.Id, x => reg, (a, b) => reg);
                    }
                }
            }

            _registrationSubject.OnNext(_registrations.Values);
        }

        private void Register(params Registration[] registrations)
        {
            var newRegistrations = new List<Registration>();
            foreach (var registration in registrations)
            {
                newRegistrations.Add(Register(registration));
            }

            foreach (var reg in newRegistrations)
            {
                _registrations.AddOrUpdate(reg.Id, reg, (a, b) => reg);
            }
        }

        private Registration Register(Registration registration)
        {
            var registrationType = _handlerTypeDescriptorProvider.GetRegistrationType(registration.Method);
            if (registrationType == null)
            {
                // vscode client throws if given an unknown registration type
                throw new NotSupportedException($"Unknown Registration Type '{registration.Method}'");
            }

            var deserializedRegistration = new Registration {
                Id = registration.Id,
                Method = registration.Method,
                RegisterOptions = registration.RegisterOptions is JToken token
                    ? token.ToObject(registrationType, _serializer.JsonSerializer)
                    : registration.RegisterOptions
            };

            return deserializedRegistration;
        }

        public IObservable<IEnumerable<Registration>> Registrations
        {
            get {
                if (_registrationSubject.IsDisposed)
                {
                    return Observable.Empty<IEnumerable<Registration>>();
                }

                return _registrationSubject.AsObservable();
            }
        }

        public IEnumerable<Registration> CurrentRegistrations => _registrations.Values;

        public IEnumerable<Registration> GetRegistrationsForMethod(string method) => _registrations.Select(z => z.Value).Where(x => x.Method == method);

        public IEnumerable<Registration> GetRegistrationsMatchingSelector(DocumentSelector documentSelector) =>
            _registrations
               .Select(z => z.Value)
               .Where(
                    x => x.RegisterOptions is ITextDocumentRegistrationOptions ro &&
                         ro.DocumentSelector != null &&
                         ro.DocumentSelector
                           .Join(
                                documentSelector,
                                z => z.HasLanguage ? z.Language :
                                    z.HasScheme ? z.Scheme :
                                    z.HasPattern ? z.Pattern : string.Empty,
                                z => z.HasLanguage ? z.Language :
                                    z.HasScheme ? z.Scheme :
                                    z.HasPattern ? z.Pattern : string.Empty, (a, b) => a
                            )
                           .Any(y => y.HasLanguage || y.HasPattern || y.HasScheme)
                );

        public void Dispose()
        {
            if (_registrationSubject.IsDisposed) return;
            _registrationSubject.Dispose();
        }
    }
}

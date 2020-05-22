using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;
using ISerializer = OmniSharp.Extensions.LanguageServer.Protocol.Serialization.ISerializer;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    class RegistrationManager : IRegisterCapabilityHandler, IUnregisterCapabilityHandler, IRegistrationManager, IDisposable
    {
        private readonly ISerializer _serializer;
        private readonly ISourceCache<Registration, string> _registrations;

        public RegistrationManager(ISerializer serializer)
        {
            _serializer = serializer;
            _registrations = new SourceCache<Registration, string>(x => x.Id);
        }

        Task<Unit> IRequestHandler<RegistrationParams, Unit>.Handle(RegistrationParams request,
            CancellationToken cancellationToken)
        {
            Register(request.Registrations.ToArray());

            return Unit.Task;
        }

        // TODO:

        Task<Unit> IRequestHandler<UnregistrationParams, Unit>.Handle(UnregistrationParams request,
            CancellationToken cancellationToken)
        {
            _registrations.Edit(updater => {
                foreach (var item in request.Unregisterations ?? new UnregistrationContainer())
                {
                    updater.RemoveKey(item.Id);
                }
            });

            return Unit.Task;
        }

        public void RegisterCapabilities(ServerCapabilities serverCapabilities)
        {
            _registrations.Edit(updater => {
                foreach (var registrationOptions in LspHandlerDescriptorHelpers.GetStaticRegistrationOptions(
                    serverCapabilities))
                {
                    var descriptor = LspHandlerTypeDescriptorHelper.GetHandlerTypeForRegistrationOptions(registrationOptions);
                    if (descriptor == null)
                    {
                        // TODO: Log this
                        continue;
                    }

                    updater.AddOrUpdate(new Registration() {
                        Id = registrationOptions.Id,
                        Method = descriptor.Method,
                        RegisterOptions = registrationOptions
                    });
                }

                if (serverCapabilities.Workspace == null) return;

                foreach (var registrationOptions in LspHandlerDescriptorHelpers.GetStaticRegistrationOptions(serverCapabilities
                    .Workspace))
                {
                    var descriptor = LspHandlerTypeDescriptorHelper.GetHandlerTypeForRegistrationOptions(registrationOptions);
                    if (descriptor == null)
                    {
                        // TODO: Log this
                        continue;
                    }

                    updater.AddOrUpdate(new Registration() {
                        Id = registrationOptions.Id,
                        Method = descriptor.Method,
                        RegisterOptions = registrationOptions
                    });
                }
            });
        }

        // TODO: Register static handlers
        private void Register(params Registration[] registrations)
        {
            _registrations.Edit(updater => {
                foreach (var registration in registrations)
                {
                    Register(registration, updater);
                }
            });
        }

        private void Register(Registration registration, ISourceUpdater<Registration, string> updater)
        {
            var typeDescriptor = LspHandlerTypeDescriptorHelper.GetHandlerTypeDescriptor(registration.Method);
            if (typeDescriptor == null)
            {
                updater.AddOrUpdate(registration);
                return;
            }

            var deserializedRegistration = new Registration() {
                Id = registration.Id,
                Method = registration.Method,
                RegisterOptions = registration.RegisterOptions is JToken token
                    ? token.ToObject(typeDescriptor.RegistrationType, _serializer.JsonSerializer)
                    : registration.RegisterOptions
            };
            updater.AddOrUpdate(deserializedRegistration);
        }

        public IObservableList<Registration> Registrations => _registrations
            .Connect()
            .RemoveKey()
            .AsObservableList();

        public IObservableList<Registration> GetRegistrationsForMethod(string method) => _registrations
            .Connect()
            .RemoveKey()
            .Filter(x => x.Method == method)
            .AsObservableList();

        public IObservableList<Registration> GetRegistrationsMatchingSelector(DocumentSelector documentSelector) =>
            _registrations
                .Connect()
                .RemoveKey()
                .Filter(x => x.RegisterOptions is ITextDocumentRegistrationOptions ro && ro.DocumentSelector
                    .Join(documentSelector,
                        z => z.HasLanguage ? z.Language :
                            z.HasScheme ? z.Scheme :
                            z.HasPattern ? z.Pattern : string.Empty,
                        z => z.HasLanguage ? z.Language :
                            z.HasScheme ? z.Scheme :
                            z.HasPattern ? z.Pattern : string.Empty, (a, b) => a)
                    .Any(x => x.HasLanguage || x.HasPattern || x.HasScheme)
                )
                .AsObservableList();

        public void Dispose() => _registrations.Dispose();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    static class LanguageServerHelpers
    {
        static IEnumerable<T> GetUniqueHandlers<T>(CompositeDisposable disposable)
        {
            return GetUniqueHandlers(disposable).OfType<T>();
        }

        static IEnumerable<IJsonRpcHandler> GetUniqueHandlers(CompositeDisposable disposable)
        {
            return GetAllDescriptors(disposable)
                  .Concat(disposable.OfType<CompositeDisposable>().SelectMany(GetAllDescriptors))
                  .Concat(disposable.OfType<LspHandlerDescriptorDisposable>().SelectMany(GetAllDescriptors))
                  .Select(z => z.Handler)
                  .Distinct();
        }

        static IEnumerable<ILspHandlerDescriptor> GetAllDescriptors(CompositeDisposable disposable)
        {
            return disposable.OfType<ILspHandlerDescriptor>()
                             .Concat(disposable.OfType<CompositeDisposable>().SelectMany(GetAllDescriptors))
                             .Concat(disposable.OfType<LspHandlerDescriptorDisposable>().SelectMany(GetAllDescriptors))
                             .Distinct();
        }

        static IEnumerable<ILspHandlerDescriptor> GetAllDescriptors(LspHandlerDescriptorDisposable disposable)
        {
            return disposable.Descriptors;
        }

        internal static void InitHandlers(ILanguageServer client, CompositeDisposable result, ISupportedCapabilities supportedCapabilities)
        {
            Observable.Concat(
                GetUniqueHandlers<IOnLanguageServerInitialize>(result)
                   .Select(handler => Observable.FromAsync(ct => handler.OnInitialize(client, client.ClientSettings, ct)))
                   .Merge(),
                GetUniqueHandlers<IOnLanguageServerInitialized>(result)
                   .Select(handler => Observable.FromAsync(ct => handler.OnInitialized(client, client.ClientSettings, client.ServerSettings, ct)))
                   .Merge(),
                GetUniqueHandlers<IOnLanguageServerStarted>(result)
                   .Select(handler => Observable.FromAsync(ct => handler.OnStarted(client, ct)))
                   .Merge()
            ).Subscribe();
        }

        internal static IDisposable RegisterHandlers(
            IObservable<Unit> initializeComplete,
            IClientLanguageServer client,
            IServerWorkDoneManager serverWorkDoneManager,
            ISupportedCapabilities supportedCapabilities,
            IEnumerable<ILspHandlerDescriptor> collection
        )
        {
            var descriptors = new List<ILspHandlerDescriptor>();
            foreach (var descriptor in collection)
            {
                if (descriptor is LspHandlerDescriptor lspHandlerDescriptor &&
                    lspHandlerDescriptor.TypeDescriptor?.HandlerType != null &&
                    typeof(IDoesNotParticipateInRegistration).IsAssignableFrom(lspHandlerDescriptor.TypeDescriptor.HandlerType))
                {
                    continue;
                }

                descriptors.Add(descriptor);
            }

            return DynamicallyRegisterHandlers(client, initializeComplete, serverWorkDoneManager, supportedCapabilities, descriptors);
        }

        internal static IDisposable DynamicallyRegisterHandlers(
            IClientLanguageServer client,
            IObservable<Unit> initializeComplete,
            IServerWorkDoneManager serverWorkDoneManager,
            ISupportedCapabilities supportedCapabilities,
            IReadOnlyList<ILspHandlerDescriptor> descriptors
        )
        {
            if (descriptors.Count == 0)
                return Disposable.Empty; // No dynamic registrations supported by client.

            var disposable = new CompositeDisposable();

            var result = initializeComplete
                        .LastOrDefaultAsync()
                        .Select(
                             _ => {
                                 var registrations = new HashSet<Registration>();
                                 foreach (var descriptor in descriptors)
                                 {
                                     if (!descriptor.HasCapability || !supportedCapabilities.AllowsDynamicRegistration(descriptor.CapabilityType!)) continue;
                                     if (descriptor.RegistrationOptions is IWorkDoneProgressOptions wdpo)
                                     {
                                         wdpo.WorkDoneProgress = serverWorkDoneManager.IsSupported;
                                     }

                                     registrations.Add(
                                         new Registration {
                                             Id = descriptor.Id.ToString(),
                                             Method = descriptor.RegistrationMethod,
                                             RegisterOptions = descriptor.RegistrationOptions
                                         }
                                     );
                                 }

                                 return registrations.Distinct(new Registration.TextDocumentComparer()).ToArray();
                             }
                         )
                        .Where(z => z.Any())
                        .SelectMany(
                             registrations => Observable.FromAsync(ct => client.RegisterCapability(new RegistrationParams { Registrations = registrations.ToArray() }, ct)),
                             (a, b) => a
                         )
                        .Aggregate(Array.Empty<Registration>(), (z, _) => z)
                        .Subscribe(
                             registrations => {
                                 disposable.Add(
                                     Disposable.Create(
                                         () => {
                                             client.UnregisterCapability(
                                                 new UnregistrationParams {
                                                     Unregisterations = registrations.ToArray()
                                                 }
                                             ).ToObservable().Subscribe();
                                         }
                                     )
                                 );
                             }
                         );
            disposable.Add(result);
            return disposable;
        }

        internal static IDisposable RegisterHandlers(
            IObservable<Unit> initializeComplete,
            IClientLanguageServer client,
            IServerWorkDoneManager serverWorkDoneManager,
            ISupportedCapabilities supportedCapabilities,
            IDisposable handlerDisposable
        )
        {
            if (handlerDisposable is LspHandlerDescriptorDisposable lsp)
            {
                return new CompositeDisposable(
                    lsp, RegisterHandlers(
                        initializeComplete,
                        client,
                        serverWorkDoneManager,
                        supportedCapabilities,
                        lsp.Descriptors
                    )
                );
            }

            if (!( handlerDisposable is CompositeDisposable cd )) return Disposable.Empty;
            cd.Add(
                RegisterHandlers(
                    initializeComplete,
                    client,
                    serverWorkDoneManager,
                    supportedCapabilities,
                    cd.OfType<LspHandlerDescriptorDisposable>().SelectMany(z => z.Descriptors)
                )
            );
            return cd;
        }
    }
}

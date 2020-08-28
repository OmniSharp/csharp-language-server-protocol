using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
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
            return disposable.OfType<ILspHandlerDescriptor>()
                             .Select(z => z.Handler)
                             .OfType<T>()
                             .Concat(disposable.OfType<CompositeDisposable>().SelectMany(GetUniqueHandlers<T>))
                             .Concat(disposable.OfType<LspHandlerDescriptorDisposable>().SelectMany(GetLspHandlers<T>))
                             .Distinct();
        }

        static IEnumerable<T> GetLspHandlers<T>(LspHandlerDescriptorDisposable disposable)
        {
            return disposable.Descriptors
                             .Select(z => z.Handler)
                             .OfType<T>()
                             .Distinct();
        }

        internal static void InitHandlers(ILanguageServer client, CompositeDisposable result)
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
            Task initializeComplete,
            IClientLanguageServer client,
            IServerWorkDoneManager serverWorkDoneManager,
            ISupportedCapabilities supportedCapabilities,
            IEnumerable<ILspHandlerDescriptor> collection
        )
        {
            var registrations = new List<Registration>();
            foreach (var descriptor in collection)
            {
                if (descriptor is LspHandlerDescriptor lspHandlerDescriptor &&
                    lspHandlerDescriptor.TypeDescriptor?.HandlerType != null &&
                    typeof(IDoesNotParticipateInRegistration).IsAssignableFrom(lspHandlerDescriptor.TypeDescriptor.HandlerType))
                {
                    continue;
                }

                if (descriptor.HasCapability && supportedCapabilities.AllowsDynamicRegistration(descriptor.CapabilityType))
                {
                    if (descriptor.RegistrationOptions is IWorkDoneProgressOptions wdpo)
                    {
                        wdpo.WorkDoneProgress = serverWorkDoneManager.IsSupported;
                    }

                    registrations.Add(
                        new Registration {
                            Id = descriptor.Id.ToString(),
                            Method = descriptor.Method,
                            RegisterOptions = descriptor.RegistrationOptions
                        }
                    );
                }
            }

            // Fire and forget
            DynamicallyRegisterHandlers(client, initializeComplete, registrations.ToArray()).ToObservable().Subscribe();

            return Disposable.Create(
                () => {
                    client.UnregisterCapability(
                        new UnregistrationParams {
                            Unregisterations = registrations.ToArray()
                        }
                    ).ToObservable().Subscribe();
                }
            );
        }

        internal static async Task DynamicallyRegisterHandlers(IClientLanguageServer client, Task initializeComplete, Registration[] registrations)
        {
            if (registrations.Length == 0)
                return; // No dynamic registrations supported by client.

            var @params = new RegistrationParams { Registrations = registrations };

            await initializeComplete;

            await client.RegisterCapability(@params);
        }

        internal static IDisposable RegisterHandlers(
            Task initializeComplete,
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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    static class LanguageClientHelpers
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

        internal static void InitHandlers(ILanguageClient client, CompositeDisposable result)
        {
            Observable.Concat(
                GetUniqueHandlers<IOnLanguageClientInitialize>(result)
                   .Select(handler => Observable.FromAsync(ct => handler.OnInitialize(client, client.ClientSettings, ct)))
                   .Merge(),
                GetUniqueHandlers<IOnLanguageClientInitialized>(result)
                   .Select(handler => Observable.FromAsync(ct => handler.OnInitialized(client, client.ClientSettings, client.ServerSettings, ct)))
                   .Merge(),
                GetUniqueHandlers<IOnLanguageClientStarted>(result)
                   .Select(handler => Observable.FromAsync(ct => handler.OnStarted(client, ct)))
                   .Merge()
            ).Subscribe();
        }
    }
}
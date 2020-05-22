using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone
{
    public static class WorkDoneProxyExtensions
    {
        public static TResult ObserveWorkDone<T, TResult>(this IClientLanguageClient proxy, T @params, Func<IClientLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IClientLanguageClient proxy, T @params, Func<IClientLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IGeneralLanguageClient proxy, T @params, Func<IGeneralLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IGeneralLanguageClient proxy, T @params, Func<IGeneralLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this ITextDocumentLanguageClient proxy, T @params, Func<ITextDocumentLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this ITextDocumentLanguageClient proxy, T @params, Func<ITextDocumentLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IWindowLanguageClient proxy, T @params, Func<IWindowLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IWindowLanguageClient proxy, T @params, Func<IWindowLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IWorkspaceLanguageClient proxy, T @params, Func<IWorkspaceLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(this IWorkspaceLanguageClient proxy, T @params, Func<IWorkspaceLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer)
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        private static void DoObserveWorkDone(IClientProxy proxy, IWorkDoneProgressParams @params, IObserver<WorkDoneProgress> observer)
        {
            var token = @params.WorkDoneToken ??= new ProgressToken(Guid.NewGuid().ToString());
            proxy.WorkDoneManager.Monitor(token).Subscribe(observer);
        }

        private static void DoObserveWorkDone(IClientProxy proxy, IWorkDoneProgressParams @params, IWorkDoneProgressObserver observer)
        {
            var token = @params.WorkDoneToken ??= new ProgressToken(Guid.NewGuid().ToString());
            var observable = proxy.WorkDoneManager.Monitor(token);
            observable.Subscribe(
                v => {
                    switch (v)
                    {
                        case WorkDoneProgressBegin begin:
                            observer.OnBegin(begin);
                            break;
                        case WorkDoneProgressReport report:
                            observer.OnReport(report);
                            break;
                        case WorkDoneProgressEnd end:
                            observer.OnEnd(end);
                            break;
                    }
                },
                observer.OnError,
                observer.OnCompleted);
        }
    }
}

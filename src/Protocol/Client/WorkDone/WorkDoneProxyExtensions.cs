using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone
{
    public static class WorkDoneProxyExtensions
    {
        public static TResult ObserveWorkDone<T, TResult>(
            this IClientLanguageClient proxy, T @params, Func<IClientLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IClientLanguageClient proxy, T @params, Func<IClientLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IGeneralLanguageClient proxy, T @params, Func<IGeneralLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IGeneralLanguageClient proxy, T @params, Func<IGeneralLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this ITextDocumentLanguageClient proxy, T @params, Func<ITextDocumentLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this ITextDocumentLanguageClient proxy, T @params, Func<ITextDocumentLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IWindowLanguageClient proxy, T @params, Func<IWindowLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IWindowLanguageClient proxy, T @params, Func<IWindowLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IWorkspaceLanguageClient proxy, T @params, Func<IWorkspaceLanguageClient, T, TResult> func, IObserver<WorkDoneProgress> observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        public static TResult ObserveWorkDone<T, TResult>(
            this IWorkspaceLanguageClient proxy, T @params, Func<IWorkspaceLanguageClient, T, TResult> func, IWorkDoneProgressObserver observer
        )
            where T : IWorkDoneProgressParams
        {
            DoObserveWorkDone(proxy, @params, observer);
            return func(proxy, @params);
        }

        private static void DoObserveWorkDone(ILanguageProtocolProxy proxy, IWorkDoneProgressParams @params, IObserver<WorkDoneProgress> observer)
        {
            var token = SetWorkDoneToken(@params);
            proxy.GetRequiredService<IClientWorkDoneManager>().Monitor(token).Subscribe(observer);
        }

        private static void DoObserveWorkDone(ILanguageProtocolProxy proxy, IWorkDoneProgressParams @params, IWorkDoneProgressObserver observer)
        {
            var token = SetWorkDoneToken(@params);
            var observable = proxy.GetRequiredService<IClientWorkDoneManager>().Monitor(token);
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
                observer.OnCompleted
            );
        }

        private static readonly PropertyInfo WorkDoneTokenProperty = typeof(IWorkDoneProgressParams).GetProperty(nameof(IWorkDoneProgressParams.WorkDoneToken))!;

        private static ProgressToken SetWorkDoneToken(IWorkDoneProgressParams @params)
        {
            if (@params.WorkDoneToken is not null) return @params.WorkDoneToken;
            WorkDoneTokenProperty.SetValue(@params, new ProgressToken(Guid.NewGuid().ToString()));
            return @params.WorkDoneToken!;
        }
    }
}

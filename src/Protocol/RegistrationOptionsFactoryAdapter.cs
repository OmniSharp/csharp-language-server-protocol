using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public static class RegistrationAdapter<TCapability>
        where TCapability : ICapability
    {
        public static Func<TCapability, TRegistrationOptions> Adapt<TRegistrationOptions>(Func<TCapability, TRegistrationOptions>? registrationOptionsFactory)
            where TRegistrationOptions : class, new() => registrationOptionsFactory ?? ( _ => new TRegistrationOptions() );

        public static Func<TCapability, TRegistrationOptions> Adapt<TRegistrationOptions>(Func<TRegistrationOptions>? registrationOptionsFactory)
            where TRegistrationOptions : class, new() => _ => registrationOptionsFactory?.Invoke() ?? new TRegistrationOptions();

        public static Func<TCapability, TRegistrationOptions> Adapt<TRegistrationOptions>(TRegistrationOptions? registrationOptions)
            where TRegistrationOptions : class, new() => _ => registrationOptions ?? new TRegistrationOptions();
    }

    public static class RegistrationAdapter
    {
        public static Func<TRegistrationOptions> Adapt<TRegistrationOptions>(Func<TRegistrationOptions>? registrationOptionsFactory)
            where TRegistrationOptions : class, new() => registrationOptionsFactory ?? ( () => new TRegistrationOptions() );

        public static Func<TRegistrationOptions> Adapt<TRegistrationOptions>(TRegistrationOptions? registrationOptions)
            where TRegistrationOptions : class, new() => () => registrationOptions ?? new TRegistrationOptions();
    }

    public static class PartialAdapter
    {
        public static Action<TParams, IObserver<TItem>, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<TItem>, CancellationToken> handler)
            => handler;

        public static Action<TParams, IObserver<TItem>, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<TItem>> handler)
            => (a, o, _) => handler(a, o);

        public static Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> Adapt<TParams, TItem>(
            Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler
        )
            => handler;

        public static Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<IEnumerable<TItem>>> handler)
            => (a, o, _) => handler(a, o);
    }


    public static class PartialAdapter<TCapability>
        where TCapability : ICapability
    {
        public static Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> Adapt<TParams, TItem>(
            Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> handler
        )
            => handler;

        public static Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> Adapt<TParams, TItem>(
            Action<TParams, IObserver<IEnumerable<TItem>>, TCapability> handler
        )
            => (a, o, c, _) => handler(a, o, c);

        public static Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> Adapt<TParams, TItem>(
            Action<TParams, IObserver<IEnumerable<TItem>>, CancellationToken> handler
        )
            => (a, o, _, t) => handler(a, o, t);

        public static Action<TParams, IObserver<IEnumerable<TItem>>, TCapability, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<IEnumerable<TItem>>> handler)
            => (a, o, _, _) => handler(a, o);

        public static Action<TParams, IObserver<TItem>, TCapability, CancellationToken> Adapt<TParams, TItem>(
            Action<TParams, IObserver<TItem>, TCapability, CancellationToken> handler
        )
            => handler;

        public static Action<TParams, IObserver<TItem>, TCapability, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<TItem>, TCapability> handler)
            => (a, o, c, _) => handler(a, o, c);

        public static Action<TParams, IObserver<TItem>, TCapability, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<TItem>> handler)
            => (a, o, _, _) => handler(a, o);

        public static Action<TParams, IObserver<TItem>, TCapability, CancellationToken> Adapt<TParams, TItem>(Action<TParams, IObserver<TItem>, CancellationToken> handler)
            => (a, o, _, t) => handler(a, o, t);
    }

    public static class HandlerAdapter
    {
        public static Func<TParams, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, Task<TResult>> handler) => (a, _) => handler(a);
        public static Func<TParams, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, CancellationToken, Task<TResult>> handler) => handler;

        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Func<TParams, Task> handler) => (a, _) => handler(a);
        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Func<TParams, CancellationToken, Task> handler) => handler;

        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Action<TParams> handler) => (a, _) => {
            handler(a);
            return Task.CompletedTask;
        };

        public static Func<TParams, CancellationToken, Task> Adapt<TParams>(Action<TParams, CancellationToken> handler) => (a, t) => {
            handler(a, t);
            return Task.CompletedTask;
        };
    }

    public static class HandlerAdapter<TCapability>
        where TCapability : ICapability
    {
        public static Func<TParams, TCapability, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, TCapability, Task<TResult>> handler) =>
            (a, c, _) => handler(a, c);

        public static Func<TParams, TCapability, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, TCapability, CancellationToken, Task<TResult>> handler) =>
            handler;

        public static Func<TParams, TCapability, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, Task<TResult>> handler) => (a, _, _) => handler(a);

        public static Func<TParams, TCapability, CancellationToken, Task<TResult>> Adapt<TParams, TResult>(Func<TParams, CancellationToken, Task<TResult>> handler) =>
            (a, _, t) => handler(a, t);

        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Func<TParams, TCapability, CancellationToken, Task> handler) => handler;
        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Func<TParams, TCapability, Task> handler) => (a, c, _) => handler(a, c);
        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Func<TParams, Task> handler) => (a, _, _) => handler(a);
        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Func<TParams, CancellationToken, Task> handler) => (a, _, t) => handler(a, t);

        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Action<TParams, TCapability, CancellationToken> handler) => (a, c, t) => {
            handler(a, c, t);
            return Task.CompletedTask;
        };

        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Action<TParams, TCapability> handler) => (a, c, _) => {
            handler(a, c);
            return Task.CompletedTask;
        };

        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Action<TParams> handler) => (a, _, _) => {
            handler(a);
            return Task.CompletedTask;
        };

        public static Func<TParams, TCapability, CancellationToken, Task> Adapt<TParams>(Action<TParams, CancellationToken> handler) => (a, _, t) => {
            handler(a, t);
            return Task.CompletedTask;
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class ClientCapabilityProvider
    {
        private readonly IHandlerCollection _collection;

        public ClientCapabilityProvider(IHandlerCollection collection)
        {
            _collection = collection;
        }

        public bool HasStaticHandler<T>(Supports<T> capability)
            where T : DynamicCapability, ConnectedCapability<IJsonRpcHandler>
        {
            if (!capability.IsSupported) return false;
            if (capability.Value == null) return false;
            if (capability.Value.DynamicRegistration) return false;

            var handlerType = typeof(T).GetTypeInfo().ImplementedInterfaces
                .Single(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ConnectedCapability<>))
                .GetTypeInfo().GetGenericArguments()[0].GetTypeInfo();
            return !capability.Value.DynamicRegistration &&
                _collection.Any(z =>
                    z.HandlerType.GetTypeInfo().IsAssignableFrom(handlerType) ||
                    z.Handler.GetType().GetTypeInfo().IsAssignableFrom(handlerType));
        }

        public IOptionsGetter GetStaticOptions<T>(Supports<T> capability)
            where T : DynamicCapability, ConnectedCapability<IJsonRpcHandler>
        {
            return !HasStaticHandler(capability) ? Null : new OptionsGetter(_collection);
        }

        private static readonly IOptionsGetter Null = new NullOptionsGetter();

        public interface IOptionsGetter
        {
            /// <summary>
            /// Gets a single option from a given interface.
            /// </summary>
            /// <param name="action"></param>
            /// <typeparam name="TInterface"></typeparam>
            /// <typeparam name="TOptions"></typeparam>
            /// <returns></returns>
            TOptions Get<TInterface, TOptions>(Func<TInterface, TOptions> action)
                where TOptions : class;

            /// <summary>
            /// Reduces the options from multiple interfaces to a single option.
            /// </summary>
            /// <typeparam name="TInterface"></typeparam>
            /// <typeparam name="TOptions"></typeparam>
            /// <param name="action"></param>
            /// <returns></returns>
            TOptions Reduce<TInterface, TOptions>(Func<IEnumerable<TInterface>, TOptions> action)
                where TOptions : class;
        }

        private class NullOptionsGetter : IOptionsGetter
        {
            public TOptions Get<TInterface, TOptions>(Func<TInterface, TOptions> action)
                where TOptions : class
            {
                return null;
            }

            public TOptions Reduce<TInterface, TOptions>(Func<IEnumerable<TInterface>, TOptions> action)
                where TOptions : class
            {
                return null;
            }
        }

        private class OptionsGetter : IOptionsGetter
        {
            private readonly IHandlerCollection _collection;

            public OptionsGetter(IHandlerCollection collection)
            {
                _collection = collection;
            }

            public TOptions Get<TInterface, TOptions>(Func<TInterface, TOptions> action)
                where TOptions : class
            {
                return _collection
                    .Select(x => x.Registration?.RegisterOptions is TInterface cl ? action(cl) : null)
                    .FirstOrDefault(x => x != null);
            }

            public TOptions Reduce<TInterface, TOptions>(Func<IEnumerable<TInterface>, TOptions> action)
                where TOptions : class
            {
                return action(_collection
                    .Select(x => x.Registration?.RegisterOptions is TInterface cl ? cl : default)
                    .Where(x => x != null));
            }
        }
    }
}

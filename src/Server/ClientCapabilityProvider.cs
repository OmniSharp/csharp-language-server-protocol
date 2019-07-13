using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
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
            // Dynamic registration will cause us to double register things if we report our capabilities staticly.
            // However if the client does not tell us it's capabilities we should just assume that they do not support
            // dynamic registraiton
            if (capability.IsSupported && capability.Value != null && capability.Value.DynamicRegistration == true) return false;

            var handlerTypes = typeof(T).GetTypeInfo().ImplementedInterfaces
                .Where(x => x.GetTypeInfo().IsGenericType && x.GetTypeInfo().GetGenericTypeDefinition() == typeof(ConnectedCapability<>))
                .Select(x => x.GetTypeInfo().GetGenericArguments()[0].GetTypeInfo());

            return handlerTypes.All(_collection.ContainsHandler);
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

            public Supports<TOptions> Can<TInterface, TOptions>(Func<TInterface, TOptions> action)
                where TOptions : class
            {
                var options = _collection
                    .Select(x => x.Registration?.RegisterOptions is TInterface cl ? action(cl) : null)
                    .FirstOrDefault(x => x != null);
                if (options == null)
                    return Supports.OfBoolean<TOptions>(false);

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
